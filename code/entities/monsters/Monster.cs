using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

public partial class Monster : ThingEntity {
    public virtual string SpriteRoot => "";

    int spriteIndex = 0;
    TimeSince nextFrame = 0;
    public string animationSteps = "";
    public int animationTime = 0;
    [Net] public char frameName {get;set;} = 'A';
    [Net] public bool hasFacing {get;set;} = true;
    [Net] public bool isAmbusher {get;set;} = false;
    public virtual int MoveSpeed => 8;
    public virtual int MaxHealth => 40;
    public virtual int PainChance => 0;
    public virtual string GibSound => "DSSLOP";
    public virtual string PainSound => null;
    public virtual string[] DeathSound => Array.Empty<string>();
    public virtual string[] SeeSound => Array.Empty<string>();
    public virtual string[] ActiveSound => Array.Empty<string>();
    public virtual bool HasMelee => false;
    public virtual bool HasMissile => false;
    [Net] public MonsterState currentState {get;set;}
    [Net] public Entity Target {get;set;}
    private IEnumerator StateHandler;
    public bool JustAttacked = false;
    public bool JustHit = false;
    public int ReactionTime = 8;

    [Event.Tick.Server]
    public void UpdateState(){
        if(animationSteps.Length > 0){
            if(nextFrame>=0){
                nextFrame =-(float)animationTime/35f;
                frameName = animationSteps[spriteIndex];
                spriteIndex++;
                if(spriteIndex >= animationSteps.Length){
                    spriteIndex = 0;
                    animationSteps = "";
                    StepState();
                }
            }
        }else{
            StepState();
        }
    }
    [Event.Frame]
    public void UpdateSpriteFacing(){
        SpriteName = $"{SpriteRoot}{frameName}{(hasFacing?Facing():"0")}";
    }

    private void StepState(){
        FullBright = false;
        if(!GetStateHandler().MoveNext()){
            StateHandler = null;
            GetStateHandler();
        }
    }

    public DamageInfo murderAttack = DamageInfo.Generic(0);
    public void SetState(MonsterState state){
        if(currentState != state){
            StateHandler = null;
            animationTime = 0;
            animationSteps = "";
            nextFrame = 0;
        }
        currentState = state;
        GetStateHandler();
    }

    private IEnumerator GetStateHandler(){
        if(StateHandler != null)
            return StateHandler;
        switch(currentState){
            case MonsterState.Spawn:
                StateHandler = StateSpawn();
                break;
            case MonsterState.See:
                StateHandler = StateSee();
                break;
            case MonsterState.Missile:
                StateHandler = StateMissile();
                break;
            case MonsterState.Melee:
                StateHandler = StateMelee();
                break;
            case MonsterState.Pain:
                StateHandler = StatePain();
                break;
            case MonsterState.Death:
                StateHandler = StateDeath();
                break;
            case MonsterState.XDeath:
                StateHandler = StateXDeath();
                break;
            case MonsterState.Raise:
                StateHandler = StateRaise();
                break;
            case MonsterState.Dead:
                StateHandler = StateDead();
                break;
        }
        return StateSpawn();
    }

    bool heardEnemy = false;
    public bool LookForTargets(){
        var sec = DoomMap.GetSector(Position);
        if(sec == null)return false;
        if(sec.SoundTarget!=null && sec.SoundTarget.IsValid && sec.SoundTarget.Health>0){
            heardEnemy = true;
            if(!isAmbusher){
                Target = sec.SoundTarget;
                if(SeeSound.Length > 0){
                    SoundLoader.PlaySound(SeeSound[DoomGame.RNG.Next()%SeeSound.Length], Position);
                }
                SetState(MonsterState.See);
                return true;
            }
        }
        foreach(var ply in All.OfType<DoomPlayer>()){
            var ang = Rotation.LookAt(ply.Position - Position).Angles().yaw - Rotation.Angles().yaw;
            while(ang<-180)ang+=360;
            while(ang>180)ang-=360;
            if(!heardEnemy && (ang > 90 || ang < -90)){
                continue;
            }
            var tr = Trace.Ray(WorldSpaceBounds.Center, ply.WorldSpaceBounds.Center)
                .Ignore(this)
                .HitLayer(CollisionLayer.Player)
                .HitLayer(CollisionLayer.NPC)
                .HitLayer(CollisionLayer.NPC_CLIP)
                .Run();
            if(tr.Entity == ply){
                Target = ply;
                if(SeeSound.Length > 0){
                    SoundLoader.PlaySound(SeeSound[DoomGame.RNG.Next()%SeeSound.Length], Position);
                }
                SetState(MonsterState.See);
                return true;
            }
        }
        return false;
    }

    public int moveDir = 0;
    public int curDir = 0;
    public int moveCount = 0;
    public void FaceTarget(){
        if(Target == null || !Target.IsValid)return;
        moveDir = (int)((Rotation.LookAt(Target.WorldSpaceBounds.Center - WorldSpaceBounds.Center).Angles().yaw + 25.5f)/45f);
        if(moveDir >= 8)moveDir -= 8;
        if(moveDir < 0)moveDir += 8;
        curDir = moveDir;
        Rotation = Rotation.FromYaw(curDir * 45f);
    }
    public void Chase(){
        if(ReactionTime > 0)ReactionTime--;
        if(DoomGame.RNG.Next()%256 < 3){
            if(ActiveSound.Length > 0){
                SoundLoader.PlaySound(ActiveSound[DoomGame.RNG.Next()%ActiveSound.Length], Position);
            }
        }
        if(Target == null || !Target.IsValid || Target.Health <= 0){
            if(!LookForTargets()){
                SetState(MonsterState.Spawn);
            }
            return;
        }
        int dDir = moveDir - curDir;
        if(dDir > 4)dDir -= 4;
        if(dDir < -4)dDir += 4;
        if(dDir > 0)curDir += 1;
        if(dDir < 0)curDir -= 1;
        Rotation = Rotation.FromYaw(curDir * 45f);
        if(JustAttacked){
            JustAttacked = false;
            NewChaseDir();
            return;
        }
        if(HasMelee){
            if(Target.WorldSpaceBounds.Center.Distance(WorldSpaceBounds.Center) < 64f){
                SetState(MonsterState.Melee);
                return;
            }
        }
        if(HasMissile){
            if(moveCount <= 0){
                if(CheckMissileRange()){
                    JustAttacked = true;
                    SetState(MonsterState.Missile);
                    return;
                }
            }
        }
        if(!Move()){
            NewChaseDir();
        }
        if(moveCount <= 0){
            NewChaseDir();
        }
        moveCount--;
    }

    public bool CheckMissileRange(){
        var tr = Trace.Ray(WorldSpaceBounds.Center, Target.WorldSpaceBounds.Center)
            .Ignore(this)
            .HitLayer(CollisionLayer.Player)
            .HitLayer(CollisionLayer.NPC)
            .HitLayer(CollisionLayer.NPC_CLIP)
            .Run();
        if(tr.Entity != Target){
            return false;
        }
        if(JustHit){
            JustHit = false;
            return true;
        }
        if(ReactionTime > 0)
            return false;
        int d = (int)WorldSpaceBounds.Center.Distance(Target.WorldSpaceBounds.Center);
        d -= 64;
        if(!HasMelee){
            d -= 128;
        }
        // ARCHVILE, 896 DON'T ATTACK
        // REVENENAT DOESN'T ATTACK > 14*64
        // OTHERWISE, REVNANT HALF
        // CYBERDEMON, SPIDER, LOST SOUL HALF
        if(d > 200)d = 200;
        // CYBERDEMON IS 160 CAP
        if(d < 0) d = 0;
        if(DoomGame.RNG.Next()%256 < d)
            return false;
        return true;
        
    }

    public void NewChaseDir(){
        moveCount = DoomGame.RNG.Next()%16;
        if(Target != null && Target.IsValid && Target.Health > 0){
            moveDir = (int)((Rotation.LookAt(Target.WorldSpaceBounds.Center - WorldSpaceBounds.Center).Angles().yaw+25.5f)/45);
            if(moveDir >= 8)moveDir -= 8;
            if(moveDir < 0)moveDir += 8;
            if(TryMove().can)
                return;
        }
        moveDir = DoomGame.RNG.Next()%8;
    }

    public (bool can, Vector3 pos) TryMove(){
        var moveOff = Rotation.FromYaw(moveDir * 45) * Vector3.Forward * MoveSpeed;
        var tr = Trace.Box(CollisionBounds, Position, Position + moveOff)
            .HitLayer(CollisionLayer.NPC_CLIP)
            .HitLayer(CollisionLayer.NPC)
            .Ignore(this)
            .Run();
        // Step down?
        if(tr.Fraction >= 1){
            tr = Trace.Box(CollisionBounds, Position + moveOff, Position + new Vector3(0,0,-24.1f)*WadLoader.mapScale + moveOff)
                .HitLayer(CollisionLayer.NPC_CLIP)
                .HitLayer(CollisionLayer.NPC)
                .Ignore(this)
                .Run();
            return (tr.Fraction < 1, tr.EndPosition);
        }
        // Step UP.
        var shortMan = new BBox(
            CollisionBounds.Mins,
            CollisionBounds.Maxs.WithZ(CollisionBounds.Maxs.z - 24*WadLoader.mapScale.z)
        );
        tr = Trace.Box(shortMan, Position + new Vector3(0,0,24)*WadLoader.mapScale, Position + new Vector3(0,0,24)*WadLoader.mapScale + moveOff)
            .HitLayer(CollisionLayer.NPC_CLIP)
            .HitLayer(CollisionLayer.NPC)
            .Ignore(this)
            .Run();
        if(tr.Fraction < 1)return (false, tr.EndPosition);
        tr = Trace.Box(shortMan, Position + new Vector3(0,0,24)*WadLoader.mapScale + moveOff, Position + new Vector3(0,0,-24.1f)*WadLoader.mapScale + moveOff)
            .HitLayer(CollisionLayer.NPC_CLIP)
            .HitLayer(CollisionLayer.NPC)
            .Ignore(this)
            .Run();
        return (tr.Fraction < 1, tr.EndPosition);
    }
    public bool Move(){
        (var can, var pos) = TryMove();
        if(can)
            Position = pos;
        return can;
    }

    #region StateHandlers
    public virtual IEnumerator StateSpawn() {
        if(LookForTargets())yield break;
        animationSteps = "A";
        animationTime = 10;
        yield return null;
        if(LookForTargets())yield break;
        animationSteps = "B";
        animationTime = 10;
    }
    public virtual IEnumerator StateSee() {
        Chase();
        animationSteps = "A";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "A";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "B";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "B";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "C";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "C";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "D";
        animationTime = 3;
        yield return null;
        Chase();
        animationSteps = "D";
        animationTime = 3;
    }

    public virtual IEnumerator StateMelee() {
        yield return null;
    }

    public virtual IEnumerator StateMissile() {
        yield return null;
    }

    public virtual IEnumerator StatePain() {
        animationSteps = "G";
        animationTime = 3;
        yield return null;
        animationSteps = "G";
        animationTime = 3;
        if(PainSound != null){
            SoundLoader.PlaySound(PainSound, Position);
        }
        yield return null;
        currentState = MonsterState.See;
    }

    public virtual IEnumerator StateDeath() {
        animationSteps = "H";
        animationTime = 5;
        hasFacing = false;
        yield return null;
        animationSteps = "I";
        animationTime = 5;
        if(DeathSound.Length > 0){
            SoundLoader.PlaySound(DeathSound[DoomGame.RNG.Next()%DeathSound.Length], Position);
        }
        yield return null;
        animationSteps = "J";
        animationTime = 5;
        // No collide.
        CollisionGroup = CollisionGroup.Debris;
        SetInteractsAs( CollisionLayer.Debris );
        yield return null;
        OnDeath(murderAttack);
        animationSteps = "K";
        animationTime = 5;
        yield return null;
        frameName = 'L';
        currentState = MonsterState.Dead;
        SetState(MonsterState.Dead);
        animationTime = 9000;
    }

    public virtual IEnumerator StateXDeath() {
        animationSteps = "M";
        animationTime = 5;
        hasFacing = false;
        yield return null;
        animationSteps = "N";
        animationTime = 5;
        // SCREAM
        yield return null;
        animationSteps = "O";
        animationTime = 5;
        // No collide.
        CollisionGroup = CollisionGroup.Debris;
        SetInteractsAs( CollisionLayer.Debris );
        yield return null;
        OnDeath(murderAttack);
        animationSteps = "PQRST";
        animationTime = 5;
        yield return null;
        frameName = 'U';
        currentState = MonsterState.Dead;
        SetState(MonsterState.Dead);
        animationTime = 9000;
    }

    public virtual IEnumerator StateRaise() {
        yield return null;
    }

    public virtual IEnumerator StateDead(){
        yield return null;
    }
    #endregion

    public virtual string Facing(){
        if(Local.Pawn is not DoomPlayer ply)return "1";
        if(!ply.IsValid || !IsValid)return "1";
        var ang = Rotation.LookAt(ply.Position - Position).Angles().yaw - Rotation.Angles().yaw;
        while(ang<-180)ang+=360;
        while(ang>180)ang-=360;
        FlipUV = ang<=-22.5;
        int angIndex = (int)Math.Floor(Math.Abs(ang)/45f + 0.5f);
        if(angIndex == 0)return "1";
        if(angIndex == 4)return "5";
        return $"{angIndex+1}{frameName}{9-angIndex}";
    }

    public override void Spawn(){
        base.Spawn();
        CollisionGroup = CollisionGroup.Default;
        ClearCollisionLayers();
        AddCollisionLayer(CollisionLayer.NPC);
        AddCollisionLayer(CollisionLayer.Hitbox);
        AddCollisionLayer(CollisionLayer.Debris);
        SetInteractsWith(CollisionLayer.NPC_CLIP);

        Health = MaxHealth;
        currentState = MonsterState.Spawn;
    }

    public override void TakeDamage( DamageInfo info )
	{
        ReactionTime = 0;
        if(Health <= 0)return;
		Velocity += info.Force;
        if(DoomGame.RNG.Next()%256 <= PainChance){
            if((Target == null && info.Attacker is Monster) || info.Attacker is DoomPlayer){
                Target = info.Attacker;
                JustHit = true;
            }
            SetState(MonsterState.Pain);
        }
        Health -= info.Damage;
        if(Health <= 0){
            murderAttack = info;
            SetState(MonsterState.Death);
        }
	}

    #region ShootBullets
    public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f ){
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
	private static Surface FleshSurface = ResourceLibrary.Get<Surface>("surfaces/flesh.surface");
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
			FleshSurface ??= ResourceLibrary.Get<Surface>("surfaces/flesh.surface");
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
					.WithAttacker( this )
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
		var pos = WorldSpaceBounds.Center;
        var dir = (Target.WorldSpaceBounds.Center - pos).Normal;

		for ( int i = 0; i < numBullets; i++ )
		{
			ShootBullet( pos, dir, spread, force / numBullets, damage, bulletSize );
		}
	}
    #endregion

    public virtual void OnDeath(DamageInfo hit){}

    public enum MonsterState{
        Spawn,
        See,
        Missile,
        Melee,
        Pain,
        Death,
        XDeath,
        Raise,
        Dead
    }
}