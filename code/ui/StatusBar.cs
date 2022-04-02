using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

public class StatusBar : Panel {
    Face face;
    Arms arms;
    DoomTextBig health;
    DoomTextBig armor;
    DoomTextBig ammo;
    Dictionary<AmmoType, DoomTextAmmo> ammoAmounts = new();
    Dictionary<AmmoType, DoomTextAmmo> ammoMaxes = new();

    // Blue yellow red key displays
    KeycardDisplay blueKeyDisplay;
    KeycardDisplay yellowKeyDisplay;
    KeycardDisplay redKeyDisplay;

    public StatusBar(){
        face = AddChild<Face>();
        arms = AddChild<Arms>();
        health = AddChild<DoomTextBig>("health");
        armor = AddChild<DoomTextBig>("armor");
        ammo = AddChild<DoomTextBig>("ammo");
        AddChild(blueKeyDisplay = new(0, "blue"));
        AddChild(yellowKeyDisplay = new(1, "yellow"));
        AddChild(redKeyDisplay = new(2, "red"));

        foreach(var typ in Enum.GetValues<AmmoType>().Where(c=>c!=AmmoType.None)){
            var ammoAmount = AddChild<DoomTextAmmo>();
            ammoAmount.AddClass("ammo");
            ammoAmount.AddClass("amount");
            ammoAmount.AddClass(typ.ToString());
            ammoAmount.Text = "0";
            ammoAmounts[typ] = ammoAmount;
            var ammoMax = AddChild<DoomTextAmmo>();
            ammoMax.AddClass("ammo");
            ammoMax.AddClass("max");
            ammoMax.AddClass(typ.ToString());
            ammoMax.Text = "0";
            ammoMaxes[typ] = ammoMax;
        }
        
        var barTex = TextureLoader2.Instance.GetUITexture("STBAR");
        Style.BackgroundImage = barTex;
        Style.Width = barTex.Width;
        Style.Height = barTex.Height;
        Style.Dirty();
    }

    public void ResizeChildren(float s){
        var barTex = Style.BackgroundImage;
        Style.Width = barTex.Width * s;
        Style.Height = barTex.Height * s;
        Style.Dirty();
        face.ResizeChildren(s);
        arms.ResizeChildren(s);
    }

    public override void Tick(){
        if(Local.Pawn is not DoomPlayer ply)return;
        health.Text = ply.Health+"%";
        armor.Text = ply.Armor+"%";
        if(ply.ActiveChild is Weapon wep && wep.UsesAmmo){
            ammo.Text = ply.AmmoCount(wep.Clip1Type) + "";
        }else{
            ammo.Text = "";
        }
        foreach(var typ in Enum.GetValues<AmmoType>().Where(c=>c!=AmmoType.None)){
            ammoAmounts[typ].Text = ply.AmmoCount(typ)+"";
            ammoMaxes[typ].Text = ply.AmmoMax(typ)+"";
        }
    }
}