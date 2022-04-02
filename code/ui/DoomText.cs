using System.Collections.Generic;
using Sandbox.UI;

public class DoomTextBig : Panel {
    public List<BigTextLetter> Letters = new();
    public string _text;
    public string Text {
        get => _text;
        set {
            _text = value;
            RecalculateLetters();
        }
    }
    public DoomTextBig(){

    }
    public DoomTextBig(string text){
        Text = text;
    }
    
    public void RecalculateLetters(){
        foreach(var letter in Letters){
            letter.Delete(false);
        }
        Letters.Clear();
        foreach(char c in Text){
            var letter = new BigTextLetter(c);
            AddChild(letter);
            Letters.Add(letter);
        }
    }
}

public class BigTextLetter : Panel {
    public BigTextLetter(char letter){
        string tex = "STTNUM"+letter;
        if(letter == '%')
            tex = "STTPRCNT";
        if(letter == '-')
            tex = "STTMINUS";
        var letterTex = TextureLoader2.Instance.GetUITexture(tex);
        Style.BackgroundImage = letterTex;
        Style.Width = letterTex.Width * DoomHud.LastHudScale;
        Style.Height = letterTex.Height * DoomHud.LastHudScale;
        Style.Dirty();
    }
}

public class DoomTextAmmo : Panel {
    public List<AmmoTextLetter> Letters = new();
    public string _text;
    public string Text {
        get => _text;
        set {
            _text = value;
            RecalculateLetters();
        }
    }
    public DoomTextAmmo(){

    }
    public DoomTextAmmo(string text){
        Text = text;
    }
    
    public void RecalculateLetters(){
        foreach(var letter in Letters){
            letter.Delete(false);
        }
        Letters.Clear();
        foreach(char c in Text){
            var letter = new AmmoTextLetter(c);
            AddChild(letter);
            Letters.Add(letter);
        }
    }
}

public class AmmoTextLetter : Panel {
    public AmmoTextLetter(char letter){
        string tex = "STYSNUM"+letter;
        var letterTex = TextureLoader2.Instance.GetUITexture(tex);
        Style.BackgroundImage = letterTex;
        Style.Width = letterTex.Width * DoomHud.LastHudScale;
        Style.Height = letterTex.Height * DoomHud.LastHudScale;
        Style.Dirty();
    }
}

public class DoomTextChat : Panel {
    public List<ChatTextLetter> Letters = new();
    public string _text;
    public string Text {
        get => _text;
        set {
            _text = value;
            RecalculateLetters();
        }
    }
    public DoomTextChat(){

    }
    public DoomTextChat(string text){
        Text = text;
    }
    
    public void RecalculateLetters(){
        foreach(var letter in Letters){
            letter.Delete(true);
        }
        Letters.Clear();
        foreach(char c in Text.ToUpper()){
            var letter = new ChatTextLetter(c);
            AddChild(letter);
            Letters.Add(letter);
        }
    }
}

public class ChatTextLetter : Panel {
    public ChatTextLetter(char letter){
        string letterNumber = ""+(int)letter;
        while(letterNumber.Length<3)letterNumber = "0"+letterNumber;
        string tex = "STCFN"+letterNumber;
        if(letter=='|')
            tex = "STCFN121";
        if(letter == ' '){
            Style.Width = 4 * DoomHud.LastTextScale;
            Style.Height = 7 * DoomHud.LastTextScale;
            Style.Dirty();
            return;
        }
        if(TextureLoader2.UITextures.ContainsKey(tex)){
            var letterTex = TextureLoader2.Instance.GetUITexture(tex);
            Style.BackgroundImage = letterTex;
            Style.Width = letterTex.Width * DoomHud.LastTextScale;
            Style.Height = letterTex.Height * DoomHud.LastTextScale;
            if(letter == ',' || letter == '.' || letter == '_')
                Style.AlignSelf = Align.FlexEnd;
            if(letter == '-')
                Style.AlignSelf = Align.Center;
            Style.Dirty();
        }else{
            Delete();
        }
    }
}