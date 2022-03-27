using Sandbox;

public partial class Medkit : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "MEDIA0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Health < 100){
            if(ply==Local.Pawn)StatusText.AddChatEntry("",ply.Health<25 ? "Picked up a medkit that you REALLY need!" : "Picked up a medkit.");
            ply.Health+=25;
            if(ply.Health > 100)ply.Health = 100;
            SoundLoader.PlaySound("DSITEMUP", Position);
            ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}