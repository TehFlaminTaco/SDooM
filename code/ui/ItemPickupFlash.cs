using Sandbox;
using Sandbox.UI;

public class ItemPickupFlash : Panel {
    public static ItemPickupFlash Instance;
    public ItemPickupFlash(){
        Instance = this;
    }

    public static TimeSince FlashStart {get;set;}
    public static void DoFlash(){
        FlashStart = 0;
    }

    public override void Tick(){
        float ticksSince = (int)(FlashStart * 35f);
        if(ticksSince < 0)return;
        if(ticksSince >= 3){
            Style.Opacity = 0;
            Style.Dirty();
        }else{
            Style.Opacity = (1f - (ticksSince/4f))*0.3f;
            Style.Dirty();
        }
    }
}