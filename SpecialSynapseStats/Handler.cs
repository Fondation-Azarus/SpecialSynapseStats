using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System.Collections.Generic;
using Synapse.Api.Enum;
using MEC;
using System;

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

        private IEnumerator<float> XpMinReward()
        {
            while (!Round.Get.RoundEnded)
            {
                foreach (Player p in Server.Get.Players)
                {
                    SSSMethods.AddDataFloat(p, PluginClass.playtimeData);
                    SSSMethods.AddXp(p, SSSMethods.GetXpRewardsRoleID(p) == null ? PluginClass.Config.xpGeneralPerMinute : SSSMethods.GetXpRewardsRoleID(p).survivalXp + PluginClass.Config.xpGeneralPerMinute, "none");
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

                if (!SSSMethods.CanAddData(ev.Player))
                {
                    ev.Player.GiveTextHint(PluginClass.Translation.ActiveTranslation.cantAddDataHint, 10);
                    ev.Player.SendConsoleMessage(PluginClass.Translation.ActiveTranslation.cantAddDataConsoleMessage);

                    if (PluginClass.Config.securityStatsEnabled)
                    {
                        PluginClass.FirstLogin(ev.Player);
                        ev.Player.SetData(PluginClass.dataConsent, "false");
                    }

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
                SSSMethods.AddDataFloat(p, PluginClass.startedRoundsData);
        }

        private void OnSetClass(PlayerSetClassEventArgs ev)
        {
            if (ev.Player == null || !PluginClass.Config.spawnClassRegister)
                return;

            if (ev.SpawnReason == CharacterClassManager.SpawnReason.ForceClass || ev.SpawnReason == CharacterClassManager.SpawnReason.Overwatch || ev.SpawnReason == CharacterClassManager.SpawnReason.Died)
                return;

            SSSMethods.AddDataFloat(ev.Player, ev.Role.ToString());
        }

        private void OnItemUse(PlayerItemInteractEventArgs ev)
        {
            if (ev.Player == null || ev.CurrentItem == null || !PluginClass.Config.itemUseRegister)
                return;

            if (ev.State != ItemInteractState.Finalizing)
                return;

            SSSMethods.AddDataFloat(ev.Player, ev.CurrentItem.Name);
        }

        private void OnDamage(PlayerDamageEventArgs ev)
        {
            if (ev.Victim == null || ev.Killer == null)
                return;

            if (ev.Victim != ev.Killer && !SSSMethods.AreAllies(ev.Victim, ev.Killer))
            {
                SSSMethods.AddDataFloat(ev.Killer, PluginClass.dmgsInflictedData, ev.Damage);
                SSSMethods.AddDataFloat(ev.Victim, PluginClass.dmgasReceivedData, ev.Damage);
            }
        }

        private void OnScpAttack(ScpAttackEventArgs ev)
        {
            if (ev.Scp == null || ev.Target == null)
                return;

            if (ev.Target.GodMode || ev.AttackType != ScpAttackType.Scp106_Grab)
                return;

            SSSMethods.AddXp(ev.Scp, PluginClass.Config.scp106GrabXp, PluginClass.Translation.ActiveTranslation.scp106grabMessage);
        }

        private void OnDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Victim == null)
                return;
            SSSMethods.AddDataFloat(ev.Victim, PluginClass.deathsData);

            if (ev.DamageType == DamageType.Warhead && PluginClass.Config.warheadScpKillAssist)
                foreach (Player p in Server.Get.GetPlayers(p => p.Team == Team.MTF || p.Team == Team.RSC))
                    SSSMethods.AddXp(p, SSSMethods.GetXpRewardsRoleID(p).scpKillAssistXp, PluginClass.Translation.ActiveTranslation.scpKillAssistMessage);

            if (ev.Killer == null)
                return;

            if (ev.Killer == ev.Victim || ev.Killer.RoleType == RoleType.Scp079) // SCP-079 kills are weird and cause some issues
                return;

            if (!SSSMethods.AreAllies(ev.Killer, ev.Victim))
                SSSMethods.AddDataFloat(ev.Killer, PluginClass.killsData);
            else
                SSSMethods.AddDataFloat(ev.Killer, PluginClass.teamkillsData);

            Configs.XpRewardsRoleID erri = SSSMethods.GetXpRewardsRoleID(ev.Killer);
            if (erri != null)
            {
                if (!erri.roleIDKillXp.TryGetValue(ev.Victim.RoleID, out float xp1))
                {
                    if (erri.teamKillXp.TryGetValue(ev.Victim.Team, out float xp2))
                        SSSMethods.AddXp(ev.Killer, xp2, PluginClass.Translation.ActiveTranslation.killMessage.Replace("%victim%", ev.Victim.NickName));
                }
                else
                    SSSMethods.AddXp(ev.Killer, xp1, PluginClass.Translation.ActiveTranslation.killMessage.Replace("%victim%", ev.Victim.NickName));
            }

            if (ev.Victim.Team == Team.SCP && ev.Killer.Team != Team.SCP && ev.DamageType != DamageType.Warhead)
                foreach (Player p in Server.Get.GetPlayers(p => !SSSMethods.AreAllies(p, ev.Killer)))
                    SSSMethods.AddXp(p, SSSMethods.GetXpRewardsRoleID(p).scpKillAssistXp, PluginClass.Translation.ActiveTranslation.scpKillAssistMessage);
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
                Configs.XpRewardsRoleID erri = SSSMethods.GetXpRewardsRoleID(ev.Player);
                if (erri == null)
                    return;

                SSSMethods.AddXp(ev.Player, erri.escapeXp, PluginClass.Translation.ActiveTranslation.escapeMessage);
                SSSMethods.AddDataFloat(ev.Player, PluginClass.escapesData);

                foreach (Player p in Server.Get.GetPlayers(p => SSSMethods.AreAllies(p, ev.Player) && (p.Zone == ZoneType.Surface || !PluginClass.Config.assistEscapeSurfaceZone) && p != ev.Player))
                    SSSMethods.AddXp(p, SSSMethods.GetXpRewardsRoleID(p).escapeAssistXp, PluginClass.Translation.ActiveTranslation.escapeAssistMessage.Replace("%player%", ev.Player.NickName));
                if (ev.Player.TeamID == (int)Team.SCP)
                    foreach (Player p in Server.Get.Players)
                        SSSMethods.AddXp(p, SSSMethods.GetXpRewardsRoleID(p).scpEscapeXp, PluginClass.Translation.ActiveTranslation.scpEscapeMessage.Replace("%player%", ev.Player.NickName));
            }
            
            if (ev.Cuffer == null)
                return;

            if (!SSSMethods.AreAllies(ev.Cuffer, ev.Player)) // Idk why you would cuff your allie if you manage to do this with a plug-in but just in case
                SSSMethods.AddXp(ev.Cuffer, SSSMethods.GetXpRewardsRoleID(ev.Cuffer).captureXp, PluginClass.Translation.ActiveTranslation.captureMessage.Replace("%player%", ev.Player.NickName));
        }

        private void OnWarheadDetonation()
        {
            foreach (Player ci in Server.Get.GetPlayers(p => SSSMethods.GetXpRewardsRoleID(p).warheadXp > 0))
                SSSMethods.AddXp(ci, SSSMethods.GetXpRewardsRoleID(ci).warheadXp, PluginClass.Translation.ActiveTranslation.warheadMessage);
        }

        private void OnRoundEnd()
        {
            foreach (Player p in Server.Get.Players)
            {
                SSSMethods.AddDataFloat(p, PluginClass.endedRoundsData);
                if (SSSMethods.TryParseFloat(p, PluginClass.startedRoundsData, out float parsedStartedRounds) && SSSMethods.TryParseFloat(p, PluginClass.endedRoundsData, out float parsedEndedRounds, "1"))
                p.SetData(PluginClass.roundsData, Math.Min((int)parsedStartedRounds, (int)parsedEndedRounds).ToString());
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
                SSSMethods.AddDataFloat(ev.BannedPlayer, PluginClass.bansData);
                SSSMethods.AddDataFloat(ev.BannedPlayer, PluginClass.totalBanDurationData, (int)ev.BanDuration);
            }
            else // Kick
                SSSMethods.AddDataFloat(ev.BannedPlayer, PluginClass.kicksData);
        }
    }
}