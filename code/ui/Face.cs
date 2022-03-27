using System;
using Sandbox;
using Sandbox.UI;
public partial class Face : Panel {
    public static Face Instance = null;
    private static string FlashImage = "";
    private static TimeSince FlashStart = 0;
    private static int FlashPriority = 0;
    public Face(){
        Instance = this;
        Tick();
    }
    float scale = 1;
    public override void Tick(){
        base.Tick();

        var barTex = TextureLoader2.Instance.GetUITexture(PickFace());
        Style.BackgroundImage = barTex;
        Style.Width = barTex.Width * scale;
        Style.Height = barTex.Height * scale;
        Style.Dirty();
    }

    [ClientRpc]
    public static void FlashFaceServer(string name, float time, int priority){
        FlashFace(name,time,priority);
    }
    public static void FlashFace(string name, float time, int priority){
        if(priority < FlashPriority)return;
        FlashImage = name;
        FlashStart = -time;
        FlashPriority = priority;
    }

    int LastFaceLook = 0;
    public string PickFace(){
        if(Local.Pawn is not DoomPlayer ply)return "STFDEAD0";
        int plyHP = ply.Health switch {
            >80 => 0,
            >60 => 1,
            >40 => 2,
            >20 => 3,
            >0 => 4,
            _ => -1
        };
        if(plyHP == -1)return "STFDEAD0";

        if(ply.RampageStart > 2f)FlashFace("KILL",1f/10f,5);

        if(!string.IsNullOrEmpty(FlashImage) && FlashStart < 0)
            return FlashImage=="ST"?$"STFST{plyHP}{LastFaceLook}":(FlashImage=="TL"||FlashImage=="TR")?$"STF{FlashImage}{plyHP}0":$"STF{FlashImage}{plyHP}";
        FlashPriority = 0; 
        LastFaceLook = DoomGame.RNG.Next()%3;
        FlashFace("ST", 0.5f, 0);
        return $"STF{FlashImage}{plyHP}{LastFaceLook}";
    }

    public void ResizeChildren(float s){
        scale = s;
        var faceTex = Style.BackgroundImage;
        Style.Width = faceTex.Width * s;
        Style.Height = faceTex.Height * s;
        Style.Dirty();
    }
}