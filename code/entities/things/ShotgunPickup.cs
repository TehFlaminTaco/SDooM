using System.Linq;
using Sandbox;

public class ShotgunPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "SHOTA0";
        }
    }
    public int ammoHeld = 8;
    public override void OnTouched(DoomPlayer ply){
        bool hasWep = (ply.Inventory as Inventory).All().OfType<DoomShotgun>().Any();
        if(ply.AmmoCount(AmmoType.Shell)>=ply.AmmoMax(AmmoType.Shell) && hasWep)return;
        if(ply==Local.Pawn)StatusText.AddChatEntry("","Got the shotgun!");
        if(Host.IsServer){
            if(!hasWep){
                var sgun = new DoomShotgun();
                ply.Inventory.Add(sgun);
                ply.ActiveChild = sgun;
                SoundLoader.PlaySound("DSWPNUP", Position);
                Face.FlashFaceServer(To.Single(ply), "EVL", 2f, 8);
            }else{
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            ply.AddAmmo(AmmoType.Shell, ammoHeld);
        }
        
        if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}