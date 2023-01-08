using System.Collections.Generic;
using System.Linq;
using TownOfHost.Extensions;
using TownOfHost.Interface.Menus.CustomNameMenu;
using TownOfHost.Managers;
using TownOfHost.Roles;
using UnityEngine;

namespace ValentinesAddon.Gamemodes;

public static class RoleAssigner
{
    private static readonly List<Color> LoverColors = new()
    {
        new Color(1f, 0.6f, 0f), new Color(0f, 1f, 0.96f), new Color(0.45f, 1f, 0.24f), new Color(0.75f, 0.4f, 1f),
        new Color(1f, 0.36f, 0.24f), new Color(1f, 0.79f, 0.94f), new Color(0f, 0.13f, 0.76f)
    };
    
    public static void AssignLovers(List<PlayerControl> players)
    {
        List<PlayerControl> candidates = players.Where(p => p.GetSubrole<Lovers>() == null).ToList();

        while (candidates.Count > 0)
        {
            if (candidates.Count == 1) // Unlucky rip
            {
                candidates[0].RpcMurderPlayer(candidates[0]);
                return;
            }

            PlayerControl victim = candidates.PopRandom();
            Game.AssignSubrole(victim, CustomRoleManager.Special.Lovers);
            Color color = LoverColors.PopRandom();
            Lovers victimRole = victim.GetSubrole<Lovers>()!;

            victim.GetDynamicName().RemoveRule(GameState.Roaming, UI.Subrole, victimRole.Partner.PlayerId);
            victimRole.Partner.GetDynamicName().RemoveRule(GameState.Roaming, UI.Subrole, victim.PlayerId);

            victim.GetDynamicName().SetComponentValue(UI.Subrole, new DynamicString(color.Colorize("♡")));
            victimRole.Partner.GetDynamicName().SetComponentValue(UI.Subrole, new DynamicString(color.Colorize("♡")));

            victim.GetDynamicName().AddRule(GameState.Roaming, UI.Subrole, new DynamicString(color.Colorize("♡")), victimRole.Partner.PlayerId);
            victimRole.Partner.GetDynamicName().AddRule(GameState.Roaming, UI.Subrole, new DynamicString(color.Colorize("♡")), victim.PlayerId);

            candidates = players.Where(p => p.GetSubrole<Lovers>() == null).ToList();
        }
    }
}