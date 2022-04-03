using Sandbox;

public partial class DoomChaingun : DoomGun {
    public override AmmoType Clip1Type => AmmoType.Bullet;
    public override int HoldSlot => 4;
    public override int pixelOffset => 56;
    public override int weaponDown => 47;
    int shotCount = 0;
    public override void OnTick(){
        switch(State){
            case GUNSTATE_IDLE:{
                DoomHud.Instance.weaponSprite?.SetSprite("CHGGA0");
                shotCount = 0;
                if(CanPrimaryAttack()){
                    State = GUNSTATE_FIRE;
                    animationIndex = 1;
                    frameTicks = 0;
                    DoomHud.Instance.weaponSprite?.SetSprite("CHGGA0");
                    DoomHud.Instance.weaponSprite?.SetFlash("CHGFA0",14,19);
                    OnTick();
                }
                break;
            }

            case GUNSTATE_FIRE:{
                switch(animationIndex){
                    case 1:{
                        DoomHud.Instance.weaponSprite?.SetSprite("CHGGA0");
                        DoomHud.Instance.weaponSprite?.SetFlash("CHGFA0",14,19);
                        frameTicks = 4;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                            SoundLoader.PlaySound("DSPISTOL", Owner.Position);
                        }
                        ShootBullet( new Vector2(shotCount==0?0.0f:5.6f,0f), 1.5f, DoomGame.RNG.Next()%3*5, 3.0f );
                        break;
                    }
                    case 2:{
                        DoomHud.Instance.weaponSprite?.SetSprite("CHGGB0");
                        DoomHud.Instance.weaponSprite?.SetFlash("CHGFB0",14,21);
                        frameTicks = 4;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                            SoundLoader.PlaySound("DSPISTOL", Owner.Position);
                        }
                        ShootBullet( new Vector2(shotCount==0?0.0f:5.6f,0f), 1.5f, DoomGame.RNG.Next()%3*5, 3.0f );
                        shotCount = 1;
                        break;
                    }
                    case 3:{
                        if(CanPrimaryAttack()){
                            State = GUNSTATE_FIRE;
                            animationIndex = 1;
                            frameTicks = 0;
                            DoomHud.Instance.weaponSprite?.SetSprite("CHGGA0");
                            DoomHud.Instance.weaponSprite?.SetFlash("CHGFA0",14,19);
                            OnTick();
                            return;
                        }else{
                            DoomHud.Instance.weaponSprite?.SetFlash(null,0,0);
                            DoomHud.Instance.weaponSprite?.SetSprite("CHGGA0");
                            frameTicks = 1;
                            State = GUNSTATE_IDLE;
                        }
                        break;
                    }
                }
                animationIndex++;
                break;
            }
        }
        
    }
}