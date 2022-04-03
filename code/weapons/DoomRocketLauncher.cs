public class DoomRocketLauncher : DoomGun {
    public override int pixelOffset => State == GUNSTATE_IDLE ? 43 : 51;
    public override int weaponDown => 32;
    public override int HoldSlot => 5;
    public override AmmoType Clip1Type => AmmoType.Rocket;

    public override void OnTick() {
        switch (State) {
            case GUNSTATE_IDLE: {
                DoomHud.Instance.weaponSprite?.SetSprite("MISGA0");
                if (CanPrimaryAttack()) {
                    State = GUNSTATE_FIRE;
                    animationIndex = 1;
                    frameTicks = 0;
                    DoomHud.Instance.weaponSprite?.SetSprite("MISGB0");
                    DoomHud.Instance.weaponSprite?.SetFlash("MISFA0", 24, 18);
                    OnTick();
                }
                break;
            }

            case GUNSTATE_FIRE: {
                switch(animationIndex){
                    case 1:{
                        frameTicks = 1;
                        break;
                    }
                    case 3:{
                        DoomHud.Instance.weaponSprite?.SetFlash("MISFB0", 14, 24);
                        break;
                    }
                    case 7:{
                        DoomHud.Instance.weaponSprite?.SetFlash("MISFC0", 5, 31);
                        break;
                    }
                    case 8:{
                        if(IsServer){
                            SoundLoader.PlaySound("DSRLAUNC", Owner.Position);
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                            var rocket = new RocketProjectile(){
                                Position = Owner.EyePosition,
                                Attacker = Owner
                            };
                            rocket.Rotation = Owner.EyeRotation;
                            rocket.Velocity += Owner.EyeRotation.Forward * 20f;

                        }
                        break;
                    }
                    case 11:{
                        DoomHud.Instance.weaponSprite?.SetFlash("MISFD0", 0, 44);
                        break;
                    }
                    case 15:{
                        DoomHud.Instance.weaponSprite?.SetFlash(null, 0, 0);
                        break;
                    }
                    case 20:{
                        if (CanPrimaryAttack()) {
                            State = GUNSTATE_FIRE;
                            animationIndex = 1;
                            frameTicks = 0;
                            DoomHud.Instance.weaponSprite?.SetSprite("MISGB0");
                            DoomHud.Instance.weaponSprite?.SetFlash("MISFA0", 24, 18);
                            OnTick();
                            return;
                        }else{
                            DoomHud.Instance.weaponSprite?.SetSprite("MISGA0");
                            State = GUNSTATE_IDLE;
                            break;
                        }
                    }
                }
                animationIndex++;
                break;
            }
        }
    }    
}    