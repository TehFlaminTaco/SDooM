using Sandbox;

public class ItemRespawnFlash : ThingEntity {
    public string spriteBase = "IFOG";
    public string sequence = "ABCDE"; 
    public int spriteIndex = 0;
    public TimeSince lastFrame = 0;
    public override bool SpriteCentered => true;
    [Event.Tick.Server]
    public void AnimationAndExplosion(){
        if(lastFrame > 0){
            lastFrame = -8f/35f;
            SpriteName = spriteBase + sequence[spriteIndex++] + "0";
            if(spriteIndex >= sequence.Length){
                DeleteAsync(0f);
                spriteIndex = 0;
            }
        }
    }

    public override void Spawn(){
        base.Spawn();
        SetupPhysicsFromAABB(PhysicsMotionType.Static, new Vector3(-Radius, -Radius, SpriteCentered?-Height/2:0) * WadLoader.mapScale, new Vector3(Radius, Radius, SpriteCentered?Height/2:Height) * WadLoader.mapScale);
        CollisionGroup = CollisionGroup.Never;
    }
}