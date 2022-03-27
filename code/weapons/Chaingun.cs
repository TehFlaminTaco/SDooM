using Sandbox;

[Library( "weapon_chaingun", Title = "Chaingun", Spawnable = true, Group = "Weapon" )]
partial class Chaingun : Weapon
{
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float PrimaryRate => 525.0f/60f;
	public virtual float Damage => DoomGame.RNG.Next()%10+5;

	public override AmmoType Clip1Type => AmmoType.Bullet;
	public override int HoldSlot => 4;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
	}
	[Net, Predicted] bool refire {get;set;}
	[Net, Predicted] int shotCount {get;set;}
	public override bool CanPrimaryAttack()
	{
		return Owner.IsValid() && TimeSincePrimaryAttack > (1 / PrimaryRate) && (refire||Input.Down( InputButton.Attack1 )) && (Owner as DoomPlayer).HasAmmo(Clip1Type, Ammo1PerShot);
	}

	public override void AttackPrimary()
	{
		if(Host.IsServer)DoomMap.GetSector(Owner.Position).PropogateSound(Owner);
		refire = !refire;
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if(Input.Pressed(InputButton.Attack1))shotCount=0;
		shotCount++;
		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "rust_smg.shoot" );
		(Owner as DoomPlayer).RemoveAmmo(Clip1Type, Ammo1PerShot);
		//
		// Shoot the bullets
		//
		ShootBullet( new Vector2(shotCount <= 2 ? 0f : 5.6f, 0f), 1.5f, Damage, 3.0f );
	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin( 0.5f, 4.0f, 1.0f, 0.5f );
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 2 ); // TODO this is shit
	}

}
