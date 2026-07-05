using Substance;

namespace Test.Desktop;

internal class Program
{
    public static void Main(string[] args)
    {
        var app = new Application(new Window());

        app.Exec();
    }
}