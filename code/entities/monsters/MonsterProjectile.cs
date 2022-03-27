using Sandbox;
public class MonsterProjectile : ThingEntity {
    public int Damage;
    public string LoopSequence = "AB";
    public string DeathSequence = "CDE";
    public string ImpactSound = null;
    public int LoopFrames = 0;
    public int DeathFrames = 0;
    public int Speed = 10;
    public Entity Attacker;

    public override bool SpriteCentered => true;
    public override float Height => 8;
    public override float Radius => 3;

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SetupPhysicsFromAABB(PhysicsMotionType.Static, new Vector3(-Radius, -Radius, 0) * WadLoader.mapScale, new Vector3(Radius, Radius, Height) * WadLoader.mapScale);
            CollisionGroup = CollisionGroup.Weapon;
            SetInteractsAs( CollisionLayer.Trigger );
            SetInteractsExclude( CollisionLayer.Debris );
        }
    }
    public string spriteBase = "BAL1";
    public string sequence = "AB"; 
    public int spriteIndex = 0;
    public TimeSince lastFrame = 0;
    public TimeSince lastTick = 0;
    [Event.Tick.Server]
    public void AnimationAndExplosion(){
        if(lastTick > 0 && sequence != DeathSequence){
            lastTick = -1f/35f;
            var tr = Trace.Box(CollisionBounds, Position, Position + Velocity)
                .HitLayer(CollisionLayer.Player)
                .HitLayer(CollisionLayer.NPC)
                .HitLayer(CollisionLayer.NPC_CLIP)
                .Ignore(this)
                .Ignore(Attacker)
            .Run();
            if(tr.Hit){
                if(ImpactSound != null){
                    SoundLoader.PlaySound(ImpactSound, Position);
                }
                if(sequence != DeathSequence){
                    sequence = DeathSequence;
                    spriteIndex = 0;
                    tr.Entity.TakeDamage(DamageInfo.Generic(Damage).WithAttacker(Attacker));
                }
            }else{
                Position = tr.EndPosition;
            }
        }
        if(lastFrame > 0){
            lastFrame = -(sequence == DeathSequence ? DeathFrames : LoopFrames)/35f;
            SpriteName = spriteBase + sequence[spriteIndex++] + "0";
            if(spriteIndex >= sequence.Length){
                if(sequence == DeathSequence)
                    DeleteAsync(0f);
                spriteIndex = 0;
            }
        }
    }
}