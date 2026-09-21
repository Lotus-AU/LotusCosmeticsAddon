using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Lotus;
using Lotus.Addons;
using VentLib;
using VentLib.Logging;
using LogLevel = VentLib.Logging.LogLevel;

namespace LotusCosmetics.Bootstrapper;

[BepInAutoPlugin("com.lotusau.LotusCosmeticsBootstrapper", "LotusCosmeticsBootstrapper", "1.8.0")]
[BepInDependency(Vents.Id)]
[BepInDependency(ProjectLotus.Id)]
public partial class LotusCosmeticPlugin : BasePlugin
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LotusCosmeticPlugin));
    public static LogLevel BootstrapperLogLevel = LogLevel.Info.Similar("BOOTSTRAP", ConsoleColor.Cyan);
    private Harmony _harmony = new("com.lotusau.LotusCosmeticsBootstrapper");
    
    public override void Load()
    {
        BootLog("Loading LotusCosmeticPlugin");
        if (AddonManager.Addons.Any(a => a.GetType().Assembly.GetName().Name == "LotusCosmetics"))
        {
            BootLog("LotusCosmetics already loaded. Not continuing.");
            return;
        }
        
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
 
        using var stream = typeof(LotusCosmeticPlugin).Assembly
            .GetManifestResourceStream("LotusCosmetics.Bootstrapper.Resources.LotusCosmetics.dll");
        using var ms = new MemoryStream();
        stream!.CopyTo(ms);
        Assembly addonAssembly = Assembly.Load(ms.ToArray());

        Type? addonType = addonAssembly.GetTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(LotusAddon)));
        if (addonType == null)
        {
            BootLog("No LotusAddon class found in embedded resource.");
            return;
        }

        var addon = (LotusAddon)Activator.CreateInstance(addonType)!;

        Vents.Register(addonAssembly);
        AddonManager.Addons.Add(addon);
        addon.Initialize();
        BootLog("LotusCosmetics loaded. Running post-initialize.");
        addon.PostInitialize(AddonManager.Addons);
    }
    
    private static void BootLog(string message, params object[] args) => log.Log(BootstrapperLogLevel, message, args);
}