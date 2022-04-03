using Sandbox;

public partial class DoomGun : Weapon {
    public const byte GUNSTATE_IDLE = 0;
    public const byte GUNSTATE_FIRE = 10;

    public byte State {get;set;}
    public TimeSince nextFrame {get;set;}
    public virtual int pixelOffset => 0;
    public virtual int weaponDown => 0;
    public int frameTicks = 1;
    public int animationIndex = 0;
    public bool weaponDrawn = false;
    public TimeSince drawStart = 0;
    public bool weaponStowed = false;
    public TimeSince stowStart = 0;
    public override void Simulate(Client cl){
        if(!weaponDrawn){
            weaponDrawn = true;
            drawStart = 0;
            DoomHud.Instance.weaponSprite?.SetFlash(null, 14, 21);
            OnTick();
            return;
        }else if(drawStart < 0.25f){ // TODO, Correct time?
            return;
        }else if(weaponStowed && stowStart < 0.25f){
            return;
        }else if(weaponStowed){
            DoomHud.Instance.bar.statusBar.arms.doSwap = true;
        }

        if(nextFrame < 0)return;
        OnTick();
        nextFrame = -frameTicks/35f;
    }
    public virtual void OnTick(){

    }
}