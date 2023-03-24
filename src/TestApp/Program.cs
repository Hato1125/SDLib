using SDL2;
using SDLib;
using SDLib.Graphics;
using SDLib.Input;
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
        app.OnEvent += Event;
        app.OnRunning += Draw;
        app.Run();
    }

    private static void Event(SDL.SDL_Event e)
    {
        Mouse.Update(e);
        Keyboard.Update();

    }

    private static void Init(IntPtr renderer)
    {
        var fontFamily = new FontFamily($"{AppContext.BaseDirectory}x12y16pxMaruMonica.ttf", 50);
        font = new(renderer, fontFamily);
        font.Text = "SDLFontTest";
        font.ImageScale = new(1.5f, 0.85f);
    }

    private static void Draw(AppTime time, IntPtr renderer)
    {
        if (Mouse.IsPushed(MouseKey.Left))
        {
            if (font != null)
            {
                font.Text += "A";
                font.FontFamily = new($"{AppContext.BaseDirectory}x12y16pxMaruMonica.ttf", 40, Color.LightGreen);
            }
        }

        font?.Draw(100, 100);
    }
}