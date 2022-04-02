using System.Collections;
using Sandbox;

public class ZombieMan : Monster {
    public override float Height => 56;
    public override float Radius => 20;
    public override string SpriteRoot => "POSS";

    public override int MaxHealth => 30;
    public override int PainChance => 200;
    public override string PainSound => "DSPOPAIN";
    public override string[] DeathSound => new[]{"DSPODTH1", "DSPODTH2", "DSPODTH3"};
    public override string[] SeeSound => new[]{"DSPOSIT1", "DSPOSIT2", "DSPOSIT3"};
    public override string[] ActiveSound => new[]{"DSPOSACT"};

    public override bool HasMissile => true;

    public override string GetObituary( DoomPlayer victim, Entity murderWeapon ){
        return $"{victim.Client?.Name??"Doomguy"} was killed by a Zombieman.";
    }

    public override void OnDeath(DamageInfo hit){
        if(IsServer){
            var clip = new ClipPickup(){
                Position = Position + Height/2
            };
            clip.Velocity = hit.Force;
            clip.ammoHeld = 5;
        }
    }

    public override IEnumerator StateMissile() {
        animationSteps = "E";
        animationTime = 10;
        yield return null;
        FaceTarget();
        animationSteps = "F";
        animationTime = 8;
        SoundLoader.PlaySound("DSPISTOL", Position);
        if(Target != null && Target.IsValid)
            ShootBullets(1, new Vector2(22, 0), 10, 3+DoomGame.RNG.Next()%12, 0.5f);
        yield return null;
        animationSteps = "E";
        animationTime = 8;
        yield return null;
        SetState(MonsterState.See);
    }
}