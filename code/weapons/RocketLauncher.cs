using System.Collections.Generic;
using Sandbox;

[Library( "weapon_rocketlauncher", Title = "Rocket Launcher", Spawnable = true, Group = "Weapon" )]
partial class RocketLauncher : Weapon
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public TimeSince TimeSinceDischarge { get; set; }

	public override AmmoType Clip1Type => AmmoType.Rocket;
	public override int HoldSlot => 5;

	public virtual float Damage => 9.0f;
	public virtual float Force => 1.5f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 ) && (Owner as DoomPlayer).HasAmmo(Clip1Type, Ammo1PerShot);
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
		
		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );
		//ShootBullet( 0.05f, Force, Damage, 3.0f );
		if(IsServer){
			var startPos = Owner.EyePosition + Owner.EyeRotation.Forward * 30f;
			var dir = Rotation.LookAt(((Owner as DoomPlayer).EyeTrace().EndPosition - startPos).Normal);
			var rocket = new Rocket();
			rocket.Position = startPos;
			rocket.Rotation = dir.RotateAroundAxis(Vector3.Right, 90f);
			rocket.ApplyAbsoluteImpulse(dir.Forward * 30000f);
		}
		(Owner as DoomPlayer).RemoveAmmo(Clip1Type, 1);
	}

	private void Discharge()
	{
		if ( TimeSinceDischarge < 0.5f )
			return;

		TimeSinceDischarge = 0;

		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var pos = muzzle.Position;
		var rot = muzzle.Rotation;

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );
		//ShootBullet( pos, rot.Forward, 0.05f, Force, Damage, 3.0f );

		ApplyAbsoluteImpulse( rot.Backward * 200.0f );
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if ( eventData.Speed > 500.0f )
		{
			Discharge();
		}
	}
}

partial class Rocket : Prop {
	float armTime = 0.0f;
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/sbox_props/gas_cylinder_tall/gas_cylinder_tall.vmdl" );
		Scale = 0.2f;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		armTime = Time.Now + 0.1f;
    }

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if(Time.Now > armTime)
			TakeDamage(DamageInfo.Generic(90000f));
	}
}