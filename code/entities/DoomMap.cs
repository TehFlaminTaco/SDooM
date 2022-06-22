using System.Threading;
using System.Numerics;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
[Library( "ent_doommap", Title = "Doom Map" )]
public partial class DoomMap : Prop {
	//public WadLoader MapData;
	TimeSince created = 0;
	bool spawnThings = false;
	public override void Spawn()
	{
		base.Spawn();
		if(Host.IsServer){
			SetupFromData();
			foreach(var thing in All.OfType<ThingEntity>()){
				thing.Delete();
			}
			spawnThings = true;
			created = 0;
			
		}
    }

	[Event.Tick]
	public void MapTick(){
		if(spawnThings && created > 2f){
			spawnThings = false;
			ThingGenerator.GenerateThings();	
		}
	}

	public static Vector3 ToMapLocation(Vector2 pos){
		var tr = Trace.Ray(new Vector3(pos.x, pos.y, MapLoader.maxZ + 10) * WadLoader.mapScale,new Vector3(pos.x, pos.y, MapLoader.minZ - 10) * WadLoader.mapScale ).UseHitboxes().Size(2).Run();
		if(tr.Entity is SectorMeshProp mp){
			Sector s = mp.sector;
			if(s != null){
				return new Vector3(pos.x, pos.y, s.floorHeight) * WadLoader.mapScale;
			}
		}
		return Vector3.Zero;
	}

	public static Sector GetSector(Vector2 pos){
		var tr = Trace.Ray(new Vector3(pos.x, pos.y, MapLoader.maxZ + 10) * WadLoader.mapScale,new Vector3(pos.x, pos.y, MapLoader.minZ - 10) * WadLoader.mapScale ).UseHitboxes().Size(2).Run();
		if(tr.Entity is SectorMeshProp mp){
			return mp.sector;
		}
		return null;
	}

	public static IEnumerable<ModelEntity> ThingsInSector(Sector s){
		s.floorObject?.Tags.Add("thissector");
		s.ceilingObject?.Tags.Add("thissector");
		var reslt = All.Where(c=>c is DoomPlayer || c is ThingEntity).OfType<ModelEntity>()
			.Where(c=>{
				var tr = Trace.Box(c.CollisionBounds, c.Position, c.Position + new Vector3(0,0,-1000))
					.HitLayer(CollisionLayer.All)
					.WithAnyTags("thissector")
					.Run();
				return tr.Hit;
			}).ToList();
		s.floorObject?.Tags.Remove("thissector");
		s.ceilingObject?.Tags.Remove("thissector");
		return reslt;
	}

	public override void ClientSpawn(){
		base.ClientSpawn();
		if(Host.IsClient){
			/*var msc = MusicLoader.LoadMusic("D_E1M1");
			if(msc.HasValue)
				msc.Value.SetVolume(200f);
			else
				Log.Info("What, No stream?");*/
			// TODO: Fix music
		}
	}

	public override void Simulate(Client cl){
		if(Host.IsServer)return;
		/*if(!Children.Any()){
			SetupFromData();
		}*/
	}
	public void SetupFromData(){
		//Mesh mapMesh = new Mesh();
		//var modelBuilder = new ModelBuilder();
		/*foreach(var LineDef in MapLoader.linedefs){
			//Log.Info(MapData.SideDefs[LineDef.FrontSidedef].MiddleTexture);
			if(LineDef.Front.tMid != "-")
				modelBuilder.AddMesh(MeshFromLinedef(LineDef, LineDef.Front, 0));
			
		}*/
		{
            int index = 0;
            foreach (Linedef l in MapLoader.linedefs)
            {
                if (l.Back != null)
                {
                    //top part (front)
                    if (l.Front.tHigh != "-" || l.Front.Sector.ceilingHeight > l.Back.Sector.ceilingHeight)
                        l.TopFrontObject = new LineMeshProp(){
							solid = true,
							_line = index,
							position = 3,
							isFront = true,
							Parent = this
						};

                    //top part (back)
                    if (l.Back.tHigh != "-" || l.Front.Sector.ceilingHeight < l.Back.Sector.ceilingHeight)
                        l.TopBackObject = new LineMeshProp(){
							solid = true,
							_line = index,
							position = 3,
							isFront = false,
							Parent = this
						};

                    //bottom part (front)
                    if (l.Front.tLow != "-" || l.Front.Sector.minimumFloorHeight < l.Back.Sector.floorHeight)
                        l.BotFrontObject = new LineMeshProp(){
							solid = true,
							_line = index,
							position = 1,
							isFront = true,
							Parent = this
						};
                    //bottom part (back)
                    if (l.Back.tLow != "-" || l.Front.Sector.floorHeight > l.Back.Sector.floorHeight)
                        l.BotBackObject = new LineMeshProp(){
							solid = true,
							_line = index,
							position = 1,
							isFront = false,
							Parent = this
						};

                    //middle (front)
                    if (l.Front.tMid != "-")
                        l.MidFrontObject = new LineMeshProp(){
							solid = (l.flags&0x01)>0,
							_line = index,
							position = 2,
							isFront = true,
							Parent = this
						};

                    //middle (back)
                    if (l.Back.tMid != "-")
                        l.MidBackObject = new LineMeshProp(){
							solid = (l.flags&0x01)>0,
							_line = index,
							position = 2,
							isFront = false,
							Parent = this
						};

                    if ((l.flags & (1 << 0)) != 0)
                    	l.InvisibleBlockerObject = new LineMeshProp(){
							solid = true,
							_line = index,
							isInvisibleBlocker = true,
							isFront = false,
							Parent = this
						};
					else if(LineDefFunction.WalkTriggers.Contains(l.lineType)){
						l.InvisibleBlockerObject = new LineMeshProp(){
							solid = true,
							_line = index,
							isInvisibleBlocker = true,
							isWalkTrigger = true,
							isFront = false,
							Parent = this
						};
					}

                        
                }
                else //solid wall
                    l.SolidWallObject = new LineMeshProp(){
						solid = true,
						_line = index,
						isSolidWall = true,
						isFront = true,
						Parent = this
					};

                index++;
            }
        }
		{
			int index = 0;
			foreach(var Sector in MapLoader.sectors){
				Sector.floorObject = new SectorMeshProp(){
					solid = true,
					_sector = index,
					floor = 1,
					Parent = this
				};
				Sector.ceilingObject = new SectorMeshProp(){
					solid = true,
					_sector = index,
					floor = 2,
					Parent = this
				};
				index++;
			}
		}
		//Model = modelBuilder.Create();
	}
}

public static class Vector3Helper {
	public static Vector3 FromXZY(this Vector3 v){
		return new Vector3(v.x, v.z, v.y) * WadLoader.mapScale;
	}
}

public partial class MeshProp : Prop {
	[Net] public bool solid {get; set;}
	public Material meshMaterial = null;

	public void Finish((Mesh mesh, List<VoxelVertex> points) mp){
		if(mp.mesh is null)return;
		var modelBuilder = new ModelBuilder();
		Vector3 mins = mp.points[0].Position;
		Vector3 maxs = mp.points[0].Position;
		for(int i=0; i < mp.points.Count; i++){
			var p = mp.points[i].Position;
			if(p.x < mins.x)mins.x = p.x;
			if(p.y < mins.y)mins.y = p.y;
			if(p.z < mins.z)mins.z = p.z;
			if(p.x > maxs.x)maxs.x = p.x;
			if(p.y > maxs.y)maxs.y = p.y;
			if(p.z > maxs.z)maxs.z = p.z;
		}
		mp.mesh.SetBounds(mins, maxs);
		modelBuilder.AddMesh(mp.mesh);
		if(solid){
			for(int i=0; i < mp.points.Count; i+=3){
				Vector3 p1 = mp.points[i].Position;
				Vector3 p2 = mp.points[i+1].Position;
				Vector3 p3 = mp.points[i+2].Position;
				modelBuilder.AddCollisionMesh(new Vector3[]{p1, p2, p3}, new int[]{0,1,2});
			}
		}
		Model = modelBuilder.Create();
		SetupPhysicsFromModel(PhysicsMotionType.Static);
	}
}

public partial class LineMeshProp : MeshProp {
	[Net] public int _line {get; set;}
	[Net] public int position {get; set;}
	[Net] public bool isFront {get; set;}
	[Net] public bool isSolidWall {get; set;}
	[Net] public bool isInvisibleBlocker {get; set;}
	[Net] public bool isWalkTrigger {get; set;}
	[Net] public bool isLoaded {get; set;}
	public Linedef line {
		get {
			if(MapLoader.linedefs!=null && (MapLoader.linedefs.Count > _line))
				return MapLoader.linedefs[_line];
			return null;
		}
	}

	public override void Spawn(){
		if(Host.IsServer){
			isLoaded = true;
			//SetupMesh();
		}
	}
	public override void ClientSpawn(){
		//SetupMesh();
	}

	bool MeshSetup = false;
	[Event.Tick]
	[Event( "server.tick" )]
	[Event( "client.tick" )]
	public void Tick(){
		if(MeshSetup)return;
		if(isLoaded && (isSolidWall || isInvisibleBlocker || position > 0) && DoomGame.LevelLoaded() && line != null && !MapLoader.Loading){
			SetupMesh();
			if(isSolidWall) line.SolidWallObject = this;
			if(isInvisibleBlocker) line.InvisibleBlockerObject = this;
			if(isFront){
				if(position == 1) line.BotFrontObject = this;
				if(position == 2) line.MidFrontObject = this;
				if(position == 3) line.TopFrontObject = this;
			}else{
				if(position == 1) line.BotBackObject = this;
				if(position == 2) line.MidBackObject = this;
				if(position == 3) line.TopBackObject = this;
			}
			MeshSetup = true;
		}
	}

	public void SetupMesh(){
		Assert.True(isLoaded && (isSolidWall || isInvisibleBlocker || position > 0));
		if(line!=null && ((isSolidWall||isFront) ? (line.Front!=null && line.Front.Sector!=null) : (line.Back!=null && line.Back.Sector!=null))){
			Sector sector = (isSolidWall||isFront)?line.Front.Sector:line.Back.Sector;
			RenderColor = new Color(sector.brightness,sector.brightness,sector.brightness,1);
		}
		var l = line;
		if(!Host.IsServer){
			if(l.Front != null && l.Front.Sector!=null){
				if(l.Front.Sector.ceilingObject != null && l.Front.Sector.ceilingObject.IsValid ){
					l.Front.Sector.ceilingHeight = l.Front.Sector.ceilingObject.LocalPosition.z/WadLoader.mapScale.z + (l.Front.Sector._ceilingHeight / MapLoader.sizeDividor);
				}
				if(l.Front.Sector.floorObject != null && l.Front.Sector.floorObject.IsValid ){
					l.Front.Sector.floorHeight = l.Front.Sector.floorObject.LocalPosition.z/WadLoader.mapScale.z + (l.Front.Sector._floorHeight / MapLoader.sizeDividor);
				}
			}
			if(l.Back != null && l.Back.Sector!=null){
				if(l.Back.Sector.ceilingObject != null && l.Back.Sector.ceilingObject.IsValid ){
					l.Back.Sector.ceilingHeight = l.Back.Sector.ceilingObject.LocalPosition.z/WadLoader.mapScale.z + (l.Back.Sector._ceilingHeight / MapLoader.sizeDividor);
				}
				if(l.Back.Sector.floorObject != null && l.Back.Sector.floorObject.IsValid ){
					l.Back.Sector.floorHeight = l.Back.Sector.floorObject.LocalPosition.z/WadLoader.mapScale.z + (l.Back.Sector._floorHeight / MapLoader.sizeDividor);
				}
			}
		}
		(Mesh mesh, List<VoxelVertex> points) mp = (null, null);
		if(isInvisibleBlocker) mp = CreateInvisibleBlocker(
			l,
			Math.Max(l.Front.Sector.floorHeight, l.Back.Sector.floorHeight),
			Math.Min(l.Front.Sector.ceilingHeight, l.Back.Sector.ceilingHeight)
		);
		else if(isSolidWall) mp = CreateLineQuad(
			l.Front,
			l.Front.Sector.minimumFloorHeight,
			l.Front.Sector.maximumCeilingHeight,
			l.Front.tMid,
			l.Front.offsetX,
			l.Front.offsetY,
			((l.flags & (1 << 4)) != 0) ? 1 : 0,
			false,
			l.Front.Sector.brightness
		);
		else if(position == 3 && isFront) mp = CreateLineQuad (
			line.Front,
			line.Back.Sector.ceilingHeight,
			line.Front.Sector.ceilingHeight,
			line.Front.tHigh,
			line.Front.offsetX,
			(line.flags & (1 << 3)) != 0 ? line.Front.offsetY : -line.Front.offsetY,
			(line.flags & (1 << 3)) != 0 ? 0 : 1,
			false,
			line.Front.Sector.brightness
		);
		else if(position == 3 && !isFront) mp = CreateLineQuad(
			line.Back,
			line.Front.Sector.ceilingHeight,
			line.Back.Sector.ceilingHeight,
			line.Back.tHigh,
			line.Back.offsetX,
			(line.flags & (1 << 3)) != 0 ? line.Back.offsetY : -line.Back.offsetY,
			(line.flags & (1 << 3)) != 0 ? 0 : 1,
			true,
			line.Back.Sector.brightness
		);
		else if (position == 1 && isFront) mp = CreateLineQuad(
			l.Front,
			l.Front.Sector.minimumFloorHeight,
			l.Back.Sector.floorHeight,
			l.Front.tLow,
			l.Front.offsetX,
			l.Front.offsetY,
			((l.flags & (1 << 4)) != 0) ? 2 : 0,
			false,
			l.Front.Sector.brightness
		);
		else if(position == 1 && !isFront) mp = CreateLineQuad(
			l.Back,
			l.Back.Sector.minimumFloorHeight,
			l.Front.Sector.floorHeight,
			l.Back.tLow,
			l.Back.offsetX,
			l.Front.offsetY,
			((l.flags & (1 << 4)) != 0) ? 2 : 0,
			true,
			l.Back.Sector.brightness
		);
		else if(position == 2 && isFront) mp = CreateLineQuad(
			l.Front,
			Math.Max(l.Front.Sector.floorHeight, l.Back.Sector.floorHeight),
			Math.Min(l.Front.Sector.ceilingHeight, l.Back.Sector.ceilingHeight),
			l.Front.tMid,
			l.Front.offsetX,
			l.Front.offsetY,
			((l.flags & (1 << 4)) != 0) ? 1 : 0,
			false,
			l.Front.Sector.brightness
		);
		else if(position == 2 && !isFront) mp = CreateLineQuad(
			l.Back,
			Math.Max(l.Front.Sector.floorHeight, l.Back.Sector.floorHeight),
			Math.Min(l.Front.Sector.ceilingHeight, l.Back.Sector.ceilingHeight),
			l.Back.tMid,
			l.Back.offsetX,
			l.Back.offsetY,
			((l.flags & (1 << 4)) != 0) ? 1 : 0,
			true,
			l.Back.Sector.brightness
		);
		else
			Log.Info("Unknown state!");
		if(mp.mesh == null){
			//Log.Info($"{position} {isFront} {isInvisibleBlocker} {isSolidWall}");
			//Log.Error("Couldn't generate mesh!");
			Model = null;
			return;
		}
		Finish(mp);
		if(position == 2 || isInvisibleBlocker){
			ClearCollisionLayers();
			if(isWalkTrigger){
				CollisionGroup = CollisionGroup.Trigger;
				SetInteractsWith(CollisionLayer.Player);
			}else{
				if((l.flags&0x01)>0){
					AddCollisionLayer(CollisionLayer.PLAYER_CLIP);
				}
				if((l.flags&0x02)>0){
					AddCollisionLayer(CollisionLayer.NPC_CLIP);
					AddCollisionLayer(CollisionLayer.Hitbox);
				}
			}
		}else{
			AddCollisionLayer(CollisionLayer.Hitbox);
			AddCollisionLayer(CollisionLayer.PLAYER_CLIP);
			AddCollisionLayer(CollisionLayer.NPC_CLIP);
		}
		
	}

	public (Mesh, List<VoxelVertex>) CreateLineQuad(Sidedef s, float min, float max, string tex, int offsetX, int offsetY, int peg, bool invert, float brightness)
    {
        if (max - min <= 0)
            return (null,null);

        if (s.Line.start == s.Line.end)
            return (null,null);

        if (tex == "-")
            tex = "DOORTRAK";
        Mesh mesh = new Mesh();
        Texture mainTexture = null;
		Material mat = meshMaterial;
		bool hasAlphaCut = false;
		if(mat == null){
			if (!MaterialManager.Instance.OverridesWall(tex, out var mr))
				if (TextureLoader2.NeedsAlphacut.ContainsKey(tex))
					hasAlphaCut = true;
				else
					hasAlphaCut = false;
			mat=mr;
		}
		
		mainTexture = TextureLoader2.Instance.GetWallTexture(tex);
		if(mat == null){
			mat = Material.Load(hasAlphaCut ? "materials/pixelperfect.vmat" : "materials/pixelperfectnoalpha.vmat").CreateCopy();
			mat.OverrideTexture("Color", mainTexture);
			if(IsClient)
				TextureAnimator.TryGenerateAnimator(this, mat, tex, TextureAnimator.Mode.WALL); // TODO: Don't do if already has an animator.
		}
		meshMaterial = mesh.Material = mat;
        int vc = 4;

        Vector3[] vertices = new Vector3[vc];
        Vector3[] normals = new Vector3[vc];
        Vector2[] uvs = new Vector2[vc];
        Color[] colors = new Color[vc];
        int[] indices = new int[6];

        vertices[0] = new Vector3(s.Line.start.Position.x, min, s.Line.start.Position.y);
        vertices[1] = new Vector3(s.Line.end.Position.x, min, s.Line.end.Position.y);
        vertices[2] = new Vector3(s.Line.start.Position.x, max, s.Line.start.Position.y);
        vertices[3] = new Vector3(s.Line.end.Position.x, max, s.Line.end.Position.y);

        if (mainTexture != null)
        {
            float length = (s.Line.start.Position - s.Line.end.Position).GetLength();
            float height = max - min;
            float u = length / ((float)mainTexture.Width / MapLoader.sizeDividor);
            float v = height / ((float)mainTexture.Height / MapLoader.sizeDividor);
            float ox = (float)offsetX / (float)mainTexture.Width;
            float oy = (float)offsetY / (float)mainTexture.Height;

            if (peg == 2)
            {
                float sheight = s.Sector.ceilingHeight - s.Sector.minimumFloorHeight;
                float sv = sheight / ((float)mainTexture.Height / MapLoader.sizeDividor);

                uvs[0] = new Vector2(ox, 1 - sv);
                uvs[1] = new Vector2(u + ox, 1 - sv);
                uvs[2] = new Vector2(ox, 1 - sv + v);
                uvs[3] = new Vector2(u + ox, 1 - sv + v);
            }
            else if (peg == 1)
            {
                uvs[0] = new Vector2(ox, oy);
                uvs[1] = new Vector2(u + ox, oy);
                uvs[2] = new Vector2(ox, v + oy);
                uvs[3] = new Vector2(u + ox, v + oy);
            }
            else
            {
                uvs[0] = new Vector2(ox, 1 - v - oy);
                uvs[1] = new Vector2(u + ox, 1 - v - oy);
                uvs[2] = new Vector2(ox, 1 - oy);
                uvs[3] = new Vector2(u + ox, 1 - oy);
            }
        }

        if (invert)
        {
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;

            uvs = new Vector2[4] { uvs[1], uvs[0], uvs[3], uvs[2] };
        }
        else
        {
            indices[0] = 2;
            indices[1] = 1;
            indices[2] = 0;
            indices[3] = 3;
            indices[4] = 1;
            indices[5] = 2;
        }

        Vector3 normal = (vertices[0] - vertices[1]).Normal;
        float z = normal.z;
        float x = normal.x;
        normal.x = -z;
        normal.z = x;
        for (int i = 0; i < 4; i++)
        {
            normals[i] = invert ? -normal : normal;
            colors[i] = Color.White * brightness;
        }
		List<VoxelVertex> verts = new();
		for(int i=0; i < indices.Length; i+=3){
			Vector3 p1 = vertices[indices[i]].FromXZY();
			Vector3 p2 = vertices[indices[i+1]].FromXZY();
			Vector3 p3 = vertices[indices[i+2]].FromXZY();

			verts.Add(new VoxelVertex(p1, Vector3.Up, Vector3.Right, uvs[indices[i]]));
			verts.Add(new VoxelVertex(p2, Vector3.Up, Vector3.Right, uvs[indices[i+1]]));
			verts.Add(new VoxelVertex(p3, Vector3.Up, Vector3.Right, uvs[indices[i+2]]));
		}
		verts.Reverse();
		if(mesh.HasVertexBuffer){
			mesh.SetVertexBufferSize(verts.Count);
			mesh.SetVertexBufferData(verts);
		}else{
			mesh.CreateVertexBuffer(verts.Count, Helpers.MeshLayout, verts);
		}
        return (mesh, verts);
    }

	public (Mesh, List<VoxelVertex>) CreateInvisibleBlocker(Linedef l, float min, float max)
    {
        if (max - min <= 0)
            return (null, null);

        if (l.start == l.end)
            return (null, null);
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        Vector3[] normals = new Vector3[8];

        int[] indices = new int[12];

        vertices[0] = new Vector3(l.start.Position.x, min, l.start.Position.y);
        vertices[1] = new Vector3(l.end.Position.x, min, l.end.Position.y);
        vertices[2] = new Vector3(l.start.Position.x, max, l.start.Position.y);
        vertices[3] = new Vector3(l.end.Position.x, max, l.end.Position.y);

        vertices[4] = vertices[0];
        vertices[5] = vertices[1];
        vertices[6] = vertices[2];
        vertices[7] = vertices[3];

        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 2;
        indices[4] = 1;
        indices[5] = 3;

        indices[6] = 2;
        indices[7] = 1;
        indices[8] = 0;
        indices[9] = 3;
        indices[10] = 1;
        indices[11] = 2;

        Vector3 normal = (vertices[0] - vertices[1]).Normal;
        float z = normal.z;
        float x = normal.x;
        normal.x = -z;
        normal.z = x;
        for (int i = 0; i < 4; i++)
            normals[i] = -normal;
        for (int i = 4; i < 8; i++)
            normals[i] = normal;

        List<VoxelVertex> verts = new();
		for(int i=0; i < indices.Length; i+=3){
			Vector3 p1 = vertices[indices[i]].FromXZY();
			Vector3 p2 = vertices[indices[i+1]].FromXZY();
			Vector3 p3 = vertices[indices[i+2]].FromXZY();

			verts.Add(new VoxelVertex(p1, Vector3.Up, Vector3.Right, Vector3.Zero));
			verts.Add(new VoxelVertex(p2, Vector3.Up, Vector3.Right, Vector3.Zero));
			verts.Add(new VoxelVertex(p3, Vector3.Up, Vector3.Right, Vector3.Zero));
		}
		if(mesh.HasVertexBuffer){
			mesh.SetVertexBufferSize(verts.Count);
			mesh.SetVertexBufferData(verts);
		}else{
			mesh.CreateVertexBuffer(verts.Count, Helpers.MeshLayout, verts);
		}
		return (mesh, verts);
    }
}

public partial class SectorMeshProp : MeshProp {
	[Net] public int _sector {get; set;}
	[Net] public int floor {get; set;}
	[Net] public bool isLoaded {get; set;}
	public Sector sector {
		get {
			if(MapLoader.sectors!=null && MapLoader.sectors.Count > _sector)
				return MapLoader.sectors[_sector];
			return null;
		}
	}

	public override void Spawn(){
		Predictable = true;
		if(Host.IsServer){
			isLoaded = true;
			//SetupMesh();
		}
	}
	public override void ClientSpawn(){
		//SetupMesh();
	}

	bool MeshSetup = false;
	[Event.Tick]
	[Event( "server.tick" )]
	[Event( "client.tick" )]
	public void Tick(){
		if(MeshSetup)return;
		if(isLoaded && floor > 0 && DoomGame.LevelLoaded() && sector != null && !MapLoader.Loading){
			if(floor == 1)sector.floorObject = this;
			else sector.ceilingObject = this;
			SetupMesh();
			MeshSetup = true;
			if(floor==1)SectorFunction.AddSectorsFunction(sector, this);
		}
	}

	public void SetupMesh(){
		Assert.True(isLoaded && floor > 0);
		RenderColor = new Color(sector.brightness,sector.brightness,sector.brightness,1);
		(Mesh mesh, List<VoxelVertex> points) mp = MeshFromSector(sector, floor == 2);
		if(mp.mesh == null){
			Log.Error("Couldn't generate mesh!");
		}
		Finish(mp);
		AddCollisionLayer(CollisionLayer.Hitbox);
		AddCollisionLayer(CollisionLayer.PLAYER_CLIP);
		AddCollisionLayer(CollisionLayer.NPC_CLIP);
	}

	public (Mesh, List<VoxelVertex>) MeshFromSector(Sector s, bool isCeiling){
		Triangulator2 triangulator = new Triangulator2();
		triangulator.Triangulate(s);
		Mesh mesh = new Mesh();
		Texture mainTexture = null;
		Material mat = meshMaterial;
		if(meshMaterial == null){
			if (!MaterialManager.Instance.OverridesFlat(isCeiling ? s.ceilingTexture : s.floorTexture, out var overrideMat))
				mat = MaterialManager.Instance.defaultMaterial;
			else
				mat = overrideMat;
		}
		mainTexture = TextureLoader2.Instance.GetFlatTexture(isCeiling ? s.ceilingTexture : s.floorTexture);
		if(mat == null){
			mat = Material.Load("materials/pixelperfect.vmat").CreateCopy();
			mat.OverrideTexture("Color", mainTexture);
			if(IsClient)
				TextureAnimator.TryGenerateAnimator(this, mat, isCeiling ? s.ceilingTexture : s.floorTexture, TextureAnimator.Mode.FLAT);
		}
		meshMaterial = mesh.Material = mat;
		int vc = Triangulator2.vertices.Count;

		Vector3[] vertices = new Vector3[vc];
		Vector3[] normals = new Vector3[vc];
		Vector2[] uvs = new Vector2[vc];
		int[] indices = new int[vc];

		int v = 0;
		foreach (Vector2D p in Triangulator2.vertices)
		{
			vertices[v] = new Vector3(p.x, isCeiling ? s.ceilingHeight : s.floorHeight, p.y);
			indices[v] = v;
			normals[v] = Vector3.Up;
			uvs[v] = new Vector2(p.x / MapLoader.flatUVdividor, p.y / MapLoader.flatUVdividor);
			v++;
		}
		List<VoxelVertex> verts = new();
		for(int i=0; i < indices.Length; i+=3){
			Vector3 p1 = vertices[indices[i]].FromXZY();
			Vector3 p2 = vertices[indices[i+1]].FromXZY();
			Vector3 p3 = vertices[indices[i+2]].FromXZY();

			verts.Add(new VoxelVertex(p1, Vector3.Up, Vector3.Right, uvs[i]));
			verts.Add(new VoxelVertex(p2, Vector3.Up, Vector3.Right, uvs[i+1]));
			verts.Add(new VoxelVertex(p3, Vector3.Up, Vector3.Right, uvs[i+2]));
		}
		if(!verts.Any())
			return (null, null);
		if(!isCeiling)verts.Reverse();
		if(mesh.HasVertexBuffer){
			mesh.SetVertexBufferSize(verts.Count);
			mesh.SetVertexBufferData(verts);
		}else{
			mesh.CreateVertexBuffer(verts.Count, Helpers.MeshLayout, verts);
		}
		return (mesh,verts);
	}
}