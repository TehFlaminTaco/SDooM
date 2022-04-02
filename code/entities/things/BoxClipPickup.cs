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
        if(ply.bulletAmmo < ply.AmmoMax(AmmoType.Bullet)){
            if(Host.IsServer){
                ply.AddAmmo(AmmoType.Bullet, 50);
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            
            ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
    }
}