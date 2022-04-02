
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class DoomGame : Sandbox.Game
{
	public static Random RNG = new();
	public static bool DEATHMATCH = false;
	//public static WadReader ActiveWad = null;
	public DoomGame()
	{
		if(IsServer)_ = new DoomHud();
		string hostSide = Host.IsServer ? "SERVER": "CLIENT";
		Log.Info($"Game Created on side {hostSide}");
		LoadDoomMap();
		if(!IsServer && DoomHud.Instance != null)
			DoomHud.Instance.Build();
	}

	[Event.Hotload]
	public void OnReload(){
		LoadDoomMap();
	}

	public static DoomMap MapEntity = null;
	public void LoadDoomMap(){
		/*ActiveWad = new WadReader("DOOM1.WAD");*/
		if (MapEntity != null){
			MapEntity.Delete();
		}
		if(WadLoader.Instance == null){
			WadLoader.Instance ??= new WadLoader();
			MapLoader.Instance ??= new MapLoader();
			TextureLoader2.Instance ??= new TextureLoader2();
			MaterialManager.Instance ??= new MaterialManager();
			WadLoader.Instance.Load("SHAREDOOM.WAD");
			TextureLoader2.Instance.LoadAndBuildAll();
			MaterialManager.Instance.OverrideFlatMaterials["F_SKY1"] = new MaterialOverride{
				material = Material.Load("materials/dev/fake_sky.vmat"),
				overrideName = "F_SKY1"
			};
			MaterialManager.Instance.OverrideWallMaterials["F_SKY1"] = new MaterialOverride{
				material = Material.Load("materials/dev/fake_sky.vmat"),
				overrideName = "F_SKY1"
			};
			MapLoader.Instance.Load("E1M1");
		}
		if(Host.IsServer){MapEntity = new DoomMap();}
	}

	[Event.Tick.Server]
	public void ThingRespawner(){
		if(DEATHMATCH)ThingGenerator.CheckItemRespawns();
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new DoomPlayer(client);
		pawn.Respawn();
		client.Pawn = pawn;

		MoveToSpawnpoint(pawn);
	}

	public override void MoveToSpawnpoint(Entity pwn){
		var spawn = MapLoader.things.Where(c=>c.thingType==(DEATHMATCH?11:0x01)).OrderBy(c=>Guid.NewGuid()).First();
		var tr = Trace.Ray(new Vector3(spawn.posX, spawn.posY, MapLoader.maxZ + 10) * WadLoader.mapScale,new Vector3(spawn.posX, spawn.posY, MapLoader.minZ - 10) * WadLoader.mapScale ).UseHitboxes().Size(2).Run();
		if(tr.Entity is SectorMeshProp mp){
			Sector s = mp.sector;
			if(s != null){
				pwn.Position = new Vector3(spawn.posX, spawn.posY, s.floorHeight) * WadLoader.mapScale;
				pwn.Rotation = Rotation.FromYaw(spawn.facing);
				pwn.EyeRotation = Rotation.FromYaw(spawn.facing);
				return;
			}
		}
		base.MoveToSpawnpoint(pwn);
	}
}