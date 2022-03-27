using System.Linq;
using Sandbox;

public partial class SectorFunction : AnimEntity {
    [Net] public int _sector {get; set;}
    [Net, Change] public bool Finished {get; set;}
    private void OnFinishedChanged(bool oldV, bool newV){
        Setup();
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
                new SectorFlash(){
                    _sector = floorObject._sector,
                    Finished = true
                };
                break;
        }
    }
}