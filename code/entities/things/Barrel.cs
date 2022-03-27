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
    public TimeSince lastFrame = 0;
    [Event.Tick.Server]
    public void AnimationAndExplosion(){
        if(lastFrame > 0){
            lastFrame = -8f/35f;
            SpriteName = spriteBase + sequence[spriteIndex++] + "0";
            if(spriteBase == "BEXP"){
                if(spriteIndex == 5){
                    // EXPLODE
                    
                    var bomb = new Prop(){
                        Position = Position
                    };
                    bomb.SetModel( "models/rust_props/barrels/fuel_barrel.vmdl" );
					_ = bomb.ExplodeAsync( 0f );
                    DeleteAsync(0f);
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
        }
	}
}