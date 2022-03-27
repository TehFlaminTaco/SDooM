using System.Collections;
using Sandbox;

public class Imp : Monster {
    public override float Height => 56;
    public override float Radius => 20;
    public override int MoveSpeed => 10;
    public override string SpriteRoot => "TROO";

    public override int MaxHealth => 60;
    public override int PainChance => 200;
    public override string PainSound => "DSPOPAIN";
    public override string[] DeathSound => new[]{"DSBGDTH1", "DSBGDTH2"};
    public override string[] SeeSound => new[]{"DSBGSIT1", "DSBGSIT2"};
    public override string[] ActiveSound => new[]{"DSBGACT"};

    public override bool HasMissile => true;
    public override bool HasMelee => true;

    public override IEnumerator StateMissile() {
        animationSteps = "E";
        animationTime = 8;
        yield return null;
        FaceTarget();
        animationSteps = "F";
        animationTime = 8;
        yield return null;
        FaceTarget();
        animationSteps = "G";
        animationTime = 6;
        if(Target != null && Target.IsValid){
            if(Target.WorldSpaceBounds.Center.Distance(WorldSpaceBounds.Center) <= 64f){
                SoundLoader.PlaySound("DSCLAW", Position);
                ShootBullets(1, new Vector2(0, 0), 10, 3+DoomGame.RNG.Next()%21, 2f);
            }else{
                SoundLoader.PlaySound("DSFIRSHT", Position);
                //ShootBullets(3, new Vector2(22, 0), 10, 5+DoomGame.RNG.Next()%10, 0.5f);
                var dir = (Target.WorldSpaceBounds.Center - WorldSpaceBounds.Center).Normal;
                var ball = new MonsterProjectile(){
                    Position = WorldSpaceBounds.Center,
                    Attacker = this
                };
                ball.Velocity += dir * 20;
                ball.spriteBase = "BAL1";
                ball.Damage = 3+DoomGame.RNG.Next()%21;
                ball.FullBright = true;
                ball.ImpactSound = "DSFIRXPL";
            }
        }
        yield return null;
        SetState(MonsterState.See);
    }
    public override IEnumerator StateMelee(){
        return StateMissile();
    }

    public override IEnumerator StateDeath() {
        animationSteps = "I";
        animationTime = 8;
        hasFacing = false;
        yield return null;
        animationSteps = "J";
        animationTime = 8;
        if(DeathSound.Length > 0){
            SoundLoader.PlaySound(DeathSound[DoomGame.RNG.Next()%DeathSound.Length], Position);
        }
        yield return null;
        animationSteps = "K";
        animationTime = 6;
        yield return null;
        animationSteps = "L";
        animationTime = 6;
        // No collide.
        CollisionGroup = CollisionGroup.Debris;
        SetInteractsAs( CollisionLayer.Debris );
        yield return null;
        OnDeath(murderAttack);
        frameName = 'M';
        currentState = MonsterState.Dead;
        SetState(MonsterState.Dead);
    }
}