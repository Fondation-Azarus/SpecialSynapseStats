using Synapse.Translation;
using System.Collections.Generic;

namespace SpecialSynapseStats
{
    public class Translations : IPluginTranslation
    {
        public string cantAddDataHint = "Welcome, we detected that you enabled 'Do Not Track' which means that we can't store any data from you, please go in your console for more info.";

        public string cantAddDataConsoleMessage = "We detected that you enabled 'Do Not Track' which means that we can't store any data from you like the amount of rounds you played or kills you got, it also means you won't be able to use the XP system, however you can give your consent by writing `.ConsentDNT` in this console.";
        
        public string xpMessage = "+%xp%XP";
        
        public string levelMessage = "Level %level%";
        
        public string bigLevelMessage = "Congratulations ! You're prestige %bigLevel% !";
        
        public string killMessage = "Killed %victim%";
        
        public string escapeMessage = "Escaped";
        
        public string escapeAssistMessage = "%player% escaped";
        
        public string captureMessage = "%player% captured";
        
        public string scpKillAssistMessage = "Assisted kill on SCP";
        
        public string scpEscapeMessage = "SCP escaped";
        
        public string warheadMessage = "Warhead exploded";
        
        public string scp106grabMessage = "none";
        
        public string consentOkMessage = "Your consent was received, you should be able to access new features !";
        
        public string alreadyGivenConsentErrorMessage = "You already gave your consent.";
        
        public string DNTNotEnabledErrorMessage = "You didn't enable the `Do Not Track` parameter. REVOKING YOUR CONSENT WILL IMMEDIATELY DELETE ALL YOUR DATA, WE CAN'T RESTORE THEM AFTERWARDS SO BE SURE YOU REALLY WANT T0 DO THIS BEFORE ENTERING THIS COMMAND.";
        
        public string revokeDNTAddConfirmationErrorMessage = "Add 'Confirmation' to this command before entering it if you're sure you want to revoke your consent. REVOKING YOUR CONSENT WILL IMMEDIATELY DELETE ALL YOUR DATA, WE CAN'T RESTORE THEM AFTERWARDS SO BE SURE YOU REALLY WANT T0 DO THIS BEFORE ENTERING THIS COMMAND.";
        
        public string revokeDNTOkMessage = "Data successfully deleted.";

        public string noPlayerWithIDErrorMessage = "No player with this ID was found.";

        public string noPlayerWithNicknameErrorMessage = "No player with this nickname was found.";

        public string noPermissionErrorMessage = "You need the 'vanilla.PlayerSensitiveDataAccess' to access private stats.";

        public string seeProfilOkMessage = "You should see a SSS Profile.";

        public IDictionary<string, string> translationDictionnary = new Dictionary<string, string>
        {
            { PluginClass.firstLogin, "First login" },
            { PluginClass.playtimeData, "Playtime" },
            { PluginClass.bigLevelData, "Prestige" },
            { PluginClass.levelData, "Level" },
            { PluginClass.experienceData, "XP" },
            { PluginClass.startedRoundsData, "Started round(s)" },
            { PluginClass.endedRoundsData, "Ended round(s)" },
            { PluginClass.roundsData, "Complete round(s)" },
            { PluginClass.escapesData, "Escape(s)" },
            { PluginClass.killsData, "Kill(s)" },
            { PluginClass.dmgsInflictedData, "Inflicted damage(s)" },
            { PluginClass.deathsData, "Death(s)" },
            { PluginClass.dmgasReceivedData, "Received damage(s)" },
            { PluginClass.teamkillsData, "Teamkill(s)" },
            { PluginClass.kicksData, "Kick(s)" },
            { PluginClass.bansData, "Ban(s)" },
            { PluginClass.totalBanDurationData, "Total ban duration" }
        };
    }
}
