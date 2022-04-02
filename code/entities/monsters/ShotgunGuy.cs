using System.Collections;
using Sandbox;

public class ShotgunGuy : Monster {
    public override float Height => 56;
    public override float Radius => 20;
    public override string SpriteRoot => "SPOS";

    public override int MaxHealth => 30;
    public override int PainChance => 170;
    public override string PainSound => "DSPOPAIN";
    public override string[] DeathSound => new[]{"DSPODTH1", "DSPODTH2", "DSPODTH3"};
    public override string[] SeeSound => new[]{"DSPOSIT1", "DSPOSIT2", "DSPOSIT3"};
    public override string[] ActiveSound => new[]{"DSPOSACT"};

    public override bool HasMissile => true;

    public override string GetObituary( DoomPlayer victim, Entity murderWeapon ){
        return $"{victim.Client?.Name??"Doomguy"} was shot by a Sergeant.";
    }

    public override void OnDeath(DamageInfo hit){
        if(IsServer){
            var gun = new ShotgunPickup(){
                Position = Position + Height/2,
                ammoHeld = 4
            };
            gun.Velocity = hit.Force;
            
        }
    }

    public override IEnumerator StateMissile() {
        animationSteps = "E";
        animationTime = 10;
        yield return null;
        FaceTarget();
        animationSteps = "F";
        animationTime = 10;
        FullBright = true;
        SoundLoader.PlaySound("DSSHOTGN", Position);
        if(Target != null && Target.IsValid)
            ShootBullets(3, new Vector2(22, 0), 10, 5+DoomGame.RNG.Next()%10, 0.5f);
        yield return null;
        animationSteps = "E";
        animationTime = 10;
        FullBright = false;
        yield return null;
        SetState(MonsterState.See);
    }
}