using Sandbox;

[Library( "weapon_shotgun", Title = "Shotgun", Spawnable = true, Group = "Weapon" )]
partial class Shotgun : Weapon
{
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override float PrimaryRate => 56.8f/60f;

	public override AmmoType Clip1Type => AmmoType.Shell;
	public override int HoldSlot => 3;
	public virtual float Damage => DoomGame.RNG.Next()%10+5;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl" );
	}

	public override void AttackPrimary()
	{
		if(Host.IsServer)DoomMap.GetSector(Owner.Position).PropogateSound(Owner);
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "rust_pumpshotgun.shoot" );

		//
		// Shoot the bullets
		//
		ShootBullets( 7, new Vector2(5.6f, 0f), 10.0f, Damage, 3.0f );
		(Owner as DoomPlayer).RemoveAmmo(Clip1Type, Ammo1PerShot);
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParameter( "fire", true );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 1.0f, 1.5f, 2.0f );
		}

		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 3 ); // TODO this is shit
	}
}
