using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

public partial class TextureAnimator : Entity{
    public enum Mode{
        FLAT, WALL, SPRITE
    }
    public Material Material;
    public Texture[] Textures;
    public int TicksPerFrame;

    public static string[][] _Overrides = new[]{
        // FLATS
        new[]{"NUKAGE1", "NUKAGE2", "NUKAGE3"},
        new[]{"FWATER1", "FWATER2", "FWATER3", "FWATER4"},
        new[]{"SWATER1", "SWATER2", "SWATER3", "SWATER4"},
        new[]{"LAVA1", "LAVA2", "LAVA3", "LAVA4"},
        new[]{"BLOOD1", "BLOOD2", "BLOOD3"},
        new[]{"RROCK05", "RROCK06", "RROCK07", "RROCK08"},
        new[]{"SLIME01", "SLIME02", "SLIME03", "SLIME04"},
        new[]{"SLIME05", "SLIME06", "SLIME07", "SLIME08"},
        new[]{"SLIME09", "SLIME10", "SLIME11", "SLIME12"},
        // WALLS
        new[]{"BLODGR1", "BLODGR2", "BLODGR3", "BLODGR4"},
        new[]{"BLODRIP1", "BLODRIP2", "BLODRIP3", "BLODRIP4"},
        new[]{"FIREBLU1", "FIREBLU2"},
        new[]{"FIRLAV3", "FIRELAVA"},
        new[]{"FIREMAG1", "FIREMAG2", "FIREMAG3"},
        new[]{"FIREWALA", "FIREWALL"},
        new[]{"GSTFONT1", "GSTFONT2", "GSTFONT3"},
        new[]{"ROCKRED1", "ROCKRED2", "ROCKRED3"},
        new[]{"SLADRIP1", "SLADRIP2", "SLADRIP3"},
        new[]{"BFALL1", "BFALL2", "BFALL3", "BFALL4"},
        new[]{"SFALL1", "SFALL2", "SFALL3", "SFALL4"},
        new[]{"WFALL1", "WFALL2", "WFALL3", "WFALL4"},
        new[]{"DBRAIN1", "DBRAIN2", "DBRAIN3", "DBRAIN4"},
        // SPRITES
        new[]{"BON1A0","BON1B0","BON1C0","BON1D0","BON1C0","BON1B0"},
        new[]{"BON2A0","BON2B0","BON2C0","BON2D0","BON2C0","BON2B0"},
    };

    public static void TryGenerateAnimator(Entity parent, Material mat, string name, Mode isFlat, int tpf = 8){
        if(_Overrides.Any(ovr=>ovr.Contains(name))){
            var animator = new TextureAnimator(mat, name, isFlat, tpf);
            animator.Parent = parent;
        }
        var thingAnim = AnimatedThings.animatedThings.Where(c=>name == c.Value.Name + c.Value.Sequence + "0");
        if(thingAnim.Any()){
            var animator = new TextureAnimator(mat, name, isFlat, tpf);
            animator.Parent = parent;
        }
    }

    public TextureAnimator(Material material, string name, Mode isFlat, int tpf){
        Material = material;
        TicksPerFrame = tpf;
        string[] newTextures;
        if(_Overrides.Any(ovr=>ovr.Contains(name))){
            newTextures = _Overrides.First(ovr=>ovr.Contains(name));
        }else{
            var desc = AnimatedThings.animatedThings.Where(c=>name == c.Value.Name + c.Value.Sequence + "0").First();
            newTextures = new string[desc.Value.Sequence.Length];
            int k=0;
            foreach(var c in desc.Value.Sequence){
                newTextures[k++] = desc.Value.Name + c + "0";
            }
        }
        Textures = new Texture[newTextures.Length];
        int i = 0;
        foreach(var tex in newTextures){
            Textures[i++] = isFlat switch{
                Mode.FLAT => TextureLoader2.Instance.GetFlatTexture(tex),
                Mode.WALL => TextureLoader2.Instance.GetWallTexture(tex),
                Mode.SPRITE => TextureLoader2.Instance.GetSpriteTexture(tex),
                _ => throw new ArgumentException("Bad isflat mode!")
            };
        }
    }

    [Event.Tick]
    public void Tick(){
        Material.OverrideTexture("Color", Textures[((int)(Time.Now * (35f/TicksPerFrame)))%Textures.Length]);
    }
}