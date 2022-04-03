using System.Linq;
using Sandbox;
using Sandbox.UI;

public class Arms : Panel {
    ArmsIcon[] icons;
    public Arms(){
        var armsTex = TextureLoader2.Instance.GetUITexture("STARMS");
        Style.BackgroundImage = armsTex;
        Style.Width = armsTex.Width;
        Style.Height = armsTex.Height;
        Style.Dirty();

        icons = new[]{
            new ArmsIcon(2), new ArmsIcon(3), new ArmsIcon(4),
            new ArmsIcon(5), new ArmsIcon(6), new ArmsIcon(7),
        };
        foreach(var ico in icons){
            AddChild(ico);
        }
    }

    public void ResizeChildren(float s){
        var armsIcon = Style.BackgroundImage;
        Style.Width = armsIcon.Width * s;
        Style.Height = armsIcon.Height * s;
        Style.Dirty();
        foreach(var ico in icons){
            ico.ResizeChildren(s);
        }
    }

    public bool doSwap = false;

    [Event( "buildinput" )]
	public void ProcessClientInput( InputBuilder input )
	{
		var player = Local.Pawn as DoomPlayer;
		if ( player == null )
			return;

		var inventory = player.Inventory as Inventory;
		if ( inventory == null )
			return;

        if(doSwap){
            doSwap = false;
            EquipSelected(input, player, inventory);
        }

		if ( input.Pressed( InputButton.Slot1 ) ) CycleSlot( input, player, inventory, 1 );
		if ( input.Pressed( InputButton.Slot2 ) ) CycleSlot( input, player, inventory, 2 );
		if ( input.Pressed( InputButton.Slot3 ) ) CycleSlot( input, player, inventory, 3 );
		if ( input.Pressed( InputButton.Slot4 ) ) CycleSlot( input, player, inventory, 4 );
		if ( input.Pressed( InputButton.Slot5 ) ) CycleSlot( input, player, inventory, 5 );
		if ( input.Pressed( InputButton.Slot6 ) ) CycleSlot( input, player, inventory, 6 );
		if ( input.Pressed( InputButton.Slot7 ) ) CycleSlot( input, player, inventory, 7 );
		if ( input.Pressed( InputButton.Slot8 ) ) CycleSlot( input, player, inventory, 8 );
		if ( input.Pressed( InputButton.Slot9 ) ) CycleSlot( input, player, inventory, 9 );
		if ( input.Pressed( InputButton.Slot0 ) ) CycleSlot( input, player, inventory, 0 );

		if ( input.MouseWheel != 0 ) CycleDelta( input, player, inventory, -input.MouseWheel );
	}

    void EquipSelected(InputBuilder input, DoomPlayer ply, Inventory inv){
		var wep = inv.All(activeGroup).ElementAt(subSlot);
		if(ply.ActiveChild == wep)
			return;
        if(ply.ActiveChild is DoomGun oldwep){
            oldwep.weaponStowed = false;
        }

		input.ActiveChild = wep;
        if(ply.ActiveChild is DoomGun newwep){
            newwep.weaponDrawn = false;
        }
	}


    int activeGroup = 1;
	int subSlot = 1;

    void CycleSlot(InputBuilder input, DoomPlayer ply, Inventory inv, int slot){
		int targetCount = inv.All(slot).Count();
		if(targetCount == 0)return;
        if(ply.ActiveChild is DoomGun wep){
            if(wep.State != DoomGun.GUNSTATE_IDLE){
                return;
            }
            wep.weaponStowed = true;
            wep.stowStart = System.Math.Max(0.5f - wep.drawStart, 0);
        }
		if(activeGroup != slot){
			subSlot = 0;
		}else{
			subSlot = subSlot + 1;
			if(subSlot >= targetCount){
				subSlot = 0;
			}
		}
		activeGroup = slot;
		//EquipSelected(input, ply, inv);
	}

	void CycleDelta(InputBuilder input, DoomPlayer ply, Inventory inv, int delta){
		if(inv.Count() == 0)return;
        if(ply.ActiveChild is DoomGun wep){
            if(wep.State != DoomGun.GUNSTATE_IDLE){
                return;
            }
            wep.weaponStowed = true;
            wep.stowStart = System.Math.Max(0.5f - wep.drawStart, 0);
        }
		subSlot = subSlot + delta;
		while(subSlot < 0){
			activeGroup--;
			if(activeGroup < 0)activeGroup = 9;
			subSlot += inv.All(activeGroup).Count();
		}
		while(subSlot >= inv.All(activeGroup).Count()){
			subSlot -= inv.All(activeGroup).Count();
			activeGroup++;
			if(activeGroup > 9)activeGroup = 0;
		}
		//EquipSelected(input, ply, inv);
	}
}

public class ArmsIcon : Panel {
    int Index = 0;
    public ArmsIcon(int index){
        Index = index;
        var iconTex = TextureLoader2.Instance.GetUITexture("STGNUM" + index);
        AddClass("icon"+index);
        Style.BackgroundImage = iconTex;
        Style.Width = iconTex.Width;
        Style.Height = iconTex.Height;
        Style.Dirty();
    }

    public void ResizeChildren(float s){
        var ico = Style.BackgroundImage;
        Style.Width = ico.Width * s;
        Style.Height = ico.Height * s;
        Style.Dirty();
    }
    public override void Tick(){
        if(Local.Pawn is not DoomPlayer ply)return;
        if(ply.Inventory is not Inventory i)return;
        Texture iconTex;
        if(i.All().OfType<Weapon>().Any(c=>c.HoldSlot == Index)){
            iconTex = TextureLoader2.Instance.GetUITexture("STYSNUM" + Index);
        }else{
            iconTex = TextureLoader2.Instance.GetUITexture("STGNUM" + Index);
        }
        Style.BackgroundImage = iconTex;
        Style.Dirty();
    }
}