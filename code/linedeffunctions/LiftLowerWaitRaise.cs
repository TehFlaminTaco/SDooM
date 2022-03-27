using System.Linq;
using Sandbox;

public class LiftLowerWaitRaise : SectorMover {
    bool isOpening = true;
    bool isClosing = false;
    TimeSince finishedOpening = 0f;
    public override float GetMoveTarget(){
        if(isOpening){
            return sector.Adjacent.Select(c=>c.floorHeight).Where(c=>c < sector.floorHeight).Max() + 1f;
        }
        if(isClosing){
            return sector._floorHeight/MapLoader.sizeDividor;
        }
        if(finishedOpening > 3f){
            isClosing = true;
            SoundLoader.PlaySound("DSPSTART", (Parent as SectorMeshProp).CollisionWorldSpaceCenter);
            return sector._floorHeight/MapLoader.sizeDividor;
        }
        return base.GetMoveTarget();
    }

    public override void ReachedTarget(){
        if(isOpening){
            isOpening = false;
            finishedOpening = 0f;
            SoundLoader.PlaySound("DSPSTOP", (Parent as SectorMeshProp).CollisionWorldSpaceCenter);
        }
        if(isClosing){
            isClosing = false;
            DeleteAsync(0.1f);
            finishedOpening = 0f;
            SoundLoader.PlaySound("DSPSTOP", (Parent as SectorMeshProp).CollisionWorldSpaceCenter);
        }
    }

    public override void OnBounced(){
        if(isClosing){
            isClosing = false;
            isOpening = true;
        }
    }
}