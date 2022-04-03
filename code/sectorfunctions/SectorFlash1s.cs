using System;
using System.Linq;
using Sandbox;
public partial class SectorFlash1s : SectorFunction {
    float brightHigh = -1;
    float brightLow = -1;

    public override void Setup(){
        brightHigh = sector._brightness/255f;
        brightLow = sector.Adjacent.Select(c=>c._brightness).Min()/255f;
        if(Host.IsServer)
            Parent = sector.floorObject;
    }

    [Net] TimeSince flashTime {get;set;}
    [Event.Tick.Client]
    [Event.Tick.Server]
    public void FlashSector(){
        if(IsServer && flashTime >= 1f){
            flashTime = 0f;
        }
        if(brightHigh>0 && sector != null){
            sector.brightness = flashTime>=(7f/35f)?brightLow:brightHigh;
            sector.UpdateBrightness();
        }
    }
}