using System;
using System.Linq;
using Sandbox;
public class SectorGlow : SectorFunction {
    float brightHigh = -1;
    float brightLow = -1;

    public override void Setup(){
        brightHigh = sector.Adjacent.Select(c=>c._brightness).Max()/255f;
        brightLow = sector.Adjacent.Select(c=>c._brightness).Min()/255f;
        if(Host.IsServer)
            Parent = sector.floorObject;
    }


    [Event.Tick.Client]
    [Event.Tick.Server]
    public void FlashSector(){
        
        int tick = (int)(Time.Now*35f);
        if(brightHigh>0 && sector != null){
            float f = (float)(Math.Sin(tick * (Math.Tau/35f))+1f)/2f;
            sector.brightness = brightLow + (brightHigh-brightLow)*f;
            sector.UpdateBrightness();
        }
    }
}