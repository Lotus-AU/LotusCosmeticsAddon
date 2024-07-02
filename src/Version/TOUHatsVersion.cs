using Hazel;

namespace TOUHats.Version;

/// <summary>
/// Version Representing this Addon
/// </summary>
public class TOUHatsVersion: VentLib.Version.Version
{
    public override VentLib.Version.Version Read(MessageReader reader)
    {
        return new TOUHatsVersion();
    }

    protected override void WriteInfo(MessageWriter writer)
    {
    }

    public override string ToSimpleName()
    {
        return "Town of Us Hats";
    }

    public override string ToString() => "TownOfUsHats";
}