using Sandbox;

public partial class DoomGame{
    public static string GetObituary(Entity killer, DoomPlayer victim, Entity murderWeapon){
        var victimName = victim.Client?.Name??"Doomguy";
        if(killer is null){
            return $"{victimName} died.";
        }
        if(killer is DoomPlayer murderer){
            if(murderWeapon is Weapon wep){
                return wep.GetObituary(victim, murderer);
            }
            return $"{victimName} was fragged by {killer.Client?.Name??"Doomguy"} with the {murderWeapon.GetType().Name}";
        }
        if(killer is ThingEntity te){
            return te.GetObituary(victim, murderWeapon);
        }
        if(killer is Prop p){
            return $"{victimName} blew up!";
        }
        return $"{victimName} died.";
    }
}