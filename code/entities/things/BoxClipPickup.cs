using System.Linq;
using Sandbox;

public class BoxClipPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "AMMOA0";
        }
    }

    public override void OnTouched(DoomPlayer ply){
        //if(ply==Local.Pawn)StatusText.AddChatEntry("","Got the shotgun!");
        if(Host.IsServer){
            ply.bulletAmmo += 50;
            SoundLoader.PlaySound("DSITEMUP", Position);
        }
        
        ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}