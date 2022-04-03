using Sandbox;

public partial class HealthBonus : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "BON1A0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Health < 200){
            if(ply==Local.Pawn)StatusText.AddChatEntry("","Picked up a health bonus.");
            ply.Health++;
            SoundLoader.PlaySound("DSITEMUP", Position);
            if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}