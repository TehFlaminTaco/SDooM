using System.Collections.Generic;
using Sandbox;

public class MaterialManager 
{
    public static MaterialManager Instance;

    public Material illegal;
    public Material defaultMaterial;
    public Material alphacutMaterial;
    public Material spriteMaterial;

    public MaterialOverride[] _OverrideWallMaterials = new MaterialOverride[0];
    public MaterialOverride[] _OverrideFlatMaterials = new MaterialOverride[0];

    public Dictionary<string, MaterialOverride> OverrideWallMaterials = new Dictionary<string, MaterialOverride>();
    public Dictionary<string, MaterialOverride> OverrideFlatMaterials = new Dictionary<string, MaterialOverride>();

    void Awake()
    {
        Instance = this;

        foreach (MaterialOverride mo in _OverrideWallMaterials)
            OverrideWallMaterials.Add(mo.overrideName, mo);

        foreach (MaterialOverride mo in _OverrideFlatMaterials)
            OverrideFlatMaterials.Add(mo.overrideName, mo);
    }

    public bool OverridesWall(string textureName, out Material matInstead)
    {
        matInstead = null;
        if (!OverrideWallMaterials.ContainsKey(textureName))
            return false;

        MaterialOverride mo = OverrideWallMaterials[textureName];
        if (mo.material != null)
            matInstead = mo.material;
        else
            matInstead = defaultMaterial;

        /*if (mo.animation.textureFrames.Length > 0)
        {
            TextureAnimation anim = gameObject.AddComponent<TextureAnimation>();
            anim.frames = mo.animation.textureFrames;
            anim.textureType = TextureAnimation.TextureType.Wall;
            anim.frameTime = mo.animation.frameTime;
        }*/

        return true;
    }

    public bool OverridesFlat(string textureName, out Material matInstead)
    {
        matInstead = null;
        if (!OverrideFlatMaterials.ContainsKey(textureName))
            return false;

        MaterialOverride mo = OverrideFlatMaterials[textureName];

        if (mo.material != null)
            matInstead = mo.material;
        else
            matInstead = defaultMaterial;

        /*if (mo.animation.textureFrames.Length > 0)
        {
            TextureAnimation anim = gameObject.AddComponent<TextureAnimation>();
            anim.frames = mo.animation.textureFrames;
            anim.textureType = TextureAnimation.TextureType.Flat;
            anim.frameTime = mo.animation.frameTime;
        }*/

        return true;
    }
}

[System.Serializable]
public struct MaterialOverride
{
    public string overrideName;
    public Material material;
    public MaterialAnimation animation;
    public int layer;
}

[System.Serializable]
public struct MaterialAnimation
{
    public string[] textureFrames;
    public float frameTime;
}