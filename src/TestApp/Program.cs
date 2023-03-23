using SDL2;
using SDLib;
using SDLib.Graphics;
using SDLib.Input;
using System.Drawing;

namespace TestApp;

internal class Program
{
    private static SDL.SDL_Event sdlEvent;

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
        sdlEvent = e;
    }

    private static void Init(IntPtr renderer)
    {
    }

    private static void Draw(AppTime time, IntPtr renderer)
    {
    }
}