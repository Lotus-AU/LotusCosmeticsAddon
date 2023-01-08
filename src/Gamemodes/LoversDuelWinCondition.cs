using System.Collections.Generic;
using System.Linq;
using TownOfHost.Extensions;
using TownOfHost.Managers;
using TownOfHost.Roles;
using TownOfHost.Victory.Conditions;

namespace ValentinesAddon.Gamemodes;

public class LoversDuelWinCondition: IWinCondition
{
    public bool IsConditionMet(out List<PlayerControl> winners)
    {
        winners = null;
        List<PlayerControl> alivePlayers = Game.GetAlivePlayers().ToList();
        if (alivePlayers.Count > 2) return false;
        PlayerControl winner = alivePlayers.GetRandom();
        winners = new List<PlayerControl> { winner, winner.GetSubrole<Lovers>()!.Partner };
        return true;
    }

    public WinReason GetWinReason() => WinReason.RoleSpecificWin;

    public int Priority() => 10;
}