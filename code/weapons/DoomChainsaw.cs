using Sandbox;
public class DoomChainsaw : DoomGun {
    public override bool UsesAmmo => false;
    public override int HoldSlot => 1;
    public override int pixelOffset => 64;
    public Sound? lastSound;

    public override void OnTick(){
        switch(State){
            case GUNSTATE_IDLE:{
                DoomHud.Instance.weaponSprite?.SetSprite(animationIndex == 0 ? "SAWGC0" : "SAWGD0");
                if(animationIndex == 0){
                    if(IsClient){
                        if(lastSound!=null)
                            lastSound.Value.Stop();
                        lastSound = SoundLoader.PlaySoundClientside("DSSAWIDL", Owner.Position).Item1;
                    }
                }
                animationIndex = animationIndex==0 ? 1 : 0;
                frameTicks = 4;
                if(CanPrimaryAttack()){
                    State = GUNSTATE_FIRE;
                    animationIndex = 1;
                    frameTicks = 4;
                }
                break;
            }

            case GUNSTATE_FIRE:{
                switch(animationIndex){
                    case 1:{
                        if(IsClient){
                            if(lastSound!=null)
                                lastSound.Value.Stop();
                            lastSound = SoundLoader.PlaySoundClientside("DSSAWFUL", Owner.Position).Item1;
                        }
                        DoomHud.Instance.weaponSprite?.SetSprite("SAWGA0");
                        frameTicks = 4;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                        }
                        var forward = Owner.EyeRotation.Forward;
                        foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 32, 20.0f ) ){
                            if ( !tr.Entity.IsValid() ) continue;

                            tr.Surface.DoBulletImpact( tr );

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
                        
                        break;
                    }
                    case 2:{
                        if(IsClient){
                            if(lastSound!=null)
                                lastSound.Value.Stop();
                            lastSound = SoundLoader.PlaySoundClientside("DSSAWFUL", Owner.Position).Item1;
                        }
                        DoomHud.Instance.weaponSprite?.SetSprite("SAWGB0");
                        frameTicks = 4;
                        if(Host.IsServer){
                            DoomMap.GetSector(Owner.Position)?.PropogateSound(Owner);
                        }
                        var forward = Owner.EyeRotation.Forward;
                        foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 32, 20.0f ) ){
                            if ( !tr.Entity.IsValid() ) continue;

                            tr.Surface.DoBulletImpact( tr );

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
                        if(CanPrimaryAttack()){
                            State = GUNSTATE_FIRE;
                            animationIndex = 0;
                            frameTicks = 4;
                        }
                        break;
                    }
                    case 3:{
                        DoomHud.Instance.weaponSprite?.SetSprite("SAWGC0");
                        frameTicks = 0;
                        animationIndex = 0;
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