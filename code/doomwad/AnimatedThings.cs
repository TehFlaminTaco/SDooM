using System.Collections.Generic;
public class AnimatedThings {
    public static Dictionary<int, Decoration> animatedThings = new Dictionary<int, Decoration>(){
        [47] = new Decoration(16,16,"SMIT","A","O"),
        [70] = new Decoration(16,16,"FCAN","ABC","O"),
        [43] = new Decoration(16,16,"TRE1","A","O"),
        [35] = new Decoration(16,16,"CBRA","A","O"),
        [41] = new Decoration(16,16,"CEYE","ABCB","O"),
        //[2035] = new Decoration(10,42,"BAR1","AB","O*"),
        [28] = new Decoration(16,16,"POL2","A","O"),
        [42] = new Decoration(16,16,"FSKU","ABC","O"),
        [2028] = new Decoration(16,16,"COLU","A","O"),
        [53] = new Decoration(16,52,"GOR5","A","O^"),
        [52] = new Decoration(16,68,"GOR4","A","O^"),
        [78] = new Decoration(16,64,"HDB6","A","O^"),
        [75] = new Decoration(16,64,"HDB3","A","O^"),
        [77] = new Decoration(16,64,"HDB5","A","O^"),
        [76] = new Decoration(16,64,"HDB4","A","O^"),
        [50] = new Decoration(16,84,"GOR2","A","O^"),
        [74] = new Decoration(16,88,"HDB2","A","O^"),
        [73] = new Decoration(16,88,"HDB1","A","O^"),
        [51] = new Decoration(16,84,"GOR3","A","O^"),
        [49] = new Decoration(16,68,"GOR1","ABCB","O^"),
        [25] = new Decoration(16,16,"POL1","A","O"),
        [54] = new Decoration(32,16,"TRE2","A","O"),
        [29] = new Decoration(16,16,"POL3","AB","O"),
        [55] = new Decoration(16,16,"SMBT","ABCD","O"),
        [56] = new Decoration(16,16,"SMGT","ABCD","O"),
        [31] = new Decoration(16,16,"COL2","A","O"),
        [36] = new Decoration(16,16,"COL5","AB","O"),
        [57] = new Decoration(16,16,"SMRT","ABCD","O"),
        [33] = new Decoration(16,16,"COL4","A","O"),
        [37] = new Decoration(16,16,"COL6","A","O"),
        [86] = new Decoration(16,16,"TLP2","ABCD","O"),
        [27] = new Decoration(16,16,"POL4","A","O"),
        [44] = new Decoration(16,16,"TBLU","ABCD","O"),
        [45] = new Decoration(16,16,"TGRN","ABCD","O"),
        [30] = new Decoration(16,16,"COL1","A","O"),
        [46] = new Decoration(16,16,"TRED","ABCD","O"),
        [32] = new Decoration(16,16,"COL3","A","O"),
        [48] = new Decoration(16,16,"ELEC","A","O"),
        [85] = new Decoration(16,16,"TLMP","ABCD","O"),
        [26] = new Decoration(16,16,"POL6","AB","O"),

        [10] = new Decoration(20,16,"PLAY","W",""),
        [12] = new Decoration(20,16,"PLAY","W",""),
        [34] = new Decoration(20,16,"CAND","A",""),
        [22] = new Decoration(20,16,"HEAD","L",""),
        [21] = new Decoration(20,16,"SARG","N",""),
        [18] = new Decoration(20,16,"POSS","L",""),
        [19] = new Decoration(20,16,"SPOS","L",""),
        [20] = new Decoration(20,16,"TROO","M",""),
        [23] = new Decoration(20,16,"SKUL","K",""),
        [15] = new Decoration(20,16,"PLAY","N",""),
        [62] = new Decoration(20,52,"GOR5","A","^"),
        [60] = new Decoration(20,68,"GOR4","A","^"),
        [59] = new Decoration(20,84,"GOR2","A","^"),
        [61] = new Decoration(20,52,"GOR3","A","^"),
        [63] = new Decoration(20,68,"GOR1","ABCB","^"),
        [79] = new Decoration(20,16,"POB1","A",""),
        [80] = new Decoration(20,16,"POB2","A",""),
        [24] = new Decoration(20,16,"POL5","A",""),
        [81] = new Decoration(20,16,"BRS1","A",""),
    };

}

public struct Decoration
{
    public int Width;
    public int Height;
    public string Name;
    public string Sequence;
    public string Class;
    public Decoration(int width, int height, string name, string sequence, string className)
    {
        Width = width;
        Height = height;
        Name = name;
        Sequence = sequence;
        Class = className;
    }
}