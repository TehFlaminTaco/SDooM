using Sandbox;

public partial class DoomPistol : DoomGun {
    public override AmmoType Clip1Type => AmmoType.Bullet;
    public override int HoldSlot => 2;
    public override int pixelOffset => 23;
    int shotCount = 0;
    public override void OnTick(){
        switch(State){
            case GUNSTATE_IDLE:{
                DoomHud.Instance.weaponSprite?.SetSprite("PISGA0");
                shotCount = 0;
                if(CanPrimaryAttack()){
                    State = GUNSTATE_FIRE;
                    animationIndex = 1;
                    frameTicks = 4;
                    DoomHud.Instance.weaponSprite?.SetSprite("PISGA0");
                    DoomHud.Instance.weaponSprite?.SetFlash("PISFA0", 14, 21);
                }
                break;
            }

            case GUNSTATE_FIRE:{
                switch(animationIndex){
                    case 1:{
                        DoomHud.Instance.weaponSprite?.SetFlash(null, 0, 0);
                        DoomHud.Instance.weaponSprite?.SetSprite("PISGB0");
                        frameTicks = 6;
                        if(Owner is not DoomPlayer ply)break;
                        if(ply.bulletAmmo <= 0)break;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                            SoundLoader.PlaySound("DSPISTOL", Owner.Position);
                        }
                        ShootBullet( new Vector2(shotCount==0 ? 0f : 5.6f, 0f), 1.5f, DoomGame.RNG.Next()%3*5, 3.0f );
                        shotCount = 1;
                        
                        break;
                    }
                    case 2:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PISGC0");
                        frameTicks = 4;
                        break;
                    }
                    case 3:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PISGB0");
                        frameTicks = 5;
                        if(CanPrimaryAttack()){
                            State = GUNSTATE_FIRE;
                            animationIndex = 1;
                            frameTicks = 4;
                            DoomHud.Instance.weaponSprite?.SetSprite("PISGA0");
                            DoomHud.Instance.weaponSprite?.SetFlash("PISFA0", 14, 21);
                            return;
                        }
                        break;
                    }
                    case 4:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PISGA0");
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