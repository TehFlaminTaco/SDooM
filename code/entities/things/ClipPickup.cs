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
        if(ply.bulletAmmo < ply.AmmoMax(AmmoType.Bullet)){
            if(Host.IsServer){
                ply.AddAmmo(AmmoType.Bullet, ammoHeld);
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            
            ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
}