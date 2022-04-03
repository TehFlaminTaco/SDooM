using System.Linq;
using Sandbox;

public class RocketProjectile : MonsterProjectile {
    public override float Radius => 11;
    public override void Spawn(){
        LoopSequence = "A";
        sequence = "A";
        DeathSequence = "BCD";
        ImpactSound = "DSBAREXP";
        spriteBase = "MISL";
        Speed = 20;
        Damage = 20;
        ForceFacing = true;
        base.Spawn();
    }    
    public override void OnHit(TraceResult tr){
        // TODO: Boom.
        ForceFacing = false;
        foreach(var ent in All.Where(c=>c.Position.Distance(Position)<=128f).OfType<ModelEntity>()){
            var r = ent.Position.Distance(Position) - Radius;
            var tr2 = Trace.Ray(CollisionWorldSpaceCenter, ent.CollisionWorldSpaceCenter)
                .HitLayer(CollisionLayer.NPC_CLIP)
                .Ignore(this)
                .Ignore(ent)
            .Run();
            if(tr2.Hit)continue;
            var d = System.Math.Clamp(128f - r, 0f, 128f);
            ent.TakeDamage(DamageInfo.Generic(d)
                .WithForce((ent.Position-Position).Normal * d * 0.5f)
            );
        }
        frameTime = 6;
        base.OnHit(tr);
    }
}