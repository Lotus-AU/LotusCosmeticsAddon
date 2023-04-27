using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using VentLib.Utilities;
using VentLib.Utilities.Debug.Profiling;

namespace TOUHats.CustomHats;

internal static class HatLoader
{
    private const string HAT_RESOURCE_NAMESPACE = "TOUHats.assets.Hats";
    private const string HAT_METADATA_JSON = "metadata.json";
    private const int HAT_ORDER_BASELINE = 99;
    
    private static Assembly Assembly => typeof(TOUHatsAddon).Assembly;
    private static DLoadImage _iCallLoadImage;
    
    private static bool LoadedHats = false;

    internal static void LoadHatsRoutine()
    {
        if (LoadedHats || !DestroyableSingleton<HatManager>.InstanceExists || DestroyableSingleton<HatManager>.Instance.allHats.Count == 0)
            return;
        LoadedHats = true;
        Async.Execute(LoadHats());
        //Async.Execute(LoadHats);
    }

    internal static IEnumerator LoadHats()
    {
        
        var hatJson = LoadJson();
        var hatBehaviours = DiscoverHatBehaviours(hatJson);

        var hatData = new List<HatData>();
        hatData.AddRange(DestroyableSingleton<HatManager>.Instance.allHats);
        hatData.ForEach((Action<HatData>)(x => x.StoreName = "Vanilla"));

        var originalCount = DestroyableSingleton<HatManager>.Instance.allHats.ToList().Count;
        hatBehaviours.Reverse();
        for (var i = 0; i < hatBehaviours.Count; i++)
        {
            hatBehaviours[i].displayOrder = originalCount + i;
            hatData.Add(hatBehaviours[i]);
        }
        DestroyableSingleton<HatManager>.Instance.allHats = hatData.ToArray();
        yield return null;
    }

    private static HatMetadataJson LoadJson()
    {
        var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{HAT_METADATA_JSON}");
        return JsonSerializer.Deserialize<HatMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        });
    }

    private static List<HatData> DiscoverHatBehaviours(HatMetadataJson metadata)
    {
        var hatBehaviours = new List<HatData>();

        foreach (var hatCredit in metadata.Credits)
        {
            var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{hatCredit.Id}.png");
            if (stream != null)
            {
                var hatBehaviour = GenerateHatBehaviour(stream.ReadFully());
                hatBehaviour.StoreName = hatCredit.Artist;
                hatBehaviour.ProductId = hatCredit.Id;
                hatBehaviour.name = hatCredit.Name;
                hatBehaviour.Free = true;
                hatBehaviours.Add(hatBehaviour);
            }
        }

        return hatBehaviours;
    }

    private static HatData GenerateHatBehaviour(byte[] mainImg)
    {
            
        //TODO: Move to Graphics Utils class
        var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        uint id = Profilers.Global.Sampler.Start("HatLoader::LoadImage");
        LoadImage(tex2D, mainImg, false);
        Profilers.Global.Sampler.Stop(id);
        var sprite = Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100);


        var hat = ScriptableObject.CreateInstance<HatData>();
        var a = new HatViewData();
        var b = new AddressableLoadWrapper<HatViewData>();
        b.viewData = a;
        a.MainImage = sprite;
        hat.hatViewData = b;
        hat.ChipOffset = new Vector2(-0.1f, 0.35f);

        hat.InFront = true;
        hat.NoBounce = true;

        return hat;
    }
    
    public static byte[] ReadFully(this Stream input)
    {
        using var ms = new MemoryStream();
        input.CopyTo(ms);
        return ms.ToArray();
    }
        
    public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
    {
        _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
        var il2CPPArray = (Il2CppStructArray<byte>) data;
        _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
    }
    
    private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
}