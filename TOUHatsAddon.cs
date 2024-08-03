using HarmonyLib;
using Lotus.Addons;
using TOUHats.Version;
using System.IO;
using System.Reflection;
using VentLib.Logging;

namespace TOUHats;

public class TOUHatsAddon: LotusAddon
{
    public static string RuntimeLocation;
    private Harmony harmony;
    
    public override void Initialize()
    {
        RuntimeLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        StaticLogger.Debug($"PL Runtime Location: {RuntimeLocation}");
        harmony = new Harmony("com.discussions.touhats");
        harmony.PatchAll();
    }

    public override string Name { get; } = "Town of Us Hats";

    public override VentLib.Version.Version Version { get;} = new TOUHatsVersion();
}


