using Substance.Windows;

namespace Test.Windows;

internal class Program
{
    public static void Main(string[] args)
    {
        var app = new WindowsApplication();

        app.Exec();
    }
}