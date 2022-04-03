using Sandbox;

public class Backpack : ThingEntity { 
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "BPAKA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        if(Host.IsServer){
            ply.HasBackpack = true;
            ply.AddAmmo(AmmoType.Bullet, 10);
            ply.AddAmmo(AmmoType.Shell, 4);
            ply.AddAmmo(AmmoType.Rocket, 1);
            ply.AddAmmo(AmmoType.Cell, 20);
        }
        if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
        if(ply==Local.Pawn)StatusText.AddChatEntry("","Picked up a backpack full of ammo!");
        if(IsServer)Delete();
        
    }
}