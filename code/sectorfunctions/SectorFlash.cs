using System;
using System.Linq;
using Sandbox;
public class SectorFlash : SectorFunction {
    float brightHigh = -1;
    float brightLow = -1;

    public override void Setup(){
        brightHigh = sector._brightness/255f;
        brightLow = sector.Adjacent.Select(c=>c._brightness).Min()/255f;
        if(Host.IsServer)
            Parent = sector.floorObject;
    }


    private const string flickerPattern = "mmamammmmammamamaaamammma";
    [Event.Tick.Client]
    [Event.Tick.Server]
    public void FlashSector(){
        
        int tick = (int)(Time.Now*(35f/7f));
        if(brightHigh>0 && sector != null){
            sector.brightness = flickerPattern[tick%flickerPattern.Length]=='m'?brightLow:brightHigh;
            sector.UpdateBrightness();
        }
    }
}