using Sandbox;

[Library( "weapon_fists", Title = "Fists", Spawnable = false )]
partial class Fists : Weapon
{
	public override string ViewModelPath => "models/first_person/first_person_arms.vmdl";
	public override float PrimaryRate => 123.5f/60f;
	public override float SecondaryRate => 123.5f/60f;
    public override bool UsesAmmo => false;

	public override bool CanReload()
	{
		return false;
	}

	private void Attack( bool leftHand )
	{
		if ( MeleeAttack() )
		{
			OnMeleeHit( leftHand );
		}
		else
		{
			OnMeleeMiss( leftHand );
		}

		(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );
	}

    public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Down( InputButton.Attack1 );
	}

	public override void AttackPrimary()
	{
		Attack( true );
	}

	public override void AttackSecondary()
	{
		Attack( false );
	}

	public override void OnCarryDrop( Entity dropper )
	{
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 5 );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( Owner.IsValid() )
		{
			ViewModelEntity?.SetAnimParameter( "b_grounded", Owner.GroundEntity.IsValid() );
			ViewModelEntity?.SetAnimParameter( "aim_pitch", Owner.EyeRotation.Pitch() );
		}
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
			EnableViewmodelRendering = true,
			EnableSwingAndBob = false,
		};

		ViewModelEntity.SetModel( ViewModelPath );
		ViewModelEntity.SetAnimGraph( "models/first_person/first_person_arms_punching.vanmgrph" );
	}

	private bool MeleeAttack()
	{
		var forward = Owner.EyeRotation.Forward;
		forward = forward.Normal;

		bool hit = false;
		if(Host.IsServer)DoomMap.GetSector(Owner.Position).PropogateSound(Owner);
		foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 80, 20.0f ) )
		{
			if ( !tr.Entity.IsValid() ) continue;

			tr.Surface.DoBulletImpact( tr );

			hit = true;

			if ( !IsServer ) continue;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100, DoomGame.RNG.Next()%18+2)
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}

		return hit;
	}

	[ClientRpc]
	private void OnMeleeMiss( bool leftHand )
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "attack_has_hit", false );
		ViewModelEntity?.SetAnimParameter( "attack", true );
		ViewModelEntity?.SetAnimParameter( "holdtype_attack", leftHand ? 2 : 1 );
	}

	[ClientRpc]
	private void OnMeleeHit( bool leftHand )
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "attack_has_hit", true );
		ViewModelEntity?.SetAnimParameter( "attack", true );
		ViewModelEntity?.SetAnimParameter( "holdtype_attack", leftHand ? 2 : 1 );
	}

	public override string GetObituary(DoomPlayer victim, DoomPlayer killer){
		return $"{killer.Client?.Name??"Doomguy"} beat {victim.Client?.Name??"Doomguy"} to death with his bare hands!";
	}
}