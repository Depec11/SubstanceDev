using Substance.Desktop;

namespace Test.Desktop;

internal class Program
{
    public static void Main(string[] args)
    {
        var app = new DesktopApplication();

        app.Exec();
    }
}