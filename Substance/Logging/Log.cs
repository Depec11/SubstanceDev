using System.Runtime.CompilerServices;
using System.Text;

namespace Substance.Logging;

public static class Log 
{
    private static readonly SimpleLogger s_logger;

    static Log() {
        if (!OperatingSystem.IsAndroid())
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
        }
        

        s_logger = new SimpleLogger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(string msg) => s_logger.Debug(msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string msg) => s_logger.Info(msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warning(string msg) => s_logger.Warning(msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string msg) => s_logger.Error(msg);
}