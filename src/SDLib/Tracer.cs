namespace SDLib;

public static class Tracer
{
    /// <summary>
    /// 情報出力の際のヘッダー
    /// </summary>
    public static string InfoHeader { get; set; } = "[ Info ] ";

    /// <summary>
    /// 注意出力の際のヘッダー
    /// </summary>
    public static string WarningHeader { get; set; } = "[ Warning ] ";

    /// <summary>
    /// エラー出力の際のヘッダー
    /// </summary>
    public static string ErrorHeader { get; set; } = "[ Error ] ";

    /// <summary>
    /// 情報出力の際の文字色
    /// </summary>
    public static ConsoleColor InfoColor { get; set; } = ConsoleColor.Blue;

    /// <summary>
    /// 注意出力の際の文字色
    /// </summary>
    public static ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;

    /// <summary>
    /// エラー出力の際の文字色
    /// </summary>
    public static ConsoleColor ErrorColor { get; set; } = ConsoleColor.DarkRed;

    /// <summary>
    /// 情報を出力する
    /// </summary>
    /// <param name="message">メッセージ</param>
    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = InfoColor;
        Console.WriteLine($"{InfoHeader}{message}");
        Console.ResetColor();
    }

    /// <summary>
    /// 注意を出力する
    /// </summary>
    /// <param name="message">メッセージ</param>
    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = WarningColor;
        Console.WriteLine($"{WarningHeader}{message}");
        Console.ResetColor();
    }

    /// <summary>
    /// エラーを出力する
    /// </summary>
    /// <param name="message">メッセージ</param>
    public static void PrintError(string message)
    {
        Console.ForegroundColor = ErrorColor;
        Console.WriteLine($"{ErrorHeader}{message}");
        Console.ResetColor();
    }
}