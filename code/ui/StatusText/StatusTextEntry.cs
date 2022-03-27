using Sandbox;
using Sandbox.UI;

public class StatusTextEntry : Panel
{
    public DoomTextChat Text { get; internal set; }
    public TextEntry Input {get; internal set;}

    public StatusTextEntry(TextEntry input)
    {
        Text = AddChild<DoomTextChat>();
        Input = input;
    }

    public override void Tick(){
        Text.Text = "> " + Input.Text.Insert(Input.CaretPosition, ((int)(Time.Now*2))%2==1?"|":" ");
    }
}