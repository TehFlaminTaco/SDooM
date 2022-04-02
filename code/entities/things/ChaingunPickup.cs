using System.Linq;
using Sandbox;

public class ChaingunPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "MGUNA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        bool hasWep = (ply.Inventory as Inventory).All().OfType<Chaingun>().Any();
        if(ply.AmmoCount(AmmoType.Bullet)>=ply.AmmoMax(AmmoType.Bullet) && hasWep)return;
        if(ply==Local.Pawn)StatusText.AddChatEntry("","Got the chaingun!");
        if(Host.IsServer){
            if(!(ply.Inventory as Inventory).All().OfType<Chaingun>().Any()){
                var sgun = new Chaingun();
                ply.Inventory.Add(sgun);
                ply.ActiveChild = sgun;
                SoundLoader.PlaySound("DSWPNUP", Position);
                Face.FlashFaceServer(To.Single(ply), "EVL", 2f, 8);
            }else{
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            ply.AddAmmo(AmmoType.Bullet, 20);
        }
        
        ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}