using Substance.Linux;

namespace Test.Linux;

internal class Program
{
    public static void Main(string[] args)
    {
        var app = new LinuxApplication();
        var gameLoop = new MainGameLoop();

        gameLoop.Dispose();

        app.Initialize();

        app.Exec();

        gameLoop.Dispose();
        app.Dispose();
    }
}