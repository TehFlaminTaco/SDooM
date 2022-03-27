using Sandbox;

public partial class SecurityArmor : ThingEntity {

    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "ARM1A0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        if(ply.Armor < 100){
            if(ply==Local.Pawn)StatusText.AddChatEntry("","Picked up the armor.");
            ply.Armor = 100;
            SoundLoader.PlaySound("DSITEMUP", Position);
            ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
    
}