using System.Runtime.CompilerServices;
using System.Text;

namespace Substance.Logging;

public class SimpleLogger : ILogger, IDisposable
{    
    public string FilePath { get; init; } = "engine.log";
    public LogLevel ConsoleLogLevel { get; set; }
    public LogLevel FileLogLevel { get; set; }

    private readonly StreamWriter? _writer;
    private readonly Lock _lock = new();
    private bool _disposed;

    public SimpleLogger(LogLevel consoleLogLevel = LogLevel.Info, LogLevel fileLogLevel = LogLevel.Debug, string? filePath = null) 
    {
        ConsoleLogLevel = consoleLogLevel;
        FileLogLevel = fileLogLevel;

        if (filePath != null) 
        {
            FilePath = filePath;
        }

        string fullPath;

        if (OperatingSystem.IsAndroid())
        {
            fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FilePath);
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsWindows())
        {
            fullPath = Path.GetFullPath(FilePath);
        }
        else
        {
            return;
        }

        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _writer = new StreamWriter(fullPath, append: false, Encoding.UTF8) 
        {
            AutoFlush = true
        };
    }

    ~SimpleLogger() {
        Dispose();
    }

    private void Write(LogLevel level, string msg) 
    {
        if (level < ConsoleLogLevel && level < FileLogLevel) 
        {
            return;
        }

        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var line = $"{timestamp} [{level}] {msg}";

        if (ConsoleLogLevel <= level)
        {
            if (!OperatingSystem.IsAndroid() && (OperatingSystem.IsLinux() || OperatingSystem.IsWindows()))
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = level switch {
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Debug => ConsoleColor.Gray,
                    LogLevel.Info => ConsoleColor.Green,
                    _ => ConsoleColor.White
                };
                Console.WriteLine(line);
                Console.ForegroundColor = originalColor;
            }
            else
            {
                Console.WriteLine(line);
            }
        }

        if (FileLogLevel <= level)
        {
            lock (_lock) {
                _writer?.WriteLine(line);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(string msg) => Write(LogLevel.Debug, msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(string msg) => Write(LogLevel.Info, msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(string msg) => Write(LogLevel.Warning, msg);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string msg) => Write(LogLevel.Error, msg);

    public void Flush() {
        lock (_lock) 
        {
            _writer?.Flush();
        }
    }

    public void Dispose() {
        if (_disposed) 
        {
            return;
        }

        Flush();

        _writer?.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}