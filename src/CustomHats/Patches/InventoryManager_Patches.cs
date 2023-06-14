using HarmonyLib;
using TOUHats.CustomHats.Loading;
using VentLib.Utilities.Debug.Profiling;

namespace TOUHats.CustomHats.Patches
{
    [HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.CheckUnlockedItems))]
    public class InventoryManager_Patches
    {
        public static void Prefix(InventoryManager __instance)
        {
            uint id = Profilers.Global.Sampler.Start();
            HatLoaderAsync.LoadAllHats(__instance);
            Profilers.Global.Sampler.Stop(id);
        }
    }
}   