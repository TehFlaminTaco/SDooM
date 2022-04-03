using Sandbox;

public partial class DoomFist : DoomGun {
    public override bool UsesAmmo => false;
    public override int HoldSlot => 1;
    public override int pixelOffset => State==GUNSTATE_IDLE ? 90 : 0;
    public override void OnTick(){
        switch(State){
            case GUNSTATE_IDLE:{
                DoomHud.Instance.weaponSprite?.SetSprite("PUNGA0");
                if(CanPrimaryAttack()){
                    State = GUNSTATE_FIRE;
                    animationIndex = 1;
                    frameTicks = 4;
                    DoomHud.Instance.weaponSprite?.SetSprite("PUNGB0");
                }
                break;
            }

            case GUNSTATE_FIRE:{
                switch(animationIndex){
                    case 1:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PUNGC0");
                        frameTicks = 4;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                        }
                        var forward = Owner.EyeRotation.Forward;
                        var hit = false;
                        foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 32, 20.0f ) ){
                            if ( !tr.Entity.IsValid() ) continue;

                            tr.Surface.DoBulletImpact( tr );

                            hit = true;

                            if ( !IsServer ) continue;

                            using ( Prediction.Off() )
                            {
                                var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100, DoomGame.RNG.Next()%18+2)
                                    .UsingTraceResult( tr )
                                    .WithAttacker( Owner )
                                    .WithWeapon( this );

                                tr.Entity.TakeDamage( damageInfo );
                            }
                        }
                        if(hit && Host.IsServer){
                            SoundLoader.PlaySound("DSPUNCH", Owner.Position);
                        }
                        
                        break;
                    }
                    case 2:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PUNGD0");
                        frameTicks = 5;
                        break;
                    }
                    case 3:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PUNGC0");
                        frameTicks = 4;
                        break;
                    }
                    case 4:{
                        if(CanPrimaryAttack()){
                            State = GUNSTATE_FIRE;
                            animationIndex = 0;
                            frameTicks = 4;
                            DoomHud.Instance.weaponSprite?.SetSprite("PUNGB0");
                        }
                        DoomHud.Instance.weaponSprite?.SetSprite("PUNGB0");
                        frameTicks = 5;
                        break;
                    }
                    case 5:{
                        DoomHud.Instance.weaponSprite?.SetSprite("PUNGA0");
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