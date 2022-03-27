using System;
using Sandbox;

public class SectorMover : AnimEntity {
    public Sector sector;
    public int speed = 4;
    public bool isFloor = false;


    TimeSince nextTick = 0f;
    [Event.Tick.Client]
    public void RebuildMesh(){
        if(Parent is not SectorMeshProp smp)return;
        smp.sector.Rebuild();
    }
    [Event.Tick.Server]
    public void MoveSector(){
        if(Parent is not SectorMeshProp smp)return;
        if (nextTick > 0){
            nextTick = -1f/35f;
            
            float h = isFloor ? sector.floorHeight : sector.ceilingHeight;
            float j = isFloor ? sector.ceilingHeight : sector.floorHeight;
            if(h > GetMoveTarget()){
                if(!isFloor){
                    foreach(var e in DoomMap.ThingsInSector(sector)){
                        if(Math.Abs(j-(h-speed)) < (e.CollisionBounds.Maxs.z - e.CollisionBounds.Mins.z)/WadLoader.mapScale.z){
                            OnBounced();
                            return;
                        }
                    }
                }
                h -= speed * 1f;
                if(h <= GetMoveTarget()){
                    h = GetMoveTarget();
                    ReachedTarget();
                }
                if(isFloor)sector.floorHeight = h;
                else sector.ceilingHeight = h;
                sector.Rebuild();
            }else if(h < GetMoveTarget()){
                var sectorThings = DoomMap.ThingsInSector(sector);
                if(isFloor){
                    foreach(var e in sectorThings){
                        var tr = Trace.Box(e.CollisionBounds, e.Position, e.Position + new Vector3(0,0,speed) * WadLoader.mapScale)
                            .HitLayer(CollisionLayer.PLAYER_CLIP | CollisionLayer.NPC_CLIP)
                            .Ignore(smp)
                            .Ignore(e)
                            .Run();
                        if(tr.Hit){
                            OnBounced();
                            return;
                        }
                    }
                    foreach(var e in sectorThings){
                        if(e.Position.z+e.CollisionBounds.Mins.z < (h+speed)*WadLoader.mapScale.z){
                            e.Position = e.Position.WithZ((h+speed)*WadLoader.mapScale.z+e.CollisionBounds.Mins.z);
                        }
                    }
                }

                h += speed * 1f;
                if(h >= GetMoveTarget()){
                    h = GetMoveTarget();
                    ReachedTarget();
                }
                if(isFloor)sector.floorHeight = h;
                else sector.ceilingHeight = h;
                sector.Rebuild();
            }else{
                ReachedTarget();
            }
        }
    }
    public virtual void ReachedTarget(){}
    public virtual void OnBounced(){}
    public virtual float GetMoveTarget(){
        return isFloor ? sector.floorHeight : sector.ceilingHeight;
    }
}