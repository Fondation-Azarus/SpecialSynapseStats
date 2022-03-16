using Synapse.Api.Plugin;
using Synapse.Translation;
using Synapse.Api;
using System;
using Synapse.Database;
using System.Linq;

namespace SpecialSynapseStats
{
    [PluginInformation(
        Name = "SpecialSynapseStats",
        Author = "Bonjemus",
        Description = "A plug-in that adds an XP and a stats system.",
        LoadPriority = int.MaxValue,
        SynapseMajor = SynapseController.SynapseMajor,
        SynapseMinor = SynapseController.SynapseMinor,
        SynapsePatch = SynapseController.SynapsePatch,
        Version = "1.0.0"
        )]
    public class PluginClass : AbstractPlugin
    {
        [Config(section = "SpecialSynapseStats")]
        public static Configs Config;

        [SynapseTranslation]
        public static new SynapseTranslation<Translations> Translation;

        public override void Load()
        {
            if (Config.disabled)
                return;    
            
            new Handler();
            Translation.AddTranslation(new Translations());
        }

        /// <summary>
        /// Verifies whether you can add data to the player <paramref name="player"/> by VSR standards or not.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><see langword="true"/> if you can add data to this player ; otherwise, <see langword="false"/>.</returns>
        public static bool CanAddData(Player player)
        {
            if (Config.disabled)
                return false;

            if (!player.DoNotTrack)
                return true;

            if (player.GetData(dataConsent) == null || player.GetData(dataConsent) != "true")
                return false;

            return bool.Parse(player.GetData(dataConsent));
        }

        /// <summary>
        /// Converts the <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}.Value"/> of the first <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}"/> that it finds with the <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}.Key"/> equal to <paramref name="key"/> in a <see langword="float"/> so that you can easily add a <see langword="float"/> to it if adding data to the player <paramref name="player"/> is possible. Works with <see langword="int"/> and negative numbers as well. 
        /// </summary>
        /// <param name="player">The player.</param><param name="key">The key.</param><param name="number">The value you want to add.</param>
        public static void AddDataFloat(Player player, string key, float number = 1)
        {
            if (!CanAddData(player))
                return;

            string value = player.GetData(key);
            if (value == null)
            {
                player.SetData(key, number.ToString());
                return;
            }
            player.SetData(key, (float.Parse(value) + number).ToString());
        }

        /// <summary>
        /// Adds XP to the player <paramref name="player"/> if it's possible.
        /// </summary>
        /// <param name="player">The player.</param><param name="exp">The amount of XP you want to add, shouldn't be negative.</param><param name="reason">The reason that should be displayed as a hint to the player, "none" will hide the hint and only send a message in the console.</param>
        public static void AddExp(Player player, float exp = 100, string reason = "")
        {
            if (!CanAddData(player) || exp < 0 || Config.expMultiplier == 0)
                return;

            // Yay lots of maths ! :D

            exp *= Config.expMultiplier;
            float expTotal = float.Parse(player.GetData(experienceData)) + (float)Math.Round(exp, 2);
            int level = 0;


            float expNeeded = Config.xpRorQ ? (float)Math.Round((decimal)(Config.firstLevelExpNeeded * Math.Pow(Config.qExpLevel, level + int.Parse(player.GetData(levelData)))), 2)
                : (float)Math.Round((decimal)(Config.firstLevelExpNeeded + Config.rExpLevel * (level + int.Parse(player.GetData(levelData)))), 2);


            while (expTotal >= expNeeded)
            {
                level++;
                expTotal -= expNeeded;
                expNeeded = Config.xpRorQ ? (float)Math.Round((decimal)(Config.firstLevelExpNeeded * Math.Pow(Config.qExpLevel, level + int.Parse(player.GetData(levelData)))), 2)
                : (float)Math.Round((decimal)(Config.firstLevelExpNeeded + Config.rExpLevel * (level + int.Parse(player.GetData(levelData)))), 2);
            }

            if (exp > 0)
            {
                player.SetData(experienceData, expTotal.ToString());
                if (reason != "none")
                {
                    player.GiveTextHint(reason == "" ? Translation.ActiveTranslation.xpMessage.Replace("%xp%", exp.ToString()) : $"{reason}\n{Translation.ActiveTranslation.xpMessage.Replace("%xp%", exp.ToString())}", 2);
                    player.SendConsoleMessage(reason == "" ? Translation.ActiveTranslation.xpMessage.Replace("%xp%", exp.ToString()) : $"{reason}\n{Translation.ActiveTranslation.xpMessage.Replace("%xp%", exp.ToString())}", "green");
                }
                else
                    player.SendConsoleMessage(Translation.ActiveTranslation.xpMessage.Replace("%xp%", exp.ToString()), "green");
            }

            AddLevel(player, level);
        }

        /// <summary>
        /// Adds levels to the player <paramref name="player"/> if it's possible.
        /// </summary>
        /// <param name="player">The player.</param><param name="level">The amount of levels you want to add, shouldn't be negative.</param>
        public static void AddLevel(Player player, int level = 1)
        {
            if (!CanAddData(player) || level < 0 || Config.expMultiplier == 0)
                return;

            int levelTotal = int.Parse(player.GetData(levelData)) + level;
            int b = int.Parse(player.GetData(bigLevelData));

            while (levelTotal >= Config.nmbrLevelNeededBigLevel)
            {
                b++;
                levelTotal -= Config.nmbrLevelNeededBigLevel;
            }

            if (level > 0)
            {
                player.SetData(levelData, levelTotal.ToString());
                player.GiveTextHint(Translation.ActiveTranslation.levelMessage.Replace("%level%", levelTotal.ToString()), 4);
                player.SendConsoleMessage(Translation.ActiveTranslation.levelMessage.Replace("%level%", levelTotal.ToString()), "green");
            }

            AddBigLevel(player, b);
        }

        /// <summary>
        /// Adds big levels to the player <paramref name="player"/> if it's possible.
        /// </summary>
        /// <param name="player">The player.</param><param name="bigLevel">The amount of big levels you want to add, shouldn't be negative.</param>
        public static void AddBigLevel(Player player, int bigLevel = 1)
        {
            if (!CanAddData(player) || bigLevel <= 0 || Config.expMultiplier == 0)
                return;

            AddDataFloat(player, bigLevelData, bigLevel);
            player.GiveTextHint(Translation.ActiveTranslation.bigLevelMessage.Replace("%bigLevel%", player.GetData(bigLevelData)), 6);
            player.SendConsoleMessage(Translation.ActiveTranslation.bigLevelMessage.Replace("%bigLevel%", player.GetData(bigLevelData)), "green");
        }

        /// <summary>
        /// Tries to delete the first <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}"/> with a <see cref="System.Collections.Generic.KeyValuePair{TKey, TValue}.Key"/> equal to <paramref name="key"/> from the <see cref="PlayerDbo.Data"/> <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>of the player <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player.</param><param name="key"> The key, put "all" to delete every KeyValuePair.</param>
        /// <returns><see langword="true"/> if the specified KeyValuePair was deleted ; otherwise, <see langword="false"/>.</returns>
        public static bool DeleteData(Player player, string key)
        {
            PlayerDbo pdo = DatabaseManager.PlayerRepository.FindByGameId(player.UserId);

            if (pdo == null)
                return false;

            if (key.ToLower() == "all")
            {
                foreach (string aKey in pdo.Data.Keys.ToList())
                {
                    pdo.Data.Clear();
                    pdo.Data.Remove(aKey);
                    DatabaseManager.PlayerRepository.Save(pdo);
                }
                return true;
            }

            if (player.GetData(key) == null)
                return false;

            string k = pdo.Data.Keys.FirstOrDefault(localKey => localKey == key);
            if (k == null)
                return false;

            pdo.Data.Remove(k);
            DatabaseManager.PlayerRepository.Save(pdo);
            return true;
        }

        /// <summary>
        /// Creates a SSSProfile with a lot of datas. You shouldn't use this method unless you're name is Bonjemus, which is very unlikely.
        /// </summary>
        /// <param name="player">The player.</param>
        public static void FirstLogin(Player player)
        {
            player.SetData(firstLogin, DateTime.UtcNow.AddHours(1).ToString());
            player.SetData(playtimeData, "0");
            player.SetData(bigLevelData, "0");
            player.SetData(levelData, "0");
            player.SetData(experienceData, "0");

            player.SetData(killsData, "0");
            player.SetData(dmgsInflictedData, "0");
            player.SetData(deathsData, "0");
            player.SetData(dmgasReceivedData, "0");
            player.SetData(escapesData, "0");
            player.SetData(startedRoundsData, "0");
            player.SetData(endedRoundsData, "0");
            player.SetData(roundsData, "0");

            player.SetData(teamkillsData, "0");
            player.SetData(kicksData, "0");
            player.SetData(bansData, "0");
            player.SetData(totalBanDurationData, "0");
        }


        public static readonly string dataConsent = "ConsentDNT";
        public static readonly string firstLogin = "First login";
        public static readonly string playtimeData = "Playtime";
        public static readonly string bigLevelData = "Prestige";
        public static readonly string levelData = "Level";
        public static readonly string experienceData = "XP";

        public static readonly string startedRoundsData = "Started round(s)";
        public static readonly string endedRoundsData = "Ended round(s)";
        public static readonly string roundsData = "Complete round(s)";
        public static readonly string escapesData = "Escape(s)";
        public static readonly string killsData = "Kill(s)";
        public static readonly string dmgsInflictedData = "Inflicted damage(s)";
        public static readonly string deathsData = "Death(s)";
        public static readonly string dmgasReceivedData = "Received damage(s)";

        public static readonly string teamkillsData = "Teamkill(s)";
        public static readonly string kicksData = "Kick(s)";
        public static readonly string bansData = "Ban(s)";
        public static readonly string totalBanDurationData = "Total ban duration";
    }
}