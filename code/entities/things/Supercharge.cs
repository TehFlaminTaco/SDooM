using Sandbox;

public partial class Supercharge : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "MEDIA0";
        }
    }

    public virtual string spriteBase => "SOUL";
    public string sequence = "ABCDCB"; 
    public int spriteIndex = 0;
    public TimeSince lastFrame = 0;
    [Event.Tick.Server]
    public void AnimationAndExplosion(){
        if(lastFrame > 0){
            lastFrame = -8f/35f;
            SpriteName = spriteBase + sequence[spriteIndex++] + "0";
            if(spriteIndex >= sequence.Length){
                spriteIndex = 0;
            }
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Health < 200){
            if(ply==Local.Pawn)StatusText.AddChatEntry("","supercharge!");
            ply.Health += 100;
            if(ply.Health > 200)ply.Health = 200;
            SoundLoader.PlaySound("DSGETPOW", Position);
            if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}