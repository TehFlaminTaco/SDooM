using System;
using System.Linq;
using Sandbox;

public partial class FloorEightOver : SectorMover {
    public override float GetMoveTarget(){
        return sector.Adjacent.Select(c=>c.floorHeight).Max() + 8f;
    }

    public override void ReachedTarget(){
        DeleteAsync(0.1f);
    }
}