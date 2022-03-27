using System.Linq;
using Sandbox;

public class Door : SectorMover {
    bool isOpening = true;
    bool isClosing = false;
    TimeSince finishedOpening = 0f;
    public override float GetMoveTarget(){
        if(isOpening){
            return sector.Adjacent.Select(c=>c.ceilingHeight).Where(c=>c > sector.ceilingHeight).Min() - 4f;
        }
        if(isClosing){
            return sector.floorHeight;
        }
        if(finishedOpening > 4f){
            isClosing = true;
            SoundLoader.PlaySound("DSDORCLS", (Parent as SectorMeshProp).CollisionWorldSpaceCenter);
            return sector.floorHeight;
        }
        return base.GetMoveTarget();
    }

    public override void ReachedTarget(){
        if(isOpening){
            isOpening = false;
            finishedOpening = 0f;
        }
        if(isClosing){
            isClosing = false;
            DeleteAsync(0.1f);
            finishedOpening = 0f;
        }
    }

    public override void OnBounced(){
        if(isClosing){
            isClosing = false;
            isOpening = true;
            SoundLoader.PlaySound("DSDOROPN", (Parent as SectorMeshProp).CollisionWorldSpaceCenter);
        }
    }
}