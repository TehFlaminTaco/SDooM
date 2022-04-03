using System.Linq;
using Sandbox;

public class ChainsawPickup : ThingEntity {
    public override void Spawn(){
        base.Spawn();
        if(Host.IsServer){
            SpriteName = "CSAWA0";
        }
    }
    public override void OnTouched(DoomPlayer ply){
        bool hasWep = (ply.Inventory as Inventory).All().OfType<DoomChainsaw>().Any();
        if(ply.AmmoCount(AmmoType.Bullet)>=ply.AmmoMax(AmmoType.Bullet) && hasWep)return;
        if(ply==Local.Pawn)StatusText.AddChatEntry("","A Chainsaw! Find some meat!");
        if(Host.IsServer){
            if(!hasWep){
                var sgun = new DoomChainsaw();
                ply.Inventory.Add(sgun);
                ply.ActiveChild = sgun;
                SoundLoader.PlaySound("DSWPNUP", Position);
                Face.FlashFaceServer(To.Single(ply), "EVL", 2f, 8);
            }else{
                SoundLoader.PlaySound("DSITEMUP", Position);
            }
            ply.AddAmmo(AmmoType.Bullet, 20);
        }
        
        if(Local.Pawn==ply)ItemPickupFlash.DoFlash();
        if(IsServer)Delete();
    }
}