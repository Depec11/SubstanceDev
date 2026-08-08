using Substance.Linux;

namespace Test.Linux;

internal class Program
{
    public static void Main(string[] args)
    {
        var app = new LinuxApplication();

        app.Exec();
    }
}