using Sandbox;

public partial class ArmorBonus : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "BON2A0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Armor < 200){
            if(ply==Local.Pawn)StatusText.AddChatEntry("","Picked up an armor bonus.");
            ply.Armor++;
            SoundLoader.PlaySound("DSITEMUP", Position);
            if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}