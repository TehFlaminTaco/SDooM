using System.Collections.Generic;
using Sandbox;

partial class DoomPlayer{
    //[Net, Predicted]
    //protected Dictionary<AmmoType, int> heldAmmo {get; set;} = new();

    // When dictionary support is added for [Net], I'll change this. Hopefully.
    [Net]
    public int bulletAmmo {get; set;} = 0;
    [Net]
    public int shellAmmo {get; set;} = 0;
    [Net]
    public int rocketAmmo {get; set;} = 0;
    [Net]
    public int cellAmmo {get; set;} = 0;

    [Net] public bool HasBackpack {get;set;}
    public int AmmoMax(AmmoType t){
        switch(t){
            case AmmoType.Bullet:
                return HasBackpack?400:200;
            case AmmoType.Shell:
                return HasBackpack?100:50;
            case AmmoType.Rocket:
                return HasBackpack?100:50;
            case AmmoType.Cell:
                return HasBackpack?600:300;
            default:
                return 0;
        }
    }

    public int AmmoCount(AmmoType typ){
        switch(typ){
            case AmmoType.None:
                return 0;
            case AmmoType.Bullet:
                return bulletAmmo;
            case AmmoType.Shell:
                return shellAmmo;
            case AmmoType.Rocket:
                return rocketAmmo;
            case AmmoType.Cell:
                return cellAmmo;
        }
        return 0;
    }

    public int AddAmmo(AmmoType typ, int amount){
        switch(typ){
            case AmmoType.None:
                return 0;
            case AmmoType.Bullet:
                if(bulletAmmo + amount > AmmoMax(typ)){
                    int diff = AmmoMax(typ) - bulletAmmo;
                    bulletAmmo = AmmoMax(typ);
                    return diff;
                }
                bulletAmmo += amount;
                return amount;
            case AmmoType.Shell:
                if(shellAmmo + amount > AmmoMax(typ)){
                    int diff = AmmoMax(typ) - shellAmmo;
                    shellAmmo = AmmoMax(typ);
                    return diff;
                }
                shellAmmo += amount;
                return amount;
            case AmmoType.Rocket:
                if(rocketAmmo + amount > AmmoMax(typ)){
                    int diff = AmmoMax(typ) - rocketAmmo;
                    rocketAmmo = AmmoMax(typ);
                    return diff;
                }
                rocketAmmo += amount;
                return amount;
            case AmmoType.Cell:
                if(cellAmmo + amount > AmmoMax(typ)){
                    int diff = AmmoMax(typ) - cellAmmo;
                    cellAmmo = AmmoMax(typ);
                    return diff;
                }
                cellAmmo += amount;
                return amount;
        }
        return 0;
    }

    public int RemoveAmmo(AmmoType typ, int amount){
        switch(typ){
            case AmmoType.None:
                return 0;
            case AmmoType.Bullet:
                if(bulletAmmo >= amount){
                    bulletAmmo -= amount;
                    return amount;
                }
                var t = bulletAmmo;
                bulletAmmo = 0;
                return t;
            case AmmoType.Shell:
                if(shellAmmo >= amount){
                    shellAmmo -= amount;
                    return amount;
                }
                t = shellAmmo;
                shellAmmo = 0;
                return t;
            case AmmoType.Rocket:
                if(rocketAmmo >= amount){
                    rocketAmmo -= amount;
                    return amount;
                }
                t = rocketAmmo;
                rocketAmmo = 0;
                return t;
            case AmmoType.Cell:
                if(cellAmmo >= amount){
                    cellAmmo -= amount;
                    return amount;
                }
                t = cellAmmo;
                cellAmmo = 0;
                return t;
        }
        return 0;
    }

    public bool HasAmmo(AmmoType typ, int amount = 1){
        return AmmoCount(typ)>=amount;
    }
}

partial class Weapon {
    public virtual AmmoType Clip1Type => AmmoType.None;
    public virtual AmmoType Clip2Type => AmmoType.None;
    public virtual int Ammo1PerShot => 1;
    public virtual int Ammo2PerShot => 1;
    public virtual bool UsesAmmo => true;

    public override bool CanPrimaryAttack()
    {
        if ( !Owner.IsValid() || !Input.Down( InputButton.PrimaryAttack ) ) return false;
        if(UsesAmmo)if(!((Owner as DoomPlayer)?.HasAmmo(Clip1Type)??false)) return false;

        var rate = PrimaryRate;
        if ( rate <= 0 ) return true;

        return TimeSincePrimaryAttack > (1 / rate);
    }

    public override bool CanSecondaryAttack()
    {
        if ( !Owner.IsValid() || !Input.Down( InputButton.SecondaryAttack ) ) return false;
        if(UsesAmmo)if(!((Owner as DoomPlayer)?.HasAmmo(Clip2Type)??false)) return false;

        var rate = SecondaryRate;
        if ( rate <= 0 ) return true;

        return TimeSinceSecondaryAttack > (1 / rate);
    }
}