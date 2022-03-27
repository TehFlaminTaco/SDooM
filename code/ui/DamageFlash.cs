using Sandbox;
using Sandbox.UI;

public partial class DamageFlash : Panel {
    public static DamageFlash Instance;
    public DamageFlash(){
        Instance = this;
    }

    public static TimeSince FlashStart {get;set;}
    public static void DoFlash(){
        FlashStart = 0;
    }
    [ClientRpc]
    public static void SetFlash(float amount){
        if(FlashStart > amount)FlashStart = amount;
    }

    public override void Tick(){
        int ticksSince = (int)(FlashStart * 5f);
        if(ticksSince >= 6){
            Style.Opacity = 0;
            Style.Dirty();
        }else{
            Style.Opacity = (1f - ticksSince/7f)*0.89f;
            Style.Dirty();
        }
    }
}