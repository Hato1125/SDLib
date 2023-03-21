using SDLib;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new Application(
                "Test",
                new System.Drawing.Size(100, 100)
            );
            app.OnRunning += Draw;
            app.Run();
        }

        private static void Draw(AppTime time, IntPtr renderer)
        {
        }
    }
}