using HarmonyLib;
using Lotus.Addons;

namespace TOUHats;

public class TOUHatsAddon: TOHAddon
{
    private Harmony harmony;
    
    public override void Initialize()
    {
        harmony = new Harmony("com.tealeaf.touhats");
        harmony.PatchAll();
    }

    public override string AddonName() => "Town of Us Hats";

    public override string AddonVersion() => "1.0.0";
}


