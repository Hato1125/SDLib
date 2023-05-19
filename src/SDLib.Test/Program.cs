namespace SDLib.Test;

internal class Program
{
    public static readonly App App = new();

    [STAThread]
    private static void Main()
    {
        App.Run();
    }
}