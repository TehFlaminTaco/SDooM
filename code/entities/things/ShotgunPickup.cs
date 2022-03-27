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
        if(ply==Local.Pawn)StatusText.AddChatEntry("","Got the shotgun!");
        if(Host.IsServer){
            if(!(ply.Inventory as Inventory).All().OfType<Shotgun>().Any()){
                var sgun = new Shotgun();
                ply.Inventory.Add(sgun);
                ply.ActiveChild = sgun;
                SoundLoader.PlaySound("DSWPNUP", Position);
                Face.FlashFaceServer(To.Single(ply), "EVL", 2f, 8);
            }else{
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            ply.shellAmmo += ammoHeld;
        }
        
        ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}