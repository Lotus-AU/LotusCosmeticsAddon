using HarmonyLib;
using TOUHats.CustomHats.Loading;
using VentLib.Utilities.Debug.Profiling;

namespace TOUHats.CustomHats.Patches
{
    [HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.CheckUnlockedItems))]
    public class InventoryManager_Patches
    {
        public static void Prefix()
        {
            uint id = Profilers.Global.Sampler.Start();
            HatLoaderAsync.LoadAllHats();
            //HatLoader.LoadHatsRoutine();
            Profilers.Global.Sampler.Stop(id);
        }
    }
}   