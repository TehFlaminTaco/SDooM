using System.Linq;
using System.Text.RegularExpressions;
using Sandbox;

public partial class LineDefFunction : AnimEntity {
	public static int[] WalkTriggers = new[]{2, 3, 4, 5, 6, 8, 10, 12, 13, 16, 17, 19, 22, 25, 30, 35, 36, 37, 38, 39, 40, 44, 52, 53, 54, 56, 57, 58, 59, 72, 73, 74, 75, 76, 77, 79, 80, 81, 82, 83, 84, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 104};

    [Net] public int _line {get; set;}
    public Linedef line {
		get {
			return MapLoader.linedefs[_line];
		}
	}
}

public partial class LineMeshProp : IUse{
    public bool OnUse( Entity user )
	{
		if(user is not DoomPlayer ply)return false;
		if(Host.IsServer){
			switch(line.lineType){
				case 1: case 26: case 27: case 28: {
					if(line.Back.Sector.mover == null || !line.Back.Sector.mover.IsValid){
						if(line.lineType > 1){ // If this is a key door.
							if(line.lineType switch {
								26 => ply.Keys.Blue == 0,
								27 => ply.Keys.Yellow == 0,
								28 => ply.Keys.Red == 0,
								_ => true
							}){ // And we don't have a key
								return false; // Exit.
							}
						}
						line.Back.Sector.mover=new Door(){
							Parent = line.Back.Sector.ceilingObject,
							sector = line.Back.Sector
						};
						SoundLoader.PlaySound("DSDOROPN", line.Back.Sector.ceilingObject.CollisionWorldSpaceCenter);
					}
					break;
				}
				case 11:{
					if(line.Switcher == null || !line.Switcher.IsValid){
						line.Switcher=new LineSwitch(){
							Parent = line.gameObjects.Where(c=>c!=null).First(),
							line = line
						};
						//DoomGame.ChangeLevel("E1M2");
						// TODO: Level changing.
					}
					break;
				}
				case 103:{ // S1 Door Open Stay
					if(line.Switcher == null || !line.Switcher.IsValid){
						var sec = MapLoader.sectors.Where(c=>c.tag == line.lineTag);
						if(!sec.Any())return false;
						foreach(var s in sec){
							if(s.mover == null || !s.mover.IsValid){
								s.mover = new Door(){
									Parent = s.ceilingObject,
									sector = s,
									StayOpen = true
								};
							}
						}
						line.Switcher=new LineSwitch(){
							Parent = line.gameObjects.Where(c=>c!=null).First(),
							line = line,
							Stay = true
						};
					}
					break;
				}
			}
		}
		return true;
	}

	public void OnShoot(){
		if(Host.IsServer){
			switch(line.lineType){
				case 46:
					var sec = MapLoader.sectors.Where(c=>c.tag == line.lineTag);
					if(!sec.Any())return;
					foreach(var s in sec){
						if(s.mover == null || !s.mover.IsValid){
							s.mover = new Door(){
								Parent = s.ceilingObject,
								sector = s,
								StayOpen = true
							};
						}
					}
					break;
			}
		}
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public override void StartTouch(Entity other){
		if(IsClient)return;
		if(other is DoomPlayer ply && LineDefFunction.WalkTriggers.Contains(line.lineType)){
			switch(line.lineType){
				case 36:{
					var sec = MapLoader.sectors.Where(c=>c.tag == line.lineTag);
					if(!sec.Any())return;
					foreach(var s in sec){
						if(s.mover == null || !s.mover.IsValid){
							s.mover = new FloorEightOver(){
								Parent = s.floorObject,
								sector = s,
								speed = 8,
								isFloor = true
							};
							SoundLoader.PlaySound("DSPSTART", s.floorObject.CollisionWorldSpaceCenter);
						}
					}
					break;
				}
				case 88:{
					var sec = MapLoader.sectors.Where(c=>c.tag == line.lineTag);
					if(!sec.Any())return;
					foreach(var s in sec){
						if(s.mover == null || !s.mover.IsValid){
							s.mover = new LiftLowerWaitRaise(){
								Parent = s.floorObject,
								sector = s,
								speed = 4,
								isFloor = true
							};
							SoundLoader.PlaySound("DSPSTART", s.floorObject.CollisionWorldSpaceCenter);
						}
					}
					break;
				}
			}
		}
	}
}