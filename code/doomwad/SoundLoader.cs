using System;
using System.Linq;
using Sandbox;
public partial class SoundLoader 
{
    //work in progress, but shows how to load sound from WAD

    [ClientRpc]
    public static void PlaySound(string soundName, Vector3 pos){
        PlaySoundClientside(soundName, pos);
    }
    public static (Sound?, SoundStream?) PlaySoundClientside(string soundName, Vector3 pos)
    {
        foreach (Lump l in WadLoader.lumps)
        {
            if (l.lumpName != soundName)
                continue;

            int p = 0;
            int format = l.data[p++] | ((int)l.data[p++]) << 8;
            if (format != 3)
            {
                Log.Info("SoundLoader: LoadSound: \"" + soundName + "\" format != 3 (" + format + ")");
                return (null, null);
            }

            int samplerate = l.data[p++] | ((int)l.data[p++]) << 8;
            int count = (int)(l.data[p++] | (int)l.data[p++] << 8 | (int)l.data[p++] << 16 | (int)l.data[p++] << 24);
            count -= 32; //sound lumps have 16 bytes before and after samples as padding

            p += 16; //padding
            short[] samples = new short[count];
            for (int i = 0; i < count; i++){
                sbyte d = (sbyte)(l.data[p++] - 128);
                samples[i] = (short)(d<<6);
            }
            //var strm = Sound.FromScreen("").CreateStream(samplerate, 1);
            var snd = Sound.FromWorld("audiostreamlong", pos);
            try{
                var strm = snd.CreateStream(samplerate, 1);
                strm.WriteData(samples);
                return (snd, strm);
            }catch(Exception){// There's a race condition here...
                return (null,null);
            }
        }

        Log.Info("SoundLoader: LoadSound: Could not find sound \"" + soundName + "\"");
        return (null, null);
    }
}
