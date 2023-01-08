using TownOfHost.Addons;
using ValentinesAddon.Gamemodes;

namespace ValentinesAddon;

public class ValentinesAddon: TOHAddon
{
    public override void Initialize()
    {
        RegisterRole(new Cupid());
        RegisterGamemode(new LoversDuel());
    }

    public override string AddonName() => "Hell's Angels";

    public override string AddonVersion() => "1.2.3";
}


