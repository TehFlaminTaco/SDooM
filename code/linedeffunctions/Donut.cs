using System.Linq;
using Sandbox;
public partial class Donut : SectorMover {
    public Sector modelSector;
    public bool isPillar;
    public override float GetMoveTarget(){
        return modelSector.floorHeight;
    }

    [ClientRpc]
    public void UpdateTexture(string tex){
        if(Parent is SectorMeshProp smp){
            Log.Info("Change floor, please..?");
            smp.Children.OfType<TextureAnimator>().FirstOrDefault()?.Delete();
            smp.meshMaterial?.OverrideTexture("Color", TextureLoader2.Instance.GetFlatTexture(tex));
        }
    }

    public override void ReachedTarget(){
        if(!isPillar){
            UpdateTexture(modelSector.floorTexture);
            sector.specialType = modelSector.specialType;
        }
        DeleteAsync(0.1f);
    }
}