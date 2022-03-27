using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class StatusText : Panel {
    static StatusText Current;

    public Panel Canvas { get; protected set; }
    public TextEntry Input { get; protected set; }
    public StatusTextEntry StatusEntry { get; protected set; }

    public StatusText()
    {
        Current = this;

        StyleSheet.Load( "/ui/chat/ChatBox.scss" );


        Input = Add.TextEntry( "" );
        Input.AddEventListener( "onsubmit", () => Submit() );
        Input.AddEventListener( "onblur", () => Close() );
        Input.AcceptsFocus = true;
        Input.AllowEmojiReplace = false;

        StatusEntry = new(Input);
        AddChild( StatusEntry );

        Canvas = Add.Panel( "chat_canvas" );

        Sandbox.Hooks.Chat.OnOpenChat += Open;
    }

    void Open()
    {
        StatusEntry.AddClass( "open" );
        Input.Focus();
    }

    void Close()
    {
        StatusEntry.RemoveClass( "open" );
        Input.Blur();
    }

    void Submit()
    {
        Close();

        var msg = Input.Text.Trim();
        Input.Text = "";

        if ( string.IsNullOrWhiteSpace( msg ) )
            return;

        Say( msg );
    }

    public void AddEntry( string name, string message, string avatar, string lobbyState = null )
    {
        var e = Canvas.AddChild<StatusEntry>();

        if(string.IsNullOrEmpty(name))
            e.Text.Text = message;
        else
            e.Text.Text = $"{name}: {message}";

        if ( lobbyState == "ready" || lobbyState == "staging" )
        {
            e.SetClass( "is-lobby", true );
        }
    }


    [ClientCmd( "chat_add", CanBeCalledFromServer = true )]
    public static void AddChatEntry( string name, string message, string avatar = null, string lobbyState = null )
    {
        Current?.AddEntry( name, message, avatar, lobbyState );

        // Only log clientside if we're not the listen server host
        if ( !Global.IsListenServer )
        {
            Log.Info( $"{name}: {message}" );
        }
    }

    [ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
    public static void AddInformation( string message, string avatar = null )
    {
        Current?.AddEntry( null, message, avatar );
    }

    [ServerCmd( "say" )]
    public static void Say( string message )
    {
        Assert.NotNull( ConsoleSystem.Caller );

        // todo - reject more stuff
        if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
            return;

        Log.Info( $"{ConsoleSystem.Caller}: {message}" );
        AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
    }
}