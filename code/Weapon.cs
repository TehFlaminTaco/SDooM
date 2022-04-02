using System.Collections.Generic;
using Sandbox;

public partial class Weapon : BaseWeapon, IUse
{
	public virtual int HoldSlot => 1;

	public PickupTrigger PickupTrigger { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		PickupTrigger = new PickupTrigger
		{
			Parent = this,
			Position = Position,
			EnableTouch = true,
			EnableSelfCollisions = false
		};

		PickupTrigger.PhysicsBody.AutoSleep = false;
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;
	}

	public override void Reload()
	{
		return;
	}

	public override bool CanReload()
	{
		return false;
	}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		base.Simulate( owner );
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};

		ViewModelEntity.SetModel( ViewModelPath );
	}

	public bool OnUse( Entity user )
	{
		if ( Owner != null )
			return false;

		if ( !user.IsValid() )
			return false;

		user.StartTouch( this );

		return false;
	}

	public virtual bool IsUsable( Entity user )
	{
		var player = user as Player;
		if ( Owner != null ) return false;

		if ( player.Inventory is Inventory inventory )
		{
			return inventory.CanAdd( this );
		}

		return true;
	}

	public void Remove()
	{
		//PhysicsGroup?.Wake();
		Delete();
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			_ = new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f ){
		bool InWater = Map.Physics.IsPointWater( start );

		var tr = Trace.Ray( start, end )
				.HitLayer(CollisionLayer.All ^ CollisionLayer.Hitbox, false)
				.HitLayer(CollisionLayer.Hitbox)
				.HitLayer(CollisionLayer.Player)
				.HitLayer(CollisionLayer.NPC)
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();

		yield return tr;

		//
		// Another trace, bullet going through thin material, penetrating water surface?
		//
	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	private static Surface FleshSurface = Asset.FromPath<Surface>("surfaces/flesh.surface");
	public virtual void ShootBullet( Vector3 pos, Vector3 dir, Vector2 spread, float force, float damage, float bulletSize )
	{
		var forward = dir;
		//forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward *= Rotation.FromYaw((float)(DoomGame.RNG.NextDouble()*2d-1d)*spread.x) * Rotation.FromPitch((float)(DoomGame.RNG.NextDouble()*2d-1d)*spread.y);
		forward = forward.Normal;

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( pos, pos + forward * 5000, bulletSize ) )
		{
			FleshSurface ??= Asset.FromPath<Surface>("surfaces/flesh.surface");
			if(tr.Entity is Monster)
				FleshSurface.DoBulletImpact(tr);
			else
				tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			//
			// We turn predictiuon off for this, so any exploding effects don't get culled etc
			//
			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );

				if(tr.Entity is LineMeshProp lmp){
					lmp.OnShoot();
				}
			}
		}
	}

	/// <summary>
	/// Shoot a single bullet from owners view point
	/// </summary>
	public virtual void ShootBullet( Vector2 spread, float force, float damage, float bulletSize )
	{
		(var pos, var dir) = (Owner as DoomPlayer).CameraPosition();
		ShootBullet( pos, dir.Forward, spread, force, damage, bulletSize );
	}

	/// <summary>
	/// Shoot a multiple bullets from owners view point
	/// </summary>
	public virtual void ShootBullets( int numBullets, Vector2 spread, float force, float damage, float bulletSize )
	{
		(var pos, var dir) = (Owner as DoomPlayer).CameraPosition();

		for ( int i = 0; i < numBullets; i++ )
		{
			ShootBullet( pos, dir.Forward, spread, force / numBullets, damage, bulletSize );
		}
	}

	public virtual string GetObituary(DoomPlayer victim, DoomPlayer killer){
		return $"{victim.Client?.Name??"Doomguy"} was fragged by {killer.Client?.Name??"Doomguy"} using the {this.GetType().Name}";
	}

	public virtual Vector3 ViewmodelOffset => new Vector3(0f,17f,0f);
}
