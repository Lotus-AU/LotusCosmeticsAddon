using HarmonyLib;

namespace LotusCosmetics;

[HarmonyPatch]
public class CosmeticPatches
{
    [HarmonyPatch(typeof(CorsacCosmetics.Cosmetics.CosmeticDescriptor), "GenerateGuid")]
    [HarmonyPrefix]
    static void GenerateGUIDPrefix(ref string[] parts)
    {
        if (parts.Length == 0 || !CosmeticManager.LCSourceIds.Contains(parts[0])) return;

        var patched = (string[])parts.Clone();
        patched[0] = string.Empty;
        parts = patched;
    }
}