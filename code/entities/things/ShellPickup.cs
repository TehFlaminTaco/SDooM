using System.Linq;
using Sandbox;

public class ShellPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "SHELA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        if(Host.IsServer){
            ply.shellAmmo += 4;
        }
        
        ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}