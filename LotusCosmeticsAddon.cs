using HarmonyLib;
using Lotus.Addons;
using LotusCosmetics.Version;
using System.IO;
using System.Reflection;
using VentLib.Logging;

namespace LotusCosmetics;

public class LotusCosmeticsAdon: LotusAddon
{
    public static string RuntimeLocation;
    private Harmony harmony;
    
    public override void Initialize()
    {
        RuntimeLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        #if DEBUG
        StaticLogger.Debug($"PL Runtime Location: {RuntimeLocation}");
        #endif
        harmony = new Harmony("com.discussions.lotuscosmetics");
        harmony.PatchAll();

        FileInfo bundleInfo = new(Path.Combine(RuntimeLocation, "touhats.bundle"));
        if (bundleInfo.Exists)
        {
            bundleInfo.Delete();
            StaticLogger.Info("Deleted touhats.bundle.");
        }
        FileInfo catalogInfo = new(Path.Combine(RuntimeLocation, "touhats.catalog"));
        if (catalogInfo.Exists)
        {
            catalogInfo.Delete();
            StaticLogger.Info("Deleted touhats.catalog.");
        }
    }

    public override string Name { get; } = "Project Lotus Cosmetics";

    public override VentLib.Version.Version Version { get;} = new LotusCosmeticsVersion();
}


