﻿using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api.Items;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Synapse.Api.Enum;
using Synapse.Database;
using MEC;
using System;
using MapGeneration;
using Synapse.Translation;

namespace SpecialSynapseStats
{
    public class Handler
    {
        public Handler()
        {
            Server.Get.Events.Player.PlayerJoinEvent += OnJoin;
            Server.Get.Events.Round.RoundStartEvent += OnRoundStart;
            Server.Get.Events.Player.PlayerSetClassEvent += OnSetClass;
            Server.Get.Events.Player.PlayerItemUseEvent += OnItemUse;
            Server.Get.Events.Player.PlayerDamageEvent += OnDamage;
            Server.Get.Events.Scp.ScpAttackEvent += OnScpAttack;
            Server.Get.Events.Player.PlayerDeathEvent += OnDeath;
            Server.Get.Events.Player.PlayerEscapesEvent += OnEscape;
            Server.Get.Events.Round.RoundEndEvent += OnRoundEnd;
            Server.Get.Events.Map.WarheadDetonationEvent += OnWarheadDetonation;
            Server.Get.Events.Player.PlayerBanEvent += OnBan;
        }

        /// <summary>
        /// Verifies if the player <paramref name="player"/> should be considered as part of <see cref="Team.MTF"/> depending on the server's config.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><see langword="true"/> if the player <paramref name="player"/>'s <see cref="Team"/> is equal to <see cref="Team.MTF"/> or if he's <see cref="RoleType"/> is equal to <see cref="RoleType.FacilityGuard"/> and the owner of this server wants them to be considered as part of <see cref="Team.MTF"/> ; otherwise, <see langword="false"/>.</returns>
        private static bool PurelyMTF(Player player) => PluginClass.Config.guardMTF ? player.Team == Team.MTF : player.RoleID != (int)RoleType.FacilityGuard && player.Team == Team.MTF;

        /// <summary>
        /// Tries to get the <see cref="Configs.XpRewardsRoleID"/> that has a <see cref="Configs.XpRewardsRoleID.roleID"/> equal to the player <paramref name="player"/>'s one, if it doesn't find one it will try to find one that has a <see cref="Configs.XpRewardsRoleID.team"/> equal to the player <paramref name="player"/>'s one.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><see cref="Configs.XpRewardsRoleID"/> if it finds one ; otherwise, <see langword="null"/>.</returns>
        public static Configs.XpRewardsRoleID GetXpRewardsRoleID(Player player)
        {
            if (player == null || !PluginClass.CanAddData(player))
                return null;

            return PluginClass.Config.listXpRewardsRoleID.FirstOrDefault(e => e.roleID == player.RoleID || (e.team == Team.MTF && PurelyMTF(player))) ?? PluginClass.Config.listXpRewardsRoleID.FirstOrDefault(e => e.team == player.Team);
        }

        /// <summary>
        /// Verifies if the players <paramref name="firstPlayer"/> and <paramref name="secondPlayer"/> are allies.
        /// </summary>
        /// <param name="firstPlayer">The first player (order doesn't matter).</param><param name="secondPlayer">The second player (order doesn't matter).</param>
        /// <returns><see langword="true"/> if they are allies ; otherwise, <see langword="false"/>.</returns>
        public static bool AreAllies(Player firstPlayer, Player secondPlayer) => firstPlayer.Team == secondPlayer.Team || (firstPlayer.Team == Team.MTF && secondPlayer.Team == Team.RSC) || (firstPlayer.Team == Team.CHI && secondPlayer.Team == Team.CDP);

        private IEnumerator<float> XpMinReward()
        {
            while (!Round.Get.RoundEnded)
            {
                foreach (Player p in Server.Get.Players)
                {
                    PluginClass.AddDataFloat(p, PluginClass.playtimeData);
                    PluginClass.AddXp(p, GetXpRewardsRoleID(p) == null ? PluginClass.Config.xpGeneralPerMinute : GetXpRewardsRoleID(p).survivalXp + PluginClass.Config.xpGeneralPerMinute, "none");
                }
                yield return Timing.WaitForSeconds(60);
            }
        }

        // =========================================================================

        private void OnJoin(PlayerJoinEventArgs ev)
        {
            Timing.CallDelayed(1, () =>
            {
                if (ev.Player == null)
                    return;

                if (ev.Player.DoNotTrack && ev.Player.GetData(PluginClass.dataConsent) == null)
                {
                    ev.Player.GiveTextHint(PluginClass.Translation.ActiveTranslation.cantAddDataHint, 10);
                    ev.Player.SendConsoleMessage(PluginClass.Translation.ActiveTranslation.cantAddDataConsoleMessage);
                    return;
                }

                if (ev.Player.GetData(PluginClass.firstLogin) == null)
                    PluginClass.FirstLogin(ev.Player);
            });
        }

        private void OnRoundStart()
        {
            Timing.RunCoroutine(XpMinReward());
            foreach (Player p in Server.Get.Players)
                PluginClass.AddDataFloat(p, PluginClass.startedRoundsData);
        }

        private void OnSetClass(PlayerSetClassEventArgs ev)
        {
            if (ev.Player == null || !PluginClass.Config.spawnClassRegister)
                return;

            if (ev.SpawnReason == CharacterClassManager.SpawnReason.ForceClass || ev.SpawnReason == CharacterClassManager.SpawnReason.Overwatch || ev.SpawnReason == CharacterClassManager.SpawnReason.Died)
                return;

                PluginClass.AddDataFloat(ev.Player, ev.Role.ToString());
        }

        private void OnItemUse(PlayerItemInteractEventArgs ev)
        {
            if (ev.Player == null || ev.CurrentItem == null || !PluginClass.Config.itemUseRegister)
                return;

            if (ev.State != ItemInteractState.Finalizing)
                return;

            PluginClass.AddDataFloat(ev.Player, ev.CurrentItem.Name);
        }

        private void OnDamage(PlayerDamageEventArgs ev)
        {
            if (ev.Victim == null || ev.Killer == null)
                return;

            if (ev.Victim != ev.Killer && !AreAllies(ev.Victim, ev.Killer))
            {
                PluginClass.AddDataFloat(ev.Killer, PluginClass.dmgsInflictedData, ev.Damage);
                PluginClass.AddDataFloat(ev.Victim, PluginClass.dmgasReceivedData, ev.Damage);
            }
        }

        private void OnScpAttack(ScpAttackEventArgs ev)
        {
            if (ev.Scp == null || ev.Target == null)
                return;

            if (ev.Scp.RoleType != RoleType.Scp106 || ev.Target.GodMode || ev.AttackType != ScpAttackType.Scp106_Grab)
                return;

            PluginClass.AddXp(ev.Scp, PluginClass.Config.scp106GrabXp, PluginClass.Translation.ActiveTranslation.scp106grabMessage);
        }

        private void OnDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Victim == null)
                return;
            PluginClass.AddDataFloat(ev.Victim, PluginClass.deathsData);

            if (ev.DamageType == DamageType.Warhead && PluginClass.Config.warheadScpKillAssist)
                foreach (Player p in Server.Get.GetPlayers(p => p.Team == Team.MTF || p.Team == Team.RSC))
                    PluginClass.AddXp(p, GetXpRewardsRoleID(p).scpKillAssistXp, PluginClass.Translation.ActiveTranslation.scpKillAssistMessage);

            if (ev.Killer == null)
                return;

            if (ev.Killer == ev.Victim || ev.Killer.RoleType == RoleType.Scp079) // SCP-079 kills are weird and cause some issues
                return;

            if (!AreAllies(ev.Killer, ev.Victim))
                PluginClass.AddDataFloat(ev.Killer, PluginClass.killsData);
            else
                PluginClass.AddDataFloat(ev.Killer, PluginClass.teamkillsData);

            Configs.XpRewardsRoleID erri = GetXpRewardsRoleID(ev.Killer);
            if (erri != null)
            {
                if (!erri.roleIDKillXp.TryGetValue(ev.Victim.RoleID, out float xp1))
                {
                    if (erri.teamKillXp.TryGetValue(ev.Victim.Team, out float xp2))
                        PluginClass.AddXp(ev.Killer, xp2, PluginClass.Translation.ActiveTranslation.killMessage.Replace("%victim%", ev.Victim.NickName));
                }
                else
                    PluginClass.AddXp(ev.Killer, xp1, PluginClass.Translation.ActiveTranslation.killMessage.Replace("%victim%", ev.Victim.NickName));
            }

            if (ev.Victim.Team == Team.SCP && ev.Killer.Team != Team.SCP && ev.DamageType != DamageType.Warhead)
                foreach (Player p in Server.Get.GetPlayers(p => !AreAllies(p, ev.Killer)))
                    PluginClass.AddXp(p, GetXpRewardsRoleID(p).scpKillAssistXp, PluginClass.Translation.ActiveTranslation.scpKillAssistMessage);
        }

        private void OnEscape(PlayerEscapeEventArgs ev)
        {
            if (ev.Player == null)
                return;

            if (ev.Player.RealTeam == Team.SCP && PluginClass.Config.scpEscape)
            {
                ev.Allow = true;
                ev.SpawnRole = (int)RoleType.Spectator;
            }

            if (!ev.Allow)
                return;

            if (!ev.IsCuffed)
            {
                Configs.XpRewardsRoleID erri = GetXpRewardsRoleID(ev.Player);
                if (erri == null)
                    return;

                PluginClass.AddXp(ev.Player, erri.escapeXp, PluginClass.Translation.ActiveTranslation.escapeMessage);
                PluginClass.AddDataFloat(ev.Player, PluginClass.escapesData); // U

                foreach (Player p in Server.Get.GetPlayers(p => AreAllies(p, ev.Player) && (p.Zone == ZoneType.Surface || !PluginClass.Config.assistEscapeSurfaceZone) && p != ev.Player))
                    PluginClass.AddXp(p, GetXpRewardsRoleID(p).escapeAssistXp, PluginClass.Translation.ActiveTranslation.escapeAssistMessage.Replace("%player%", ev.Player.NickName));
            }
            
            if (ev.Cuffer == null)
                return;

            PluginClass.AddXp(ev.Cuffer, GetXpRewardsRoleID(ev.Cuffer).captureXp, PluginClass.Translation.ActiveTranslation.captureMessage.Replace("%player%", ev.Player.NickName));
        }

        private void OnWarheadDetonation()
        {
            foreach (Player ci in Server.Get.GetPlayers(p => GetXpRewardsRoleID(p).warheadXp > 0))
                PluginClass.AddXp(ci, GetXpRewardsRoleID(ci).warheadXp, PluginClass.Translation.ActiveTranslation.warheadMessage);
        }

        private void OnRoundEnd()
        {
            foreach (Player p in Server.Get.Players)
            {
                PluginClass.AddDataFloat(p, PluginClass.endedRoundsData);
                p.SetData(PluginClass.roundsData, Math.Min(int.Parse(p.GetData(PluginClass.startedRoundsData)), int.Parse(p.GetData(PluginClass.endedRoundsData))).ToString());
            }
        }

        private void OnBan(PlayerBanEventArgs ev)
        {
            if (ev.BannedPlayer == null || ev.Issuer == null)
                return;

            if (ev.BannedPlayer == ev.Issuer)
                return;

            if (ev.BanDuration > 0) // Ban
            {
                PluginClass.AddDataFloat(ev.BannedPlayer, PluginClass.bansData);
                PluginClass.AddDataFloat(ev.BannedPlayer, PluginClass.totalBanDurationData, (int)ev.BanDuration);
            }
            else // Kick
                PluginClass.AddDataFloat(ev.BannedPlayer, PluginClass.kicksData);
        }
    }
}