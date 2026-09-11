using System.Reflection;
using System.IO;

namespace LotusCosmetics;

public static class CosmeticManager
{
    private static bool _initialized = false;

    public static void LoadCosmetics()
    {
        if (_initialized) return;
        _initialized = true;
        
        Assembly assembly = typeof(CosmeticManager).Assembly;

        using Stream? resource = assembly.GetManifestResourceStream("LotusCosmetics.LotusCosmetics.ccb");
        if (resource == null) throw new FileNotFoundException("Could not find embedded resource 'LotusCosmetics.ccb'.");
        
        using var memoryStream = new MemoryStream();
        resource.CopyTo(memoryStream);
        CorsacCosmetics.PluginCompat.AddBundleBytes(memoryStream.ToArray());
    }
}