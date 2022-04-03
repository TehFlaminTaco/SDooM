using System.Linq;
using Sandbox;

public partial class SectorFunction : AnimEntity {
    [Net] public int _sector {get; set;}
    [Net] public bool Finished {get; set;}
    bool didSetup = false;
    [Event.Tick.Client]
    private void AwaitChange(){
        if(Finished && !didSetup && MapLoader.sectors!=null && MapLoader.sectors.Count > _sector){
            Setup();
            didSetup = true;
        }
    }
    public virtual void Setup(){}
    public Sector sector {
		get {
			return MapLoader.sectors[_sector];
		}
	}

    public static void AddSectorsFunction(Sector s, SectorMeshProp floorObject){
        switch(s.specialType){
            case 0x1:
                _=new SectorFlash(){
                    _sector = floorObject._sector,
                    Finished = true
                };
                break;
            case 0x3:
                _=new SectorFlash1s(){
                    _sector = floorObject._sector,
                    Finished = true
                };
                break;
            case 0x8:
                _=new SectorGlow(){
                    _sector = floorObject._sector,
                    Finished = true
                };
                break;
        }
    }
}