using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

public partial class DoomPlayer : Player
{
	public struct KeyData {
		public byte Blue;
		public byte Yellow;
		public byte Red;
		public KeyData(byte blue, byte yellow, byte red) {
			Blue = blue;
			Yellow = yellow;
			Red = red;
		}

		public KeyData WithBlue(byte blue) {
			return new KeyData(blue, Yellow, Red);
		}
		public KeyData WithYellow(byte yellow) {
			return new KeyData(Blue, yellow, Red);
		}
		public KeyData WithRed(byte red) {
			return new KeyData(Blue, Yellow, red);
		}
	}

	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;

	private DamageInfo lastDamage;

	[Net] public PawnController VehicleController { get; set; }
	[Net] public PawnAnimator VehicleAnimator { get; set; }
	[Net, Predicted] public CameraMode VehicleCamera { get; set; }
	[Net, Predicted] public Entity Vehicle { get; set; }
	[Net, Predicted] public CameraMode MainCamera { get; set; }
	[Net, Predicted] public bool IsThirdPerson {get; set;}
	[Net, Predicted] public int Armor {get; set;}
	[Net, Predicted] public bool HasStrongArmor {get; set;}

	//[Net] public (int blue, int yellow, int red) keys {get;set;} 
	[Net, Predicted] public KeyData Keys {get;set;}

	public ClothingContainer Clothing = new();

	[Net] public bool GodMode {get; set;}

	public CameraMode LastCamera { get; set; }

	public static SoundEvent FallDamage = new SoundEvent("sounds/physics/bullet_impacts/flesh_npc_04.vsnd");
	public DoomPlayer()
	{
		Inventory = new Inventory( this );
	}

	public DoomPlayer( Client cl ) : this()
	{
		Clothing.LoadFromClient( cl );
	}


	public override void Spawn()
	{
		IsThirdPerson = false;
		MainCamera = new FirstPersonCamera();
		LastCamera = MainCamera;

		Keys = new(0,0,0);

		base.Spawn();
	}
	public override void ClientSpawn()
	{
		IsThirdPerson = false;
		MainCamera = new FirstPersonCamera();
		LastCamera = MainCamera;

		base.ClientSpawn();
	}

	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new TacoWalkController();
		Animator = new StandardPlayerAnimator();

		MainCamera = LastCamera;
		CameraMode = MainCamera;

		if ( DevController is NoclipController )
		{
			DevController = null;
		}

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity( this );

		//if(IsClient)
		//	InventoryBar.Instance.ReBuild();

		Inventory.Add( new DoomFist() );
        var pistol = new DoomPistol();
		Inventory.Add( pistol );
        ActiveChild = pistol;

		FallDamage.Sounds = new List<string>(new[]{
			"sounds/physics/bullet_impacts/flesh_npc_01.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_02.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_03.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_04.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_05.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_06.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_07.vsnd",
			"sounds/physics/bullet_impacts/flesh_npc_08.vsnd",
		});

		bulletAmmo = 50;
		shellAmmo = 0;
		rocketAmmo = 10;
		cellAmmo = 0;

		Armor = 0;
		HasStrongArmor = false;

		AddCollisionLayer(CollisionLayer.Hitbox);
		AddCollisionLayer(CollisionLayer.Player);

		if(DoomGame.DEATHMATCH){
			Keys = new(2,2,2);
		}
		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
		{
			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
			PlaySound( "kersplat" );
		}

		VehicleController = null;
		VehicleAnimator = null;
		VehicleCamera = null;
		Vehicle = null;

		if(lastDamage.Damage > 50f){
			SoundLoader.PlaySound("DSPDIEHI", Position);
		}else{
			SoundLoader.PlaySound("DSPLDETH", Position);
		}

		StatusText.AddChatEntry(To.Everyone, "", DoomGame.GetObituary(lastDamage.Attacker, this, lastDamage.Weapon));

		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
		LastCamera = MainCamera;
		MainCamera = new SpectateRagdollCamera();
		CameraMode = MainCamera;
		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		Inventory.DeleteContents();
	}

	public override void TakeDamage( DamageInfo info )
	{
		Velocity += info.Force;
		if(GodMode)return;
		if(IsServer){
			SoundLoader.PlaySound("DSPLPAIN", Position);
			DamageFlash.SetFlash(To.Single(this), info.Damage > 20f ? 0f : 3f/5f);
		}
		lastDamage = info;
		int totalDamage = (int)info.Damage;
		int armorAbsorb = (int)(info.Damage / (HasStrongArmor ? 2f : 3f));
		if(Armor > armorAbsorb){
			Armor -= armorAbsorb;
			totalDamage -= armorAbsorb;
		}else{
			totalDamage -= Armor;
			Armor = 0;
			HasStrongArmor = false;
		}
		lastAttacker = info.Attacker;
		info.Damage = totalDamage;
		TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );
		base.TakeDamage( info );
	}

	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	public override PawnController GetActiveController()
	{
		if ( VehicleController != null ) return VehicleController;
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}

	public override PawnAnimator GetActiveAnimator()
	{
		if ( VehicleAnimator != null ) return VehicleAnimator;

		return base.GetActiveAnimator();
	}

	public CameraMode GetActiveCamera()
	{
		//if ( VehicleCamera != null ) return VehicleCamera;

		return MainCamera;
	}

	public override void FrameSimulate(Client cl){
		base.FrameSimulate(cl);
	}

	[Event.Tick.Client]
    public void UpdateShading(){
        if(!IsValid) return;
        var sector = DoomMap.GetSector(Position);
		if(sector!=null){
			RenderColor = new Color(sector.brightness,sector.brightness,sector.brightness,1);
			foreach(var cloth in Children.OfType<ModelEntity>()){
				cloth.RenderColor = new Color(sector.brightness,sector.brightness,sector.brightness,1);
			}
		}
    }

	public TimeSince RampageStart = 0;
	[Net] Entity lastAttacker {get;set;}
	public int lastHealth {get;set;}
	public TimeSince lastSectorDamage = 0f;
	public override void Simulate( Client cl )
	{
		if(IsClient){
			int hp = (int)Health;
			if(hp > lastHealth)lastHealth = hp;
			if(hp < lastHealth){
				int damageTaken = lastHealth - hp;
				if(lastAttacker != null){
					if(damageTaken >= 20){
						Face.FlashFace("OUCH", 0.5f, 8);
					}else{
						float dOff = Rotation.LookAt(lastAttacker.WorldSpaceBounds.Center - WorldSpaceBounds.Center).Angles().yaw - Rotation.Angles().yaw;
						if(dOff > 180)dOff -= 360;
						if(dOff < -180)dOff += 360;
						if(dOff < -45f){
							Face.FlashFace("TL", 0.5f, 8);
						}else if(dOff > 45f){
							Face.FlashFace("TR", 0.5f, 8);
						}else{
							Face.FlashFace("KILL", 0.5f, 8);
						}
					}
				}else{
					Face.FlashFace(damageTaken >= 20 ? "OUCH" : "KILL", 0.5f, 7);
				}
			lastHealth = hp;
			}
		}else{
			var sectr = Trace.Box(CollisionBounds, Position, Position+new Vector3(0,0,-1))
				.Ignore(this)
				.HitLayer(CollisionLayer.NPC_CLIP)
			.Run();
			if(sectr.Entity is SectorMeshProp smp){
				switch(smp.sector.specialType){
					case 4: case 11: case 16:{
						if(lastSectorDamage > 1){
							TakeDamage(DamageInfo.Generic(20));
							lastSectorDamage = 0;
						}
						break;
					}
					case 5:{
						if(lastSectorDamage > 1){
							TakeDamage(DamageInfo.Generic(10));
							lastSectorDamage = 0;
						}
						break;
					}
					case 7:{
						if(lastSectorDamage > 1){
							TakeDamage(DamageInfo.Generic(5));
							lastSectorDamage = 0;
						}
						break;
					}
					default:
						lastSectorDamage = 0;
						break;
				}
			}
		}
		DoomGame.RNG = new System.Random(Time.Tick);
		for(int i=0; i < 30; i++)
			DoomGame.RNG.Next();
		lastEyeTrace = null;
		base.Simulate( cl );
		if(IsServer && MainCamera is ThirdPersonCameraTracked tr)tr.Update();

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		if ( VehicleController != null && DevController is NoclipController )
		{
			DevController = null;
		}

		var controller = GetActiveController();
		if ( controller != null )
			EnableSolidCollisions = !controller.HasTag( "noclip" );

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if(!Input.Down(InputButton.PrimaryAttack))RampageStart = 0;

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( IsThirdPerson )
			{
				MainCamera = new FirstPersonCamera();
				Log.Info($"Made First person! {Host.IsServer}");
				IsThirdPerson = false;
			}
			else
			{
				MainCamera = new ThirdPersonCameraTracked{Pawn = this};
				Log.Info($"Made Third person! {Host.IsServer}");
				IsThirdPerson = true;
			}
		}

		CameraMode = GetActiveCamera();

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				//Game.Current?.DoPlayerNoclip( cl );
			}

			timeSinceJumpReleased = 0;
		}

		if ( Input.Left != 0 || Input.Forward != 0 )
		{
			timeSinceJumpReleased = 1;
		}
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
		if(other is ThingEntity te){
			te.OnTouched(this);
		}
	}

	[ConCmd.Server( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = (Player)ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( slot.ClassName !=  entName  )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}

	public (Vector3 position, Rotation angle) CameraPosition(){
		if(CameraMode is ThirdPersonCameraTracked tr){
			return (tr.Position, tr.Rotation);
		}else{
			return (EyePosition, EyeRotation);
		}
	}

	TraceResult? lastEyeTrace = null;
	public TraceResult EyeTrace(){
		if(lastEyeTrace is not null){
			return (TraceResult)lastEyeTrace;
		}
		(var pos, var ang) = CameraPosition();
		var trace = Trace.Ray( pos, pos + ang.Forward * 10000.0f )
			.UseHitboxes()
			.Ignore( this, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		lastEyeTrace = trace;
		return trace;
	}

	[Event.Frame]
	public void ResetTraceOnFrame(){
		lastEyeTrace = null;
	}
}
