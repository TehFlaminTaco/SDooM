using System.Collections.Generic;
using Sandbox;

[Library( "weapon_pistol", Title = "Pistol", Spawnable = true, Group = "Weapon" )]
partial class Pistol : Weapon
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 150f/60f;
	public override float SecondaryRate => 1.0f;
	public TimeSince TimeSinceDischarge { get; set; }

	public override AmmoType Clip1Type => AmmoType.Bullet;
	public override int HoldSlot => 2;

	public virtual float Damage => DoomGame.RNG.Next()%10+5;
	public virtual float Force => 1.5f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Down( InputButton.Attack1 ) && (Owner as DoomPlayer).HasAmmo(Clip1Type, Ammo1PerShot);
	}

	public override void AttackPrimary()
	{
		if(Host.IsServer)DoomMap.GetSector(Owner.Position).PropogateSound(Owner);
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
		
		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );
		ShootBullet( new Vector2(Input.Pressed(InputButton.Attack1) ? 0f : 5.6f, 0f), Force, Damage, 3.0f );
		(Owner as DoomPlayer).RemoveAmmo(Clip1Type, Ammo1PerShot);
	}
}
