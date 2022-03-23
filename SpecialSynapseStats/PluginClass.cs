using Synapse.Api.Plugin;
using Synapse.Translation;
using Synapse.Api;
using System;

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
        Version = "1.1.0"
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

        internal static void FirstLogin(Player player)
        {
            player.SetData(firstLogin, DateTime.UtcNow.AddHours(Config.utc).ToString());

            System.Collections.Generic.List<string> data = new System.Collections.Generic.List<string>
            {
                playtimeData,
                bigLevelData,
                levelData,
                experienceData,
                killsData,
                dmgsInflictedData,
                deathsData,
                dmgasReceivedData,
                escapesData,
                startedRoundsData,
                endedRoundsData,
                roundsData,
                teamkillsData,
                kicksData,
                bansData,
                totalBanDurationData,
            };
            foreach (string datum in data)
                player.SetData(datum, "0");
        }


        public const string dataConsent = "ConsentDNT";
        public const string firstLogin = "First login";
        public const string playtimeData = "Playtime";
        public const string bigLevelData = "BigLevel";
        public const string levelData = "Level";
        public const string experienceData = "XP";

        public const string startedRoundsData = "Started round";
        public const string endedRoundsData = "Ended round";
        public const string roundsData = "Complete round";
        public const string escapesData = "Escape";
        public const string killsData = "Kill";
        public const string dmgsInflictedData = "Inflicted damage";
        public const string deathsData = "Death";
        public const string dmgasReceivedData = "Received damage";

        public const string teamkillsData = "Teamkill";
        public const string kicksData = "Kick";
        public const string bansData = "Ban";
        public const string totalBanDurationData = "Total ban duration";
    }
}