public class ObstacleThing : ThingEntity {
    public Decoration decoration;
    public override float Height => decoration.Height;
    public override float Radius => decoration.Width;
    public override void Spawn() {
        base.Spawn();
    }
}