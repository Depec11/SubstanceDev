using Substance.Windows;

namespace Test.Windows;

internal class Program
{
    public static void Main(string[] args)
    {
        var app = new WindowsApplication();
        var gameLoop = new MainGameLoop();

        app.Initialize();

        app.Exec();

        gameLoop.Dispose();
        app.Dispose();
    }
}