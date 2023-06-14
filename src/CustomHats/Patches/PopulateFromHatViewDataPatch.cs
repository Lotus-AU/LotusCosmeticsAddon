using System.Collections.Generic;
using HarmonyLib;
using PowerTools;
using TOUHats.CustomHats.Loading;
using UnityEngine;

namespace TOUHats.CustomHats.Patches;

[HarmonyPatch(typeof(HatParent), nameof(HatParent.PopulateFromHatViewData))]
public class PopulateFromHatViewDataPatch
{
    public static bool Prefix(HatParent __instance)
    {
        __instance.UpdateMaterial();

        Sprite image = HatLoaderAsync.Hats.GetValueOrDefault(__instance.Hat.name);

        if (image == null) return true;
        
        SpriteAnimNodeSync spriteAnimNodeSync = __instance.SpriteSyncNode ?? __instance.GetComponent<SpriteAnimNodeSync>();
        if ((bool) (Object) spriteAnimNodeSync)
            spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;
        if (__instance.Hat.InFront)
        {
            __instance.BackLayer.enabled = false;
            __instance.FrontLayer.enabled = true;
            __instance.FrontLayer.sprite = image;
        }

        else
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = false;
            __instance.FrontLayer.sprite = null;
            __instance.BackLayer.sprite = image;
        }
        if (!__instance.options.Initialized || !__instance.HideHat())
            return false;
        __instance.FrontLayer.enabled = false;
        __instance.BackLayer.enabled = false;
        return false;
    }
}