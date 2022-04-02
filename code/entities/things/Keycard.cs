using System;
using Sandbox;

public class Keycard : ThingEntity {
    public virtual int color => 0; // 0 = Blue, 1 = Yellow, 2 = Red
    public virtual byte setting => 1; // 1 = Electronic, 2 = Skull;
    public virtual string spriteBase => "BKEY";
    public string sequence = "AB"; 
    public int spriteIndex = 0;
    public TimeSince lastFrame = 0;
    public override bool SpriteCentered => true;
    [Event.Tick.Server]
    public void AnimationAndExplosion(){
        if(lastFrame > 0){
            lastFrame = -8f/35f;
            SpriteName = spriteBase + sequence[spriteIndex++] + "0";
            if(spriteIndex >= sequence.Length){
                spriteIndex = 0;
            }
        }
    }

    // Set ply.Keys to a modified version WithBlue
    public override void OnTouched(DoomPlayer ply){
        // If the player already has this key setting, don't do anything
        if(color switch {
            0 => ply.Keys.Blue == setting,
            1 => ply.Keys.Yellow == setting,
            2 => ply.Keys.Red == setting,
            _ => throw new Exception("Invalid key color")
        }){return;}

        // If IsClient, StatusText.AddChatEntry to inform player they got a keycard
        if(Host.IsClient){
            StatusText.AddChatEntry("", color switch {
                0 => "Picked up a blue keycard.",
                1 => "Picked up a yellow keycard.",
                2 => "Picked up a red keycard.",
                _ => "Picked up a fucked keycard."
            });
            ItemPickupFlash.DoFlash();
        }else{
            // Play pickup sound
            SoundLoader.PlaySound("DSITEMUP", Position);
        }


        _= color switch {
            0 => ply.Keys = ply.Keys.WithBlue(setting),
            1 => ply.Keys = ply.Keys.WithYellow(setting),
            2 => ply.Keys = ply.Keys.WithRed(setting),
            _ => throw new Exception("Invalid key color") // Impossible, hopefully.
        };
    }

    // Don't draw if Local Pawn as a DoomPlayer has this key
    public override bool DrawBillboard(){
        if (Local.Pawn is not DoomPlayer ply) return false;
        return color switch {
            0 => ply.Keys.Blue != setting,
            1 => ply.Keys.Yellow != setting,
            2 => ply.Keys.Red != setting,
            _ => throw new Exception("Invalid key color")
        };
    }

}