using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

namespace LotusCosmetics;

public static class CosmeticManager
{
    private static bool _initialized = false;
    public static readonly HashSet<string> LCSourceIds = new();

    public static void LoadCosmetics()
    {
        if (_initialized) return;
        _initialized = true;
        
        Assembly assembly = typeof(CosmeticManager).Assembly;

        using Stream? resource = assembly.GetManifestResourceStream("LotusCosmetics.LotusCosmetics.ccb");
        if (resource == null) throw new FileNotFoundException("Could not find embedded resource 'LotusCosmetics.ccb'");
        
        using var memoryStream = new MemoryStream();
        resource.CopyTo(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        
        RegisterBundleBytes(bytes);
        if (LotusCosmeticsAddon.CorsacIndependent)
            CorsacCosmetics.PluginCompat.QueueDiscoveryCoroutine(DeferredSetup(bytes));
        else 
            CorsacCosmetics.PluginCompat.AddBundleBytes(bytes);
    }
    
    private static IEnumerator DeferredSetup(byte[] bytes)
    {
        if (!Directory.Exists(PathPatches.BasePath)) PathPatches.CreateDirectories();
        CorsacCosmetics.PluginCompat.AddFolderSource(PathPatches.BasePath);
        CorsacCosmetics.PluginCompat.AddBundleBytes(bytes);
        yield break;
    }
    
    public static void RegisterBundleBytes(byte[] bundleBytes) 
    {
        using var ms = new MemoryStream(bundleBytes);
        var header = CorsacCosmetics.Cosmetics.Bundle.BundleHeader.Read(ms);
        var manifestBytes = new byte[header.ManifestLength];
        ms.Read(manifestBytes, 0, manifestBytes.Length);
        LCSourceIds.Add(new Guid(MD5.HashData(manifestBytes)).ToString());
    }
}