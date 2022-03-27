using System.Linq;
using Sandbox;

public class ClipPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "CLIPA0";
        }
    }
    public int ammoHeld = 10;

    public override void OnTouched(DoomPlayer ply){
        //if(ply==Local.Pawn)StatusText.AddChatEntry("","Got the shotgun!");
        if(Host.IsServer){
            ply.bulletAmmo += ammoHeld;
            SoundLoader.PlaySound("DSITEMUP", Position);
        }
        
        ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}