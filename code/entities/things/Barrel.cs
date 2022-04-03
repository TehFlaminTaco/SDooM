using System.Linq;
using Sandbox;

public class Barrel : ThingEntity { 
    public override float Height => 42;
    public override float Radius => 10;

    public override void Spawn(){
        base.Spawn();
        Health = 20;
        SpriteName = "BAR1A0";
        AddCollisionLayer(CollisionLayer.NPC);
        AddCollisionLayer(CollisionLayer.Hitbox);
        AddCollisionLayer(CollisionLayer.Debris);
        SetInteractsWith(CollisionLayer.NPC_CLIP);
    }

    public string spriteBase = "BAR1";
    public string sequence = "AB"; 
    public int spriteIndex = 0;
    public int frameTime = 6;
    public TimeSince lastFrame = 0;
    [Event.Tick.Server]
    public void AnimationAndExplosion(){
        if(lastFrame > 0){
            lastFrame = -frameTime/35f;
            SpriteName = spriteBase + sequence[spriteIndex++] + "0";
            if(spriteBase == "BEXP"){
                switch(spriteIndex){
                    case 0:{
                        frameTime = 5;
                        break;
                    }
                    case 3:{
                        if(Host.IsServer)SoundLoader.PlaySound("DSBAREXP", Position);
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
                        break;
                    }
                    case 4:{
                        frameTime = 10;
                        break;
                    }
                    case 5:{
                        DeleteAsync(0f);
                        break;
                    }
                }
            }
            if(spriteIndex >= sequence.Length){
                spriteIndex = 0;
            }
        }
    }

    public override void TakeDamage( DamageInfo info )
	{
        if(Health <= 0)return;
		Velocity += info.Force;
        Health -= info.Damage;
        if(Health <= 0){
            spriteBase = "BEXP";
            spriteIndex = 0;
            sequence = "ABCDE";
            frameTime = 5;
            lastFrame = -frameTime/35f;
        }
	}
}