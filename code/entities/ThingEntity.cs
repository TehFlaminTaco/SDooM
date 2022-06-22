using System;
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
    [Net] public bool ForceFacing {get;set;}
    [Event.Frame]
    public void OnFrame(){
        if(DrawBillboard() && SpriteName != null){
            if(sprite==null || !sprite.IsValid){
                sprite = new();
                sprite.Parent = this;
                sprite.LocalPosition = Vector3.Zero;
            }
            sprite.SpriteCentered = SpriteCentered;
            sprite.FullBright = FullBright;
            if(ForceFacing){
                char frameName = SpriteName[^2];
                sprite.SetSprite(SpriteName[0..(SpriteName.Length-1)] + Facing(frameName), FlipUV);
            }else{
                sprite.SetSprite(SpriteName, FlipUV);
            }
        }else{
            // Destroy billboard if it exists
            if(sprite!=null && sprite.IsValid){
                sprite.Delete();
                sprite = null;
            }
        }
    }
    public string Facing(char frameName){
        if(Local.Pawn is not DoomPlayer ply)return "1";
        if(!ply.IsValid || !IsValid)return "1";
        var ang = Rotation.LookAt(ply.Position - Position).Angles().yaw - Rotation.Angles().yaw;
        while(ang<-180)ang+=360;
        while(ang>180)ang-=360;
        ang = -ang;
        FlipUV = ang<=-22.5;
        int angIndex = (int)Math.Floor(Math.Abs(ang)/45f + 0.5f);
        if(angIndex == 0)return "1";
        if(angIndex == 4)return "5";
        return $"{9-angIndex}{frameName}{angIndex+1}";
    }

	public virtual string GetObituary( DoomPlayer victim, Entity murderWeapon ){
        return $"{victim.Client?.Name??"Doomguy"} was killed by {this.GetType().Name}";
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

    // virtual method to Let children hide their billboard to certain clients.
    public virtual bool DrawBillboard(){return true;}
}

public class BillboardSprite : ModelEntity{

    public BillboardSprite() {

    }

    [Event.Frame]
    public void UpdateRotation(){
        if(!IsValid) return;
        if(Local.Pawn is not DoomPlayer ply)return;
        var camAng = ply.CameraPosition().angle;
        Rotation = Rotation.FromYaw(camAng.Angles().yaw - 90);
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
            if(IsClient)
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