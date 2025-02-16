using Hazel;

namespace LotusCosmetics.Version;

/// <summary>
/// Version Representing this Addon
/// </summary>
public class LotusCosmeticsVersion: VentLib.Version.Version
{
    public override VentLib.Version.Version Read(MessageReader reader)
    {
        return new LotusCosmeticsVersion();
    }

    protected override void WriteInfo(MessageWriter writer)
    {
    }

    public override string ToSimpleName()
    {
        return "Project Lotus Cosmetics";
    }

    public override string ToString() => "ProjectLotusCosmetics";
}