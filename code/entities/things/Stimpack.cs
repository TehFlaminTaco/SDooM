using Sandbox;

public partial class Stimpack : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "STIMA0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Health < 100){
            if(ply==Local.Pawn)StatusText.AddChatEntry("","Picked up a stimpack.");
            ply.Health+=10;
            if(ply.Health > 100)ply.Health = 100;
            SoundLoader.PlaySound("DSITEMUP", Position);
            ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}