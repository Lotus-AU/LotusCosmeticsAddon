using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using FreeImageAPI;
using UnityEngine;
using VentLib.Logging;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace TOUHats.CustomHats.Loading;

public class HatLoaderAsync
{
    private const string HAT_RESOURCE_NAMESPACE = "TOUHats.assets.Hats";
    private const string HAT_METADATA_JSON = "metadata.json";
    private const int HAT_ORDER_BASELINE = 99;
    
    private static Assembly Assembly => typeof(TOUHatsAddon).Assembly;
    private static List<HatData> _allHats;
    private static bool LoadedHats;

    public static void LoadAllHats()
    {
        if (LoadedHats || !DestroyableSingleton<HatManager>.InstanceExists || DestroyableSingleton<HatManager>.Instance.allHats.Count == 0)
            return;
        LoadedHats = true;
        
        _allHats = DestroyableSingleton<HatManager>.Instance.allHats.ToArray().ToList();
        _allHats.ForEach((Action<HatData>)(x => x.StoreName = "Vanilla"));
        
        LoadJson().Credits.ForEach(json => 
            Async.ExecuteThreaded(
                () => RawLoadTexture(json), 
                MainThreadAnchor.ScheduleOnMainThread<Optional<RawTextureData>>(optional => GenerateHatBehaviour(json).Invoke(optional))
                ));
    }

    private static Action<HatData> HatDataCallback(HatMetadataElement metadata)
    {
        return hatData =>
        {
            hatData.StoreName = metadata.Artist;
            hatData.ProductId = metadata.Id;
            hatData.name = metadata.Name;
            hatData.Free = true;
            _allHats.Add(hatData);
            DestroyableSingleton<HatManager>.Instance.allHats = _allHats.ToArray();
        };
    }
    
    private static Action<Optional<RawTextureData>> GenerateHatBehaviour(HatMetadataElement metadata)
    {
        return optional => optional.Map(GenerateHatBehaviour).IfPresent(HatDataCallback(metadata));
    }

    private static RawTextureData FinalizeLoadTexture(FIBITMAP fibitmap)
    {
        uint width = FreeImage.GetWidth(fibitmap);
        uint height = FreeImage.GetHeight(fibitmap);
        uint size = width * height * 4;

        byte[] data = new byte[size];
        FreeImage.ConvertToRawBits(Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), fibitmap, (int)width * 4, 32, 0, 0, 0, false);

        RawTextureData rawTextureData = new()
        {
            Data = data,
            Width = (int)width,
            Height = (int)height,
            MipLevels = 1
        };

        
        return rawTextureData;
    }

    private static Optional<RawTextureData> RawLoadTexture(HatMetadataElement metadata)
    {
        VentLogger.Trace($"Loading Hat ({metadata.Id}): {metadata.Name} By: {metadata.Artist}");
        var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{metadata.Id}.png");
        return Optional<Stream>.Of(stream).Map(FreeImage.LoadFromStream).Map(FinalizeLoadTexture);
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

    private static HatData GenerateHatBehaviour(RawTextureData rawTexture)
    {
        Texture2D tex = new(rawTexture.Width, rawTexture.Height, TextureFormat.BGRA32,false);
        
        tex.LoadRawTextureData(Marshal.UnsafeAddrOfPinnedArrayElement(rawTexture.Data, 0), rawTexture.Data.Length);
        tex.Apply();

        var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);

        var hat = ScriptableObject.CreateInstance<HatData>();
        var a = ScriptableObject.CreateInstance<HatViewData>();
        var b = new AddressableLoadWrapper<HatViewData>();
        b.viewData = a;
        a.MainImage = sprite;
        hat.hatViewData = b;
        hat.ChipOffset = new Vector2(-0.1f, 0.35f);

        hat.InFront = true;
        hat.NoBounce = true;
        return hat;
    }
}