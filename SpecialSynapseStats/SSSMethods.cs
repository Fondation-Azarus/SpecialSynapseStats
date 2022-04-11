using Synapse.Api;
using Synapse.Database;
using System;
using System.Linq;
using System.Collections.Generic;
using Synapse;

namespace SpecialSynapseStats
{
    public class SSSMethods
    {
        internal static bool TryParseFloat(Player player, string key, out float result, string defaultValue = "0")
        {
            // Thanks Dimenzio for the idea
            result = float.NaN;
            string value = player.GetData(key);
            if (value == null)
            {
                player.SetData(key, defaultValue);
                Server.Get.Logger.Error($"{key} not defined which means it was deleted by an external plug-in you're using, {key} was reinitialized with {defaultValue} value to avoid further errors.");
                return false;
            }

            if (float.TryParse(value, out result))
                return true;

            player.SetData(key, defaultValue);
            Server.Get.Logger.Error($"{key} not a float even though it's supposed to be one meaning it was changed by an external plug-in you're using, {key} was reinitialized with {defaultValue} value to avoid further errors.");
            return false;
        }

        /// <summary>
        /// Verifies whether you can add data to the player <paramref name="player"/> by VSR standards or not.
        /// </summary>
        /// <param name="player">The player.</param> <param name="key">The key, not necessary if your key isn't meant to be used for security purposes.</param>
        /// <returns><see langword="true"/> if you can add data to this player ; otherwise, <see langword="false"/>.</returns>
        public static bool CanAddData(Player player, string key = null)
        {
            //if (Config.disabled) Even when this plug-in is disabled other plug-ins should still be able to use the system.
            //    return false;
            if (player == null)
                return false;

            if (!player.DoNotTrack)
                return true;

            if (PluginClass.Config.securityStatsEnabled && PluginClass.Config.securityStatsInverted ? !PluginClass.Config.securityStats.Contains(key) : PluginClass.Config.securityStats.Contains(key))
                return true;

            if (player.GetData(PluginClass.dataConsent) == null || player.GetData(PluginClass.dataConsent) != "true")
                return false;

            if (bool.TryParse(player.GetData(PluginClass.dataConsent), out bool dataConsent))
                return dataConsent;
            else
            {
                Server.Get.Logger.Error(PluginClass.dataConsent + " not a bool due to a external modification, it will be reinitialized with a false value to avoid further errors, you should report this error and give a list of plug-ins which modify player's data you use.");
                player.SetData(PluginClass.dataConsent, "false");
                return false;
            }
            
        }

        /// <summary>
        /// Converts the <see cref="KeyValuePair{TKey, TValue}.Value"/> of the first <see cref="KeyValuePair{TKey, TValue}"/> that it finds with the <see cref="KeyValuePair{TKey, TValue}.Key"/> equal to <paramref name="key"/> in a <see langword="float"/> so that you can easily add a <see langword="float"/> to it if adding data to the player <paramref name="player"/> is possible. Works with <see langword="int"/> and negative numbers as well. 
        /// </summary>
        /// <param name="player">The player.</param><param name="key">The key.</param><param name="number">The value you want to add.</param>
        public static void AddDataFloat(Player player, string key, float number = 1)
        {
            if (!CanAddData(player, key))
                return;

            string value = player.GetData(key);
            if (value == null)
            {
                player.SetData(key, number.ToString());
                return;
            }

            if (float.TryParse(value, out float parsedValue))
                player.SetData(key, (parsedValue + number).ToString());
            else
                Server.Get.Logger.Error(key + " not a float, AddDataFloat can't be used.");
        }

        /// <summary>
        /// Adds XP to the player <paramref name="player"/> if it's possible.
        /// </summary>
        /// <param name="player">The player.</param><param name="xp">The amount of XP you want to add, shouldn't be negative.</param><param name="reason">The reason that should be displayed as a hint to the player, "none" will hide the hint and only send a message in the console.</param>
        public static void AddXp(Player player, float xp = 100, string reason = "")
        {
            xp *= PluginClass.Config.xpMultiplier;
            if (!CanAddData(player) || xp <= 0)
                return;

            // Yay lots of maths ! :D


            if (!TryParseFloat(player, PluginClass.experienceData, out float parsedXp, xp.ToString()))
                return;
            float xpTotal = parsedXp + (float)Math.Round(xp, 2);
            int level = 0;


            if (!TryParseFloat(player, PluginClass.levelData, out float parsedLevel))
                return;
            float xpNeeded = PluginClass.Config.xpRorQ ? (float)Math.Round((decimal)(PluginClass.Config.firstLevelXpNeeded * Math.Pow(PluginClass.Config.qXpLevel, level + parsedLevel)), 2)
                : (float)Math.Round((decimal)(PluginClass.Config.firstLevelXpNeeded + PluginClass.Config.rXpLevel * (level + parsedLevel)), 2);


            while (xpTotal >= xpNeeded)
            {
                level++;
                xpTotal -= xpNeeded;
                xpNeeded = PluginClass.Config.xpRorQ ? (float)Math.Round((decimal)(PluginClass.Config.firstLevelXpNeeded * Math.Pow(PluginClass.Config.qXpLevel, level + parsedLevel)), 2)
                : (float)Math.Round((decimal)(PluginClass.Config.firstLevelXpNeeded + PluginClass.Config.rXpLevel * (level + parsedLevel)), 2);
            }

            player.SetData(PluginClass.experienceData, xpTotal.ToString());
                if (reason != "none")
                {
                    player.GiveTextHint(reason == "" ? PluginClass.Translation.ActiveTranslation.xpMessage.Replace("%xp%", xp.ToString()) : $"{reason}\n{PluginClass.Translation.ActiveTranslation.xpMessage.Replace("%xp%", xp.ToString())}", 2);
                    player.SendConsoleMessage(reason == "" ? PluginClass.Translation.ActiveTranslation.xpMessage.Replace("%xp%", xp.ToString()) : $"{reason}\n{PluginClass.Translation.ActiveTranslation.xpMessage.Replace("%xp%", xp.ToString())}", "green");
                }
                else
                    player.SendConsoleMessage(PluginClass.Translation.ActiveTranslation.xpMessage.Replace("%xp%", xp.ToString()), PluginClass.Config.xpConsoleColor);

            AddLevel(player, level);
        }

        /// <summary>
        /// Adds levels to the player <paramref name="player"/> if it's possible.
        /// </summary>
        /// <param name="player">The player.</param><param name="level">The amount of levels you want to add, shouldn't be negative.</param>
        public static void AddLevel(Player player, int level = 1)
        {
            if (!CanAddData(player) || level <= 0 || PluginClass.Config.xpMultiplier == 0)
                return;


            if (!TryParseFloat(player, PluginClass.levelData, out float parsedLevel, level.ToString()))
                return;
            int levelTotal = (int)parsedLevel + level;


            if (!TryParseFloat(player, PluginClass.bigLevelData, out float parsedBigLevel))
                return;
            int bigLevel = (int)parsedBigLevel;


            while (levelTotal >= PluginClass.Config.nmbrLevelNeededBigLevel)
            {
                bigLevel++;
                levelTotal -= PluginClass.Config.nmbrLevelNeededBigLevel;
            }

            player.SetData(PluginClass.levelData, levelTotal.ToString());
            player.GiveTextHint(PluginClass.Translation.ActiveTranslation.levelMessage.Replace("%level%", levelTotal.ToString()), 4);
            player.SendConsoleMessage(PluginClass.Translation.ActiveTranslation.levelMessage.Replace("%level%", levelTotal.ToString()), PluginClass.Config.levelConsoleColor);

            AddBigLevel(player, bigLevel);
        }

        /// <summary>
        /// Adds big levels to the player <paramref name="player"/> if it's possible.
        /// </summary>
        /// <param name="player">The player.</param><param name="bigLevel">The amount of big levels you want to add, shouldn't be negative.</param>
        public static void AddBigLevel(Player player, int bigLevel = 1)
        {
            if (!CanAddData(player) || bigLevel <= 0 || PluginClass.Config.xpMultiplier == 0)
                return;

            AddDataFloat(player, PluginClass.bigLevelData, bigLevel);
            player.GiveTextHint(PluginClass.Translation.ActiveTranslation.bigLevelMessage.Replace("%bigLevel%", player.GetData(PluginClass.bigLevelData)), 6);
            player.SendConsoleMessage(PluginClass.Translation.ActiveTranslation.bigLevelMessage.Replace("%bigLevel%", player.GetData(PluginClass.bigLevelData)), PluginClass.Config.bigLevelConsoleColor);
        }

        /// <summary>
        /// Tries to delete the first <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}"/> with a <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}.Key"/> equal to <paramref name="key"/> from the <see cref="PlayerDbo.Data"/> <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>of the player <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player.</param><param name="key"> The key, put "all" to delete every KeyValuePair.</param>
        /// <returns><see langword="true"/> if the specified KeyValuePair was deleted ; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="NullReferenceException"/>
        public static bool DeleteData(Player player, string key)
        {
            PlayerDbo pdo = DatabaseManager.PlayerRepository.FindByGameId(player.UserId);

            if (pdo == null)
                return false;

            if (key.ToLower() == "all")
            {
                if (PluginClass.Config.securityStatsEnabled)
                {
                    foreach (string k in pdo.Data.Keys)
                        if (PluginClass.Config.securityStatsInverted ? PluginClass.Config.securityStats.Contains(k) : !PluginClass.Config.securityStats.Contains(k))
                            if (float.TryParse(player.GetData(k), out _))
                                player.SetData(k, "0");
                    player.SetData(PluginClass.dataConsent, "false");
                    return true;
                }
                    
                pdo.Data.Clear();
                DatabaseManager.PlayerRepository.Save(pdo);
                return true;
            }

            if (player.GetData(key) == null)
                return false;

            pdo.Data.Remove(key);
            DatabaseManager.PlayerRepository.Save(pdo);
            return true;
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
            if (player == null) // || !CanAddData(player)) It should be returned non the less
                return null;

            return PluginClass.Config.listXpRewardsRoleID.FirstOrDefault(e => e.roleID == player.RoleID || (e.team == Team.MTF && PurelyMTF(player))) ?? PluginClass.Config.listXpRewardsRoleID.FirstOrDefault(e => e.team == player.Team);
        }

        /// <summary>
        /// Verifies if the players <paramref name="firstPlayer"/> and <paramref name="secondPlayer"/> are allies.
        /// </summary>
        /// <param name="firstPlayer">The first player (order doesn't matter).</param><param name="secondPlayer">The second player (order doesn't matter).</param>
        /// <returns><see langword="true"/> if they are allies ; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="NullReferenceException"/>
        public static bool AreAllies(Player firstPlayer, Player secondPlayer) => firstPlayer.TeamID == secondPlayer.TeamID || (firstPlayer.TeamID == (int)Team.MTF && secondPlayer.TeamID == (int)Team.RSC) || (firstPlayer.TeamID == (int)Team.CHI && secondPlayer.TeamID == (int)Team.CDP);
    }
}
