using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using Sandbox;

public class WadLoader 
{
    public static WadLoader Instance;

    void Awake()
    {
        Instance = this;
    }

    public Color ambientLightColor = Color.White;
    public bool deathmatch;
    public static List<Lump> lumps = new List<Lump>();
    public string autoloadWad = "doom2.wad";
    public string autoloadMap = "MAP01";
    //public GameObject PlayerObject;

    public static short zOff = 256;
    public static Vector3 mapScale = new Vector3(1.0f, 1.0f, 72f/56f);
    void Start()
    {
        //Shader.SetGlobalColor("_AMBIENTLIGHT", ambientLightColor);

        if (string.IsNullOrEmpty(autoloadWad))
            return;

        if (!Load(autoloadWad))
            return;

        //TextureLoader.Instance.LoadAndBuildAll();

        if (!string.IsNullOrEmpty(autoloadMap))
            if (MapLoader.Instance.Load(autoloadMap))
            {
                /*Mesher.Instance.CreateMeshes();

                MapLoader.Instance.ApplyLinedefBehavior();

                ThingManager.Instance.CreateThings(deathmatch);

                if (PlayerStart.PlayerStarts[0] == null)
                    Log.Error("PlayerStart1 == null");
                else
                {
                    PlayerObject.transform.position = PlayerStart.PlayerStarts[0].transform.position;
                    PlayerObject.transform.rotation = PlayerStart.PlayerStarts[0].transform.rotation;
                }*/

                //PlayerObject.GetComponent<AudioSource>().clip = SoundLoader.LoadSound("DSPISTOL");
                //PlayerObject.GetComponent<AudioSource>().Play();
            }
    }

    public bool Load(string file)
    {
        /*string path = Application.streamingAssetsPath + "/" + file;
        if (!File.Exists(path))
        {
            Log.Error("WadLoader: Load: File \"" + file + "\" does not exist!");
            return false;
        }*/
        byte[] wadData;
        if(file == "SHAREDOOM.WAD"){
            wadData = FileSystem.Mounted.ReadAllBytes("include/sharedoom.json").ToArray();
        }else{
            wadData = FileSystem.Data.ReadAllBytes(file).ToArray();
        }

        //FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        MemoryStream stream  = new MemoryStream(wadData);
        BinaryReader reader = new BinaryReader(stream, Encoding.ASCII);

        if (file.Length < 4)
        {
            reader.Close();
            stream.Close();
            Log.Error("WadLoader: Load: WAD length < 4!");
            return false;
        }

        try
        {
            stream.Seek(0, SeekOrigin.Begin);

            bool isiwad = (Encoding.ASCII.GetString(reader.ReadBytes(4)) == "IWAD"); //other option is "PWAD"
            if (isiwad) { }

            int numlumps = reader.ReadInt32();
            int lumpsoffset = reader.ReadInt32();

            stream.Seek(lumpsoffset, SeekOrigin.Begin);

            lumps.Clear();

            for (int i = 0; i < numlumps; i++)
            {
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32();
                string name = Encoding.ASCII.GetString(reader.ReadBytes(8)).TrimEnd('\0').ToUpper();
                lumps.Add(new Lump(offset, length, name));
            }

            //load the whole wad into memory
            long bytes = 0;
            foreach(Lump l in lumps)
            {
                l.data = new byte[l.length];
                stream.Seek(l.offset, SeekOrigin.Begin);
                stream.Read(l.data, 0, l.length);
                bytes += l.length;
            }

            Log.Info("Loaded WAD \"" + file + "\" (" + bytes + " bytes in lumps)");
        }
        catch(Exception e)
        {
            Log.Error("WadLoader: Load: Reader exception!");
            Log.Error(e);

            reader.Close();
            stream.Close();
            return false;
        }

        reader.Close();
        stream.Close();
        return true;
    }
}
