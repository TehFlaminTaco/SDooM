using System.Linq;
using Sandbox;

public class RocketLauncherPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "LAUNA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        bool hasWep = (ply.Inventory as Inventory).All().OfType<DoomRocketLauncher>().Any();
        if(ply.AmmoCount(AmmoType.Rocket)>=ply.AmmoMax(AmmoType.Rocket) && hasWep)return;
        if(ply==Local.Pawn)StatusText.AddChatEntry("","Got the rocket launcher!");
        if(Host.IsServer){
            if(!hasWep){
                var sgun = new DoomRocketLauncher();
                ply.Inventory.Add(sgun);
                ply.ActiveChild = sgun;
                SoundLoader.PlaySound("DSWPNUP", Position);
                Face.FlashFaceServer(To.Single(ply), "EVL", 2f, 8);
            }else{
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            ply.AddAmmo(AmmoType.Rocket, 2);
        }
        
        if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}