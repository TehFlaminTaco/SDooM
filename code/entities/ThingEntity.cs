using System.Collections.Generic;
using Sandbox;

public partial class ThingEntity : Prop {

    public ThingEntity() {
        
    }
    public BillboardSprite sprite;
    public virtual float Radius => 20f;
    public virtual float Height => 16f;
    [Net] public bool FullBright {get;set;}
    public virtual bool SpriteCentered => false;

    public bool FlipUV = false;

    [Net, Predicted] public string SpriteName {get;set;}
    [Event.Frame]
    public void OnFrame(){
        if(sprite==null || !sprite.IsValid){
            sprite = new();
            sprite.Parent = this;
            sprite.LocalPosition = Vector3.Zero;
        }
        sprite.SpriteCentered = SpriteCentered;
        sprite.FullBright = FullBright;
        sprite.SetSprite(SpriteName, FlipUV);
    }

    [Event.Tick.Server]
    public void KeepUpright(){
        Rotation = Rotation.FromYaw(Rotation.Angles().yaw);
    }

    public override void Spawn(){
        base.Spawn();
        SetupPhysicsFromAABB(PhysicsMotionType.Dynamic, new Vector3(-Radius, -Radius, SpriteCentered?-Height/2:0) * WadLoader.mapScale, new Vector3(Radius, Radius, SpriteCentered?Height/2:Height) * WadLoader.mapScale);
        CollisionGroup = CollisionGroup.Weapon;
        SetInteractsAs( CollisionLayer.Trigger );
        SetInteractsExclude( CollisionLayer.Debris );
    }

    public virtual void OnTouched(DoomPlayer ply){}
}

public class BillboardSprite : ModelEntity{

    public BillboardSprite() {

    }

    [Event.Frame]
    public void UpdateRotation(){
        if(!IsValid) return;
        if(Local.Pawn is not DoomPlayer ply)return;
        var camPos = ply.CameraPosition().position;
        Rotation = Rotation.FromYaw(Rotation.LookAt(camPos - Position).Angles().yaw + 90);
        var sector = DoomMap.GetSector(Position);
		if(sector!=null)RenderColor = FullBright?Color.White:new Color(sector.brightness,sector.brightness,sector.brightness,1);
    }

    string lastSprite = "";
    bool lastFlip = false;
    public bool FullBright = false;
    public bool SpriteCentered = false;
    public void SetSprite(string newSprite, bool newFlip){
        if(lastSprite == newSprite && lastFlip == newFlip)return;
        lastSprite = newSprite;
        lastFlip = newFlip;
        if(newSprite==null)return;

        Texture spriteTex = TextureLoader2.Instance.GetSpriteTexture(newSprite);
        if(spriteTex == null)return;


        Mesh mesh = new Mesh();
        List<VoxelVertex> vertices = new();

		Material mat;
        if (!MaterialManager.Instance.OverridesWall(newSprite, out var mr))
            if (TextureLoader2.NeedsAlphacut.ContainsKey(newSprite))
                mat = MaterialManager.Instance.alphacutMaterial;
            else
                mat = MaterialManager.Instance.defaultMaterial;
		else
			mat=mr;
		
		if(mat == null){
			mat = Material.Load("materials/pixelperfect.vmat").CreateCopy();
			mat.OverrideTexture("Color", spriteTex);
			TextureAnimator.TryGenerateAnimator(this, mat, newSprite, TextureAnimator.Mode.SPRITE, 6);
		}
		mesh.Material = mat;

        Vector3 p0 = new Vector3(-spriteTex.Width/2, 0, SpriteCentered?-spriteTex.Height/2:0)  * WadLoader.mapScale;
        Vector3 p1 = new Vector3(spriteTex.Width/2, 0, SpriteCentered?-spriteTex.Height/2:0)   * WadLoader.mapScale;
        Vector3 p2 = new Vector3(-spriteTex.Width/2, 0, SpriteCentered?spriteTex.Height/2:spriteTex.Height)   * WadLoader.mapScale;
        Vector3 p3 = new Vector3(spriteTex.Width/2, 0, SpriteCentered?spriteTex.Height/2:spriteTex.Height)    * WadLoader.mapScale;


        int uvRIGHT = newFlip ? 0 : 1;
        int uvLEFT = newFlip ? 1 : 0;
        vertices.Add(new VoxelVertex(p0, new Vector3(0,1,0), new Vector3(1,0,0), new Vector2(uvLEFT,0)));
        vertices.Add(new VoxelVertex(p1, new Vector3(0,1,0), new Vector3(1,0,0), new Vector2(uvRIGHT,0)));
        vertices.Add(new VoxelVertex(p2, new Vector3(0,1,0), new Vector3(1,0,0), new Vector2(uvLEFT,1)));
        vertices.Add(new VoxelVertex(p2, new Vector3(0,1,0), new Vector3(1,0,0), new Vector2(uvLEFT,1)));
        vertices.Add(new VoxelVertex(p1, new Vector3(0,1,0), new Vector3(1,0,0), new Vector2(uvRIGHT,0)));
        vertices.Add(new VoxelVertex(p3, new Vector3(0,1,0), new Vector3(1,0,0), new Vector2(uvRIGHT,1)));
        
        if(mesh.HasVertexBuffer){
			mesh.SetVertexBufferSize(vertices.Count);
			mesh.SetVertexBufferData(vertices);
		}else{
			mesh.CreateVertexBuffer(vertices.Count, Helpers.MeshLayout, vertices);
		}

        var mb = new ModelBuilder();
        mb.AddMesh(mesh);
        Model = mb.Create();
    }

}