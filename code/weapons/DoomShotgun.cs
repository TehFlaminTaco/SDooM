using Sandbox;

public partial class DoomShotgun : DoomGun {
    public override AmmoType Clip1Type => AmmoType.Shell;
    public override int HoldSlot => 3;
    public override int pixelOffset => 37;
    public override void OnTick(){
        switch(State){
            case GUNSTATE_IDLE:{
                DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                if(CanPrimaryAttack()){
                    State = GUNSTATE_FIRE;
                    animationIndex = 1;
                    frameTicks = 3;
                    DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                    DoomHud.Instance.weaponSprite?.SetFlash("SHTFA0",20,13);
                }
                break;
            }

            case GUNSTATE_FIRE:{
                switch(animationIndex){
                    case 1:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                        DoomHud.Instance.weaponSprite?.SetFlash("SHTFB0",15,22);
                        frameTicks = 7;
                        if(Owner is not DoomPlayer ply)break;
                        if(ply.shellAmmo <= 0)break;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                            SoundLoader.PlaySound("DSSHOTGN", Owner.Position);
                        }
                        for(int i=0;i<7;i++)
                            ShootBullet( new Vector2(5.6f,0f), 1.5f, DoomGame.RNG.Next()%3*5, 3.0f );
                        
                        break;
                    }
                    case 2:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGB0");
                        DoomHud.Instance.weaponSprite?.SetFlash(null,15,13);
                        frameTicks = 5;
                        break;
                    }
                    case 3:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGC0");
                        frameTicks = 5;
                        break;
                    }
                    case 4:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGD0");
                        frameTicks = 4;
                        break;
                    }
                    case 6:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGC0");
                        frameTicks = 5;
                        break;
                    }
                    case 7:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGB0");
                        frameTicks = 5;
                        break;
                    }
                    case 8:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                        frameTicks = 3;
                        break;
                    }
                    case 9:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                        frameTicks = 7;
                        if(CanPrimaryAttack()){
                            State = GUNSTATE_FIRE;
                            animationIndex = 1;
                            frameTicks = 3;
                            DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                            DoomHud.Instance.weaponSprite?.SetFlash("SHTFA0",20,13);
                            return;
                        }
                        break;
                    }
                    case 10:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SHTGA0");
                        frameTicks = 1;
                        State = GUNSTATE_IDLE;
                        break;
                    }
                }
                animationIndex++;
                break;
            }
        }
        
    }
}