using System.Reflection;

namespace Substance;

public static class Assets
{
    private static readonly Assembly[] _assemblies;

    static Assets()
    {
        _assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    public static bool Exists(Uri uri)
    {
        return Exists(uri, out _, out _);
    }

    public static Stream? Open(Uri uri)
    {
        if (!Exists(uri, out var assembly, out var resourcePath))
        {
            return null;
        }

        return assembly is null ? File.OpenRead(resourcePath) : assembly.GetManifestResourceStream(resourcePath);
    }

    private static void Parse(Uri uri, out string scheme, out string host, out string path)
    {
        scheme = uri.Scheme;
        host = uri.Host;
        path = uri.AbsolutePath[1..];
    }

    private static bool Exists(Uri uri, out Assembly? assembly, out string resourcePath)
    {
        Parse(uri, out var scheme, out var host, out var path);
        
        assembly = null;
        resourcePath = path;

        Console.WriteLine($"Scheme: {scheme}, Host: {host}, Path: {path}");
        
        if (!scheme.Equals("assets", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
    
        if (host.Equals("Local", StringComparison.OrdinalIgnoreCase))
        {
            return Path.Exists(path);
        }

        assembly = _assemblies.FirstOrDefault(
            a => host.Equals(a.GetName().Name, StringComparison.OrdinalIgnoreCase)
        );

        if (assembly is null)
        {
            return false;
        }

        resourcePath = assembly.GetName().Name + "/" + path;

        return assembly.GetManifestResourceNames().Contains(resourcePath);
    }
}