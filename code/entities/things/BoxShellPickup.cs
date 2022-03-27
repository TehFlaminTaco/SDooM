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
        if(Host.IsServer){
            ply.shellAmmo += 20;
        }
        
        ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}