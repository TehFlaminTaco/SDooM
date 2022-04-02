using System;
using System.Text.RegularExpressions;
using Sandbox;

public class LineSwitch : AnimEntity {
    public Linedef line;
    TimeSince switchFinish = 0f;
    bool switchTurnedOn = false;
    bool switchTurnedOff = false;
    public bool Stay = false;
    private static Regex SwitchNameOff = new Regex(@"^SW1");
    private static Regex SwitchNameOn = new Regex(@"^SW2");
    [Event.Tick]
    public void MoveSector(){
        if(Parent is not LineMeshProp smp)return;
        line??=smp.line;
        if(!switchTurnedOff && !switchTurnedOn){
            if(line.Front != null){
                line.Front.tHigh = SwitchNameOff.Replace(line.Front.tHigh, "SW2");
                line.Front.tMid = SwitchNameOff.Replace(line.Front.tMid, "SW2");
                line.Front.tLow = SwitchNameOff.Replace(line.Front.tLow, "SW2");
            }
            if(line.Back != null){
                line.Back.tHigh = SwitchNameOff.Replace(line.Back.tHigh, "SW2");
                line.Back.tMid = SwitchNameOff.Replace(line.Back.tMid, "SW2");
                line.Back.tLow = SwitchNameOff.Replace(line.Back.tLow, "SW2");
            }
            if(Host.IsServer){
                SoundLoader.PlaySound("DSSWTCHN", smp.CollisionWorldSpaceCenter);
            }
            if(Stay){
                switchTurnedOff = true;
                switchTurnedOn = true;
            }else{
                switchFinish = -1f;
                switchTurnedOn = true;
                smp.line.Rebuild();
            }
        }else if(!switchTurnedOff){
            if(switchFinish > 0){
                if(line.Front != null){
                    line.Front.tHigh = SwitchNameOn.Replace(line.Front.tHigh, "SW1");
                    line.Front.tMid = SwitchNameOn.Replace(line.Front.tMid, "SW1");
                    line.Front.tLow = SwitchNameOn.Replace(line.Front.tLow, "SW1");
                }
                if(line.Back != null){
                    line.Back.tHigh = SwitchNameOn.Replace(line.Back.tHigh, "SW1");
                    line.Back.tMid = SwitchNameOn.Replace(line.Back.tMid, "SW1");
                    line.Back.tLow = SwitchNameOn.Replace(line.Back.tLow, "SW1");
                }
                if(Host.IsServer){
                    SoundLoader.PlaySound("DSSWTCHN", smp.CollisionWorldSpaceCenter);
                }
                switchTurnedOff = true;
                smp.line.Rebuild();
            }
        }
    }
}