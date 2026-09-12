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
        return "LotusCosmetics";
    }

    public override string ToString() => "1.8.0";
}