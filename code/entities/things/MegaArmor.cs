using Sandbox;

public partial class MegaArmor : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "ARM2A0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Armor < 200){
            if(ply==Local.Pawn)StatusText.AddChatEntry("","Picked up the megaarmor!");
            ply.Armor = 200;
            ply.HasStrongArmor = true;
            SoundLoader.PlaySound("DSITEMUP", Position);
            if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}