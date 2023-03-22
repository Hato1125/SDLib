using SDLib;
using SDLib.Graphics;
using System.Drawing;

namespace TestApp;

internal class Program
{
    [STAThread]
    static void Main()
    {
        var app = new Application("Test", new(1280, 720));
        
        app.OnInit += Init;
        app.OnRunning += Draw;
        app.Run();
    }

    private static void Init(IntPtr renderer)
    {
    }

    private static void Draw(AppTime time, IntPtr renderer)
    {
    }
}