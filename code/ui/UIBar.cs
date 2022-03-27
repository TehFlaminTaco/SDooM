using System.Diagnostics;
using System;
using Sandbox.UI;

public class UIBar : Panel {
    public StatusBar statusBar;
    public UIBar() {
        statusBar = AddChild<StatusBar>();
    }
    
    public void ResizeChildren(float s){
        statusBar.ResizeChildren(s);
    }
}