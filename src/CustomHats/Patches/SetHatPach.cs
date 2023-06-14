using HarmonyLib;
using TOUHats.CustomHats.Loading;

namespace TOUHats.CustomHats.Patches;

[HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
public class SetHatPach
{
    public static bool Prefix(HatParent __instance, int color)
    {
        if (!HatLoaderAsync.Hats.ContainsKey(__instance.Hat.name)) return true;
        __instance.UnloadAsset();
        __instance.PopulateFromHatViewData();
        __instance.SetMaterialColor(color);
        return false;
    }
}