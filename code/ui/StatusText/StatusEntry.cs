using Sandbox.UI.Construct;
using Sandbox.UI;
using Sandbox;

public partial class StatusEntry : Panel
{
    public DoomTextChat Text { get; internal set; }

    public RealTimeSince TimeSinceBorn = 0;

    public StatusEntry()
    {
        Text = AddChild<DoomTextChat>();
    }

    public override void Tick() 
    {
        base.Tick();

        if ( TimeSinceBorn > 10 ) 
        { 
            Delete();
        }
    }
}