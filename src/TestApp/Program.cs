using SDLib;
using SDLib.Graphics;
using System.Drawing;

namespace TestApp;

internal class Program
{
    private static FontRenderer? font;

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
        var family = new FontFamily($"{AppContext.BaseDirectory}FOT-OedoKtr.otf", 50, 0, Color.White);
        font = new(renderer, family, "Moji");
    }

    private static void Draw(AppTime time, IntPtr renderer)
    {
        font?.GetTexture()?.Render(200, 200);
    }
}