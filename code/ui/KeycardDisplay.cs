using Sandbox;
using Sandbox.UI;

public class KeycardDisplay : Panel {
    byte color = 0;

    // Setup panel with an image of the keycard.
    public KeycardDisplay(byte c, string classname){
        AddClass(classname);
        color = c;
        if(GetSetting()>0){
            Style.BackgroundImage = TextureLoader2.Instance.GetUITexture(TexFromColor());
            Style.Width = Style.BackgroundImage.Width;
            Style.Height = Style.BackgroundImage.Height;
            Style.Dirty();
        }
    }

    private byte GetSetting(){
        if(Local.Pawn is not DoomPlayer ply) return 0;
        switch(color){
            case 0: return ply.Keys.Blue;
            case 1: return ply.Keys.Yellow;
            case 2: return ply.Keys.Red;
            default: return 0;
        }
    }

    private string TexFromColor(){
        switch(color){
            case 0: return "STKEYS" + (GetSetting()*3 - 3);
            case 1: return "STKEYS" + (GetSetting()*3 - 2);
            case 2: return "STKEYS" + (GetSetting()*3 - 1);
            default: return "FUCK";
        }
    }

    public override void Tick(){
        float s = DoomHud.LastHudScale;
        //scale = s;
        if(GetSetting() > 0){
            var tex = Style.BackgroundImage = TextureLoader2.Instance.GetUITexture(TexFromColor());
            Style.Width = tex.Width * s;
            Style.Height = tex.Height * s;
            Style.Dirty();
        }else{
            Style.BackgroundImage = null;
            Style.Width = 0;
            Style.Height = 0;
            Style.Dirty();
        }
    }

}