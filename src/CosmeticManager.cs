using System.Reflection;
using System.IO;
using Lotus.Managers;
using VentLib.Utilities.Extensions;

namespace LotusCosmetics;

public static class CosmeticManager
{
    private static string LotusBundleFolderPath =>
        Path.Combine(PluginDataManager.HiddenDataDirectory.FullName, "LotusCosmetics");

    private static DirectoryInfo LotusBundleFolder => new(LotusBundleFolderPath);

    private static bool _initialized = false;

    public static void LoadCosmetics()
    {
        if (_initialized) return;
        _initialized = true;

        if (!LotusBundleFolder.Exists)
            LotusBundleFolder.Create();

        FileInfo bundle = LotusBundleFolder.GetFile("LotusCosmetics.ccb");

        if (!bundle.Exists)
        {
            Assembly assembly = typeof(CosmeticManager).Assembly;

            using Stream? resource = assembly.GetManifestResourceStream("LotusCosmetics.LotusCosmetics.ccb");

            if (resource == null)
                throw new FileNotFoundException(
                    "Could not find embedded resource 'LotusCosmetics.ccb'."
                );

            using FileStream output = bundle.Create();

            resource.CopyTo(output);
        }
        
        CorsacCosmetics.PluginCompat.AddBundleSource(bundle.FullName);
    }
}