using System;
using HarmonyLib;
using Lotus.Addons;
using LotusCosmetics.Version;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using VentLib.Logging;

namespace LotusCosmetics;

public class LotusCosmeticsAddon: LotusAddon
{
    public static LotusCosmeticsAddon Instance { get; private set; }
    
    public static bool CorsacIndependent = false; // if user has corsac installed standalone
    public static string RuntimeLocation;
    
    private Harmony harmony;
    
    public override void Initialize() // the order here is important
    {
        RuntimeLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)!;
        #if DEBUG
        StaticLogger.Debug($"PL Runtime Location: {RuntimeLocation}");
        #endif
        harmony = new Harmony("com.discussions.lotuscosmetics");
        
        FileInfo touBundleInfo = new(Path.Combine(RuntimeLocation, "touhats.bundle"));
        FileInfo touCatalogInfo = new(Path.Combine(RuntimeLocation, "touhats.catalog"));
        FileInfo oldBundleInfo = new(Path.Combine(RuntimeLocation, "lotushats.bundle"));
        FileInfo oldCatalogInfo = new(Path.Combine(RuntimeLocation, "lotushats.catalog"));
        
        FileInfo corsacDLL = new(Path.Combine(RuntimeLocation, "CorsacCosmetics.dll"));
        
        if (touBundleInfo.Exists || oldBundleInfo.Exists || touCatalogInfo.Exists || oldCatalogInfo.Exists)
        {
            StaticLogger.Info("Found one or more old cosmetic files, deleting them...");
            touBundleInfo?.Delete();
            oldBundleInfo?.Delete();
            touCatalogInfo?.Delete();
            oldCatalogInfo?.Delete();
        }
        
        // theoretically both of these checks could fail in the scenario where the user is on a beta version of Starlight
        // and is using lotus from marketplace & corsac from a local file - or lotus from local file & corsac from marketplace, idk
        if (corsacDLL.Exists || IL2CPPChainloader.Instance.Plugins.ContainsKey("CorsacCosmetics")) 
        {
            StaticLogger.Warn("User has CorsacCosmetics installed, not initializing embedded version.");
            CorsacIndependent = true;
            harmony.PatchAll();
        }
        else
        {
            StaticLogger.Info("Initializing embedded Corsac Cosmetics...");
            CorsacIndependent = false;
            
            using var stream = typeof(LotusCosmeticsAddon).Assembly
                .GetManifestResourceStream("LotusCosmetics.CorsacCosmetics.dll");
            using var ms = new MemoryStream();
            stream!.CopyTo(ms);
            Assembly corsacAssembly = Assembly.Load(ms.ToArray());
            
            harmony.PatchAll();
            Type? corsacType = corsacAssembly.GetTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(BasePlugin)));
            if (corsacType == null)
            {
                StaticLogger.Exception("Failed to find CorsacCosmeticsPlugin type in embedded assembly.");
                return;
            }
            
            ((BasePlugin)Activator.CreateInstance(corsacType))!.Load();
        }
        
        CosmeticManager.LoadCosmetics();
        Instance = this;
    }

    public override string Name { get; } = "LotusCosmetics";

    public override VentLib.Version.Version Version { get;} = new LotusCosmeticsVersion();
}


