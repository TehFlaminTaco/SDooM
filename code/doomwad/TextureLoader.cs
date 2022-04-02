using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox;

public class TextureLoader2
{
    //public FilterMode DefaultFilterMode = FilterMode.Point;

    public static TextureLoader2 Instance;
    public Texture illegal;

    void Awake()
    {
        Instance = this;
        
        for (int i = 0; i < OverrideParatemers.Length; i++)
            _overrideParameters.Add(OverrideParatemers[i].textureName, OverrideParatemers[i]);

        foreach (SpriteOverride so in _OverrideSprites)
            OverrideSprites.Add(so.spriteName, so.sprite);
    }

    [System.Serializable]
    public struct TextureParameters
    {
        public string textureName;
        //public TextureWrapMode horizontalWrapMode;
        //public TextureWrapMode verticalWrapMode;
        //public FilterMode filterMode;
    }

    [System.Serializable]
    public struct SpriteOverride
    {
        public string spriteName;
        public Texture sprite;
    }

    public static Color[] Palette;
    public static List<string> PatchNames = new List<string>();
    public static Dictionary<string, int[,]> Patches = new Dictionary<string, int[,]>();
    public static List<MapTexture> MapTextures = new List<MapTexture>();
    public static Dictionary<string, Texture> WallTextures = new Dictionary<string, Texture>();
    public static Dictionary<string, Texture> FlatTextures = new Dictionary<string, Texture>();
    public static Dictionary<string, Texture> SpriteTextures = new Dictionary<string, Texture>();
    public static Dictionary<string, Texture> UITextures = new Dictionary<string, Texture>();
    public static Dictionary<string, bool> NeedsAlphacut = new Dictionary<string, bool>();
    public Dictionary<string, TextureParameters> _overrideParameters = new Dictionary<string, TextureParameters>();
    public TextureParameters[] OverrideParatemers = new TextureParameters[0];
    public List<SpriteOverride> _OverrideSprites = new List<SpriteOverride>();
    public Dictionary<string, Texture> OverrideSprites = new Dictionary<string, Texture>();
    public static string[] UIElements = {
        "STBAR",
        "STGNUM0","STGNUM1","STGNUM2","STGNUM3","STGNUM4","STGNUM5","STGNUM6","STGNUM7","STGNUM8","STGNUM9",
        "STTNUM0","STTNUM1","STTNUM2","STTNUM3","STTNUM4","STTNUM5","STTNUM6","STTNUM7","STTNUM8","STTNUM9",
        "STTMINUS", "STTPRCNT",
        "STYSNUM0","STYSNUM1","STYSNUM2","STYSNUM3","STYSNUM4","STYSNUM5","STYSNUM6","STYSNUM7","STYSNUM8","STYSNUM9",
        "STCFN033","STCFN034","STCFN035","STCFN036","STCFN037","STCFN038","STCFN039","STCFN040","STCFN041","STCFN042","STCFN043","STCFN044","STCFN045","STCFN046","STCFN047","STCFN048","STCFN049","STCFN050","STCFN051","STCFN052","STCFN053","STCFN054","STCFN055","STCFN056","STCFN057","STCFN058","STCFN059","STCFN060","STCFN061","STCFN062","STCFN063","STCFN064","STCFN065","STCFN066","STCFN067","STCFN068","STCFN069","STCFN070","STCFN071","STCFN072","STCFN073","STCFN074","STCFN075","STCFN076","STCFN077","STCFN078","STCFN079","STCFN080","STCFN081","STCFN082","STCFN083","STCFN084","STCFN085","STCFN086","STCFN087","STCFN088","STCFN089","STCFN090","STCFN091","STCFN092","STCFN093","STCFN094","STCFN095","STCFN096","STCFN097","STCFN098","STCFN099","STCFN100","STCFN101","STCFN102","STCFN103","STCFN104","STCFN105","STCFN106","STCFN107","STCFN108","STCFN109","STCFN110","STCFN111","STCFN112","STCFN113","STCFN114","STCFN115","STCFN116","STCFN117","STCFN118","STCFN119","STCFN120","STCFN121",
        "STKEYS0","STKEYS1","STKEYS2","STKEYS3","STKEYS4","STKEYS5",
        "STDISK","STCDROM","STARMS",
        "STFB0","STFB1","STFB2","STFB3",
        "STPB0","STPB1","STPB2","STPB3",
        "AMMNUM0","AMMNUM1","AMMNUM2","AMMNUM3","AMMNUM4","AMMNUM5","AMMNUM6","AMMNUM7","AMMNUM8","AMMNUM9"
    };
    private static readonly Regex FaceImage = new(@"^STF");

    public class MapTexture
    {
        public string textureName;
        public int masked;
        public int width;
        public int height;
        public int columnDirectory;
        public MapPatch[] patches;
    }

    public class MapPatch
    {
        public short originx;
        public short originy;
        public int number;
        public int stepdir;
        public int colormap;
    }

    public Texture GetWallTexture(string textureName)
    {
        if (WallTextures.ContainsKey(textureName))
            return (WallTextures[textureName]);

        Log.Info("TextureLoader2: No wall texture \"" + textureName +"\"");
        return illegal;
    }

    public Texture GetFlatTexture(string textureName)
    {
        if (FlatTextures.ContainsKey(textureName))
            return (FlatTextures[textureName]);

        Log.Info("TextureLoader2: No flat texture \"" + textureName + "\"");
        return illegal;
    }

    public Texture GetSpriteTexture(string textureName)
    {
        if (OverrideSprites.ContainsKey(textureName))
            return OverrideSprites[textureName];

        if (SpriteTextures.ContainsKey(textureName))
            return (SpriteTextures[textureName]);

        Log.Info("TextureLoader2: No sprite texture \"" + textureName + "\"");
        return illegal;
    }
    public Texture GetUITexture(string textureName)
    {
        if (OverrideSprites.ContainsKey(textureName))
            return OverrideSprites[textureName];

        if (UITextures.ContainsKey(textureName))
            return (UITextures[textureName]);

        Log.Info("TextureLoader2: No UI texture \"" + textureName + "\"");
        return illegal;
    }

    public void LoadAndBuildAll()
    {
        if (LoadPalette())
            if (LoadPatchNames())
            {
                foreach (string p in PatchNames)
                    LoadPatch(p);

                if (LoadMapTextures())
                    BuildWallTextures();

                LoadFlats();
                LoadSprites();
                LoadUI();
            }
    }

    public void LoadSprites()
    {
        bool begin = false;
        foreach (Lump l in WadLoader.lumps)
        {
            if (!begin)
            {
                if (l.lumpName == "S_START")
                    begin = true;

                continue;
            }

            if (l.lumpName == "S_END")
                break;

            int[,] pixelindices = ReadPatchData(l.data);
            int width = pixelindices.GetLength(0);
            int height = pixelindices.GetLength(1);
            Color[] pixels = new Color[height * width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    pixels[y * width + x] = Palette[pixelindices[x, y]];

            Texture tex = Texture.Create(width, height)
                .WithFormat(ImageFormat.RGBA8888)
                .WithData(pixels.SelectMany(c=>c.a<1.0?new byte[]{255,0,255,255}:new byte[]{(byte)(c.r*255), (byte)(c.g*255), (byte)(c.b*255), (byte)(c.a*255)}).ToArray())
            .Finish();
            //tex.SetPixels(pixels);
            if (_overrideParameters.ContainsKey(l.lumpName))
            {
                TextureParameters p = _overrideParameters[l.lumpName];
                //tex.wrapModeU = p.horizontalWrapMode;
                //tex.wrapModeV = p.verticalWrapMode;
                //tex.filterMode = p.filterMode;
            }
            else
            {
                //tex.wrapMode = TextureWrapMode.Clamp;
                //tex.filterMode = DefaultFilterMode;
            }
            //tex.Apply();
            SpriteTextures[l.lumpName]=tex;
        }
    }

    public void LoadUI()
    {
        foreach (Lump l in WadLoader.lumps)
        {
            if(!FaceImage.IsMatch(l.lumpName) && !UIElements.Contains(l.lumpName))
                continue;
            int[,] pixelindices = ReadPatchData(l.data);
            int width = pixelindices.GetLength(0);
            int height = pixelindices.GetLength(1);
            Color[] pixels = new Color[height * width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    pixels[(height-y-1) * width + x] = Palette[pixelindices[x, y]];

            Texture tex = Texture.Create(width, height)
                .WithFormat(ImageFormat.RGBA8888)
                .WithData(pixels.SelectMany(c=>new byte[]{(byte)(c.r*255), (byte)(c.g*255), (byte)(c.b*255), (byte)(c.a*255)}).ToArray())
            .Finish();
            //tex.SetPixels(pixels);
            if (_overrideParameters.ContainsKey(l.lumpName))
            {
                TextureParameters p = _overrideParameters[l.lumpName];
                //tex.wrapModeU = p.horizontalWrapMode;
                //tex.wrapModeV = p.verticalWrapMode;
                //tex.filterMode = p.filterMode;
            }
            else
            {
                //tex.wrapMode = TextureWrapMode.Clamp;
                //tex.filterMode = DefaultFilterMode;
            }
            //tex.Apply();
            UITextures[l.lumpName]=tex;
        }
    }

    public void BuildWallTextures()
    {
        foreach (MapTexture t in MapTextures)
        {
            Color[] pixels = new Color[t.width * t.height];

            for (int y = 0; y < t.height; y++)
                for (int x = 0; x < t.width; x++)
                    pixels[y * t.width + x] = Palette[256];

            foreach (MapPatch p in t.patches)
            {
                if (p.number >= PatchNames.Count)
                {
                    Log.Error("TextureLoader2: BuildTextures: Patch number out of range, in texture \"" + t.textureName + "\"");
                    continue;
                }

                string patchName = PatchNames[p.number];
                if (!Patches.ContainsKey(patchName))
                {
                    Log.Error("TextureLoader2: BuildTextures: Could not find patch \"" + patchName + "\"");
                    continue;
                }

                int[,] patch = Patches[patchName];

                int pheight = patch.GetLength(1);
                for (int y = 0; y < pheight; y++)
                    for (int x = 0; x < patch.GetLength(0); x++)
                    {
                        int py = pheight - y - 1;

                        if (patch[x, py] == 256)
                            continue;

                        int oy = p.originy + y;
                        int ox = p.originx + x;

                        if (ox >= 0 && ox < t.width && oy >= 0 && oy < t.height)
                            pixels[(t.height - oy - 1) * t.width + ox] = Palette[patch[x, py]];
                    }
            }
            bool alphaCut = false;
            for (int y = 0; y < t.height; y++)
                for (int x = 0; x < t.width; x++)
                    if (pixels[y * t.width + x].a < 1f){
                        pixels[y * t.width + x] = new Color(1,0,1,0);
                        alphaCut = true;
                    }
            Texture tex = Texture.Create(t.width, t.height)
                .WithFormat(ImageFormat.RGBA8888)
                .WithData(pixels.SelectMany(c=>c.a<1.0?new byte[]{255,0,255,255}:new byte[]{(byte)(c.r*255), (byte)(c.g*255), (byte)(c.b*255), (byte)(c.a*255)}).ToArray())
            .Finish();
            //tex.SetPixels(pixels);
            if (_overrideParameters.ContainsKey(t.textureName))
            {
                TextureParameters p = _overrideParameters[t.textureName];
                //tex.wrapModeU = p.horizontalWrapMode;
                //tex.wrapModeV = p.verticalWrapMode;
                //tex.filterMode = p.filterMode;
            }
            else
            {
                //tex.wrapMode = TextureWrapMode.Repeat;
                //tex.filterMode = DefaultFilterMode;
            }
            //tex.Apply();
            WallTextures[t.textureName]=tex;
            NeedsAlphacut[t.textureName]=alphaCut;
        }
    }

    public bool LoadMapTextures()
    {
        foreach (Lump l in WadLoader.lumps)
            if (l.lumpName == "TEXTURE1")
            {
                int p = 0;
                int num = (int)(l.data[p++] | (int)l.data[p++] << 8 | (int)l.data[p++] << 16 | (int)l.data[p++] << 24);

                int[] offsets = new int[num];
                for (int i = 0; i < num; i++)
                    offsets[i] = (int)(l.data[p++] | (int)l.data[p++] << 8 | (int)l.data[p++] << 16 | (int)l.data[p++] << 24);

                MapTextures.Clear();
                for (int i = 0; i < num; i++)
                {
                    p = offsets[i];
                    MapTexture t = new MapTexture();

                    t.textureName = Encoding.ASCII.GetString(new byte[]
                    {
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++]
                    }).TrimEnd('\0').ToUpper();

                    t.masked = (int)(l.data[p++] | (int)l.data[p++] << 8 | (int)l.data[p++] << 16 | (int)l.data[p++] << 24);
                    t.width = (int)(l.data[p++] | (int)l.data[p++] << 8);
                    t.height = (int)(l.data[p++] | (int)l.data[p++] << 8);
                    t.columnDirectory = (int)(l.data[p++] | (int)l.data[p++] << 8 | (int)l.data[p++] << 16 | (int)l.data[p++] << 24);
                    int patchCount = (int)(l.data[p++] | (int)l.data[p++] << 8);
                    t.patches = new MapPatch[patchCount];

                    for (int j = 0; j < patchCount; j++)
                    {
                        MapPatch patch = new MapPatch();

                        patch.originx = (short)(l.data[p++] | (short)l.data[p++] << 8);
                        patch.originy = (short)(l.data[p++] | (short)l.data[p++] << 8);
                        patch.number = (int)(l.data[p++] | (int)l.data[p++] << 8);
                        patch.stepdir = (int)(l.data[p++] | (int)l.data[p++] << 8);
                        patch.colormap = (int)(l.data[p++] | (int)l.data[p++] << 8);

                        t.patches[j] = patch;
                    }

                    MapTextures.Add(t);
                }

                return true;
            }

        return false;
    }

    private static Regex FLAT_START = new(@"F\d+_START");
    private static Regex FLAT_END = new(@"F\d+_END");
    public void LoadFlats()
    {
        bool begin = false;
        foreach (Lump l in WadLoader.lumps)
        {
            if (!begin)
            {
                if (FLAT_START.IsMatch(l.lumpName))
                    begin = true;

                continue;
            }

            if (FLAT_END.IsMatch(l.lumpName)){
                begin = false;
                continue;
            }

            Color[] pixels = new Color[64 * 64];

            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                    pixels[y * 64 + x] = Palette[l.data[y * 64 + x]];

            Texture tex = Texture.Create(64, 64)
                .WithFormat(ImageFormat.RGBA8888)
                .WithData(pixels.SelectMany(c=>new byte[]{(byte)(c.r*255), (byte)(c.g*255), (byte)(c.b*255), (byte)(c.a*255)}).ToArray())
            .Finish();
            /*Texture2D tex = new Texture2D(64, 64);
            tex.SetPixels(pixels);
            tex.filterMode = DefaultFilterMode;
            tex.Apply();*/
            FlatTextures[l.lumpName]=tex;
        }
    }

    public bool LoadPalette()
    {
        foreach (Lump l in WadLoader.lumps)
            if (l.lumpName == "PLAYPAL")
            {
                Palette = new Color[257];
                for (int i = 0; i < 256; i++)
                    Palette[i] = new Color((float)l.data[i * 3] / 255f, (float)l.data[i * 3 + 1] / 255f, (float)l.data[i * 3 + 2] / 255f, 1f);

                Palette[256] = new Color(1.0f, 0, 1.0f, 0);

                return true;
            }

        return false;
    }

    public bool LoadPatchNames()
    {
        foreach (Lump l in WadLoader.lumps)
            if (l.lumpName == "PNAMES")
            {
                PatchNames.Clear();

                int p = 0;
                int num = (int)(l.data[p++] | (int)l.data[p++] << 8 | (int)l.data[p++] << 16 | (int)l.data[p++] << 24);

                for (int i = 0; i < num; i++)
                {
                    PatchNames.Add(Encoding.ASCII.GetString(new byte[]
                    {
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++],
                        l.data[p++]
                    }).TrimEnd('\0').ToUpper());
                }

                return true;
            }

        return false;
    }

    public int[,] LoadPatch(string patchName)
    {
        if (Patches.ContainsKey(patchName))
            return Patches[patchName];

        foreach (Lump l in WadLoader.lumps)
            if (l.lumpName == patchName)
            {
                int[,] pixels = ReadPatchData(l.data);
                Patches.Add(patchName, pixels);
                return pixels;
            }

        //suppressed this as it seems shareware Doom WAD contains PNAMES for full version patchpool or something
        //Log.Error("TextureLoader2: LoadPatch: Could not find patch \"" + patchName + "\"");
        return null;
    }

    public int[,] ReadPatchData(byte[] data)
    {
        int p = 0;
        int width = (int)(data[p++] | (int)data[p++] << 8);
        int height = (int)(data[p++] | (int)data[p++] << 8);
        int left = (int)(data[p++] | (int)data[p++] << 8);
        int top = (int)(data[p++] | (int)data[p++] << 8);

        if (left > 0) { }
        if (top > 0) { }

        int[,] pixels = new int[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[x, y] = 256;

        int[] columns = new int[width];
        for (int x = 0; x < width; x++)
        {
            columns[x] =
                (int)(data[p++] |
                (int)data[p++] << 8 |
                (int)data[p++] << 16 |
                (int)data[p++] << 24);
        }

        for (int x = 0; x < width; x++)
        {
            p = columns[x];

            while (true)
            {
                int offset = data[p++];
                if (offset == byte.MaxValue)
                    break;

                int length = data[p++];
                p++; //dummy byte
                for (int y = 0; y < length; y++)
                    pixels[x, height - (offset + y + 1)] = data[p++];
                p++; //dummy byte
            }
        }

        return pixels;
    }
}
