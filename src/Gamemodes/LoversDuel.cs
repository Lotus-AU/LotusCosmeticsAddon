using System.Collections.Generic;
using TownOfHost.Gamemodes.FFA;
using TownOfHost.Victory;

namespace ValentinesAddon.Gamemodes;

public class LoversDuel: FreeForAllGamemode
{
    public override string GetName() => "Lover's Duel";

    public override void AssignRoles(List<PlayerControl> players)
    {
        base.AssignRoles(players);
        RoleAssigner.AssignLovers(players);
    }

    public override void SetupWinConditions(WinDelegate winDelegate)
    {
        winDelegate.AddWinCondition(new LoversDuelWinCondition());
    }
}