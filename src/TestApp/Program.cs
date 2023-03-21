using SDLib;

namespace TestApp;

internal class Program
{
    [STAThread]
    static void Main()
    {
        var app = new Application("Test",new(1280, 720));
        app.Run();
    }
}