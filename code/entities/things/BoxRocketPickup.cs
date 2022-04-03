using System.Linq;
using Sandbox;

public class BoxRocketPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "BROKA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        if(ply.shellAmmo < ply.AmmoMax(AmmoType.Rocket)){
            if(Host.IsServer){
                ply.AddAmmo(AmmoType.Rocket, 10);
            }
            
            if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
        
    }
}