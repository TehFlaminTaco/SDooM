using Sandbox;
using Sandbox.UI;

[Library]
public partial class DoomHud : HudEntity<RootPanel> {
    public static DoomHud Instance;

    [ConVar.ClientData("cl_hud_scale")]
    public static string _HudScale {get;set;} = "4.0";
    public static string HudScale => Local.Client!=null ? Local.Client.GetClientData("cl_hud_scale", "4.0") : "4.0";
    public static float LastHudScale = 0f;
    public DoomHud(){
        Instance = this;
        if(TextureLoader2.Instance != null)
            Build();
    }

    ItemPickupFlash itemFlash;
    DamageFlash damageFlash;
    UIBar bar;
    StatusText statusText;
    bool isBuilt = false;
    public void Build(){
        if(!IsClient)
            return;
        if(isBuilt)return;
        isBuilt = true;
        Log.Info("BUILDING!");
        RootPanel.StyleSheet.Load( "/styles/DoomHud.scss" );

        bar = RootPanel.AddChild<UIBar>();
        itemFlash = RootPanel.AddChild<ItemPickupFlash>();
        damageFlash = RootPanel.AddChild<DamageFlash>();
        statusText = RootPanel.AddChild<StatusText>();
    }

    public void ResizeChildren(float scale){
        bar.ResizeChildren(scale);
    }

    string lastScale = "";
    [Event.Tick.Client]
    public void Tick(){
        if(HudScale != lastScale && bar != null){
            if(float.TryParse(HudScale, out float s)){
                LastHudScale = s;
                ResizeChildren(s);
            }
            lastScale = HudScale;
        }
    }
}