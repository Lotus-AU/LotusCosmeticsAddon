using System.IO;
using HarmonyLib;
using Lotus.Managers;

namespace LotusCosmetics;

[HarmonyPatch(typeof(CorsacCosmetics.Cosmetics.CosmeticPaths))]
public static class PathPatches
{
    private static readonly string BasePath =
        Path.Combine(PluginDataManager.ModifiableDataDirectory.FullName, "Cosmetics");

    public static bool Prepare() => !LotusCosmeticsAddon.CorsacIndependent;

    [HarmonyPatch(nameof(CorsacCosmetics.Cosmetics.CosmeticPaths.BasePath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void BasePathPostfix(ref string __result)
    {
        __result = BasePath;
    }

    [HarmonyPatch(nameof(CorsacCosmetics.Cosmetics.CosmeticPaths.BundlePath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void BundlePathPostfix(ref string __result)
    {
        __result = Path.Combine(BasePath, "Bundles");
    }

    [HarmonyPatch(nameof(CorsacCosmetics.Cosmetics.CosmeticPaths.HatPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void HatPathPostfix(ref string __result)
    {
        __result = Path.Combine(BasePath, "Hats");
    }

    [HarmonyPatch(nameof(CorsacCosmetics.Cosmetics.CosmeticPaths.VisorPath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void VisorPathPostfix(ref string __result)
    {
        __result = Path.Combine(BasePath, "Visors");
    }

    [HarmonyPatch(nameof(CorsacCosmetics.Cosmetics.CosmeticPaths.NameplatePath), MethodType.Getter)]
    [HarmonyPostfix]
    private static void NameplatePathPostfix(ref string __result)
    {
        __result = Path.Combine(BasePath, "Nameplates");
    }

    [HarmonyPatch(nameof(CorsacCosmetics.PluginCompat), nameof(CorsacCosmetics.PluginCompat.QueueBundleDownload))]
    [HarmonyPrefix]
    private static bool QueueBundleDownloadPrefix(string url)
    {
        if (url is "https://github.com/XtraCube/CorsacCosmetics/releases/download/2.0.0/default.ccb") return false;
        return true;
    }
}
