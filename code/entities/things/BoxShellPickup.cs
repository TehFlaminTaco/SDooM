using System.Linq;
using Sandbox;

public class BoxShellPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "SBOXA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        if(ply.shellAmmo < ply.AmmoMax(AmmoType.Shell)){
            if(Host.IsServer){
                ply.AddAmmo(AmmoType.Shell, 20);
            }
            
            if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
            if(IsServer)Delete();
        }
        
    }
}