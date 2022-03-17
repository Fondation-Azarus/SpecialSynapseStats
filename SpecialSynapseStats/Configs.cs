using Synapse.Config;
using System.Collections.Generic;
using System.ComponentModel;

namespace SpecialSynapseStats
{
    public class Configs : AbstractConfigSection
    {
        [Description("Before modifying configs, you should read the GitHub wiki, if you have any questions you can always contact me on Discord ! Plug-in disabled ? :")]
        public bool disabled = false;

        [Description("What's your UTC ? :")]
        public int utc = 1;

        [Description("XP multiplier :")]
        public float xpMultiplier = 1;

        [Description("XP needed for first level :")]
        public float firstLevelXpNeeded = 200;

        [Description("Should this plug-in use 'qXpLevel' (if false it will use 'rXpLevel')")]
        public bool xpRorQ = false;

        [Description("Adds to the current level's XP needed to get the next one's :")]
        public float rXpLevel = 30;

        [Description("Multiplies the current level's XP needed to get the next one's :")]
        public float qXpLevel = 1.05f;

        [Description("How many levels are necessary to get one big level :")]
        public int nmbrLevelNeededBigLevel = 35;

        [Description("The amount of XP a specific role or team should get for a specific action :")]
        public List<XpRewardsRoleID> listXpRewardsRoleID = new List<XpRewardsRoleID>()
            {
                new XpRewardsRoleID{ roleID = (int)RoleType.ClassD, survivalXp = 5, escapeXp = 100, roleIDKillXp = new Dictionary<int,float>(){ {(int)RoleType.Scientist, 20}, {(int)RoleType.FacilityGuard, 35}}, teamKillXp = new Dictionary<Team, float>() { { Team.MTF, 40 }, {Team.SCP, 80 } } },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scientist, survivalXp = 6, escapeXp = 100, roleIDKillXp = new Dictionary<int,float>(){ {(int)RoleType.ClassD, 10}}, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 40 }, {Team.SCP, 100} }, scpKillAssistXp = 50 },
                new XpRewardsRoleID{ roleID = (int)RoleType.FacilityGuard, survivalXp = 7, escapeAssistXp = 50, captureXp = 50, roleIDKillXp = new Dictionary<int,float>(){ {(int)RoleType.ClassD, 8}}, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 35 }, {Team.SCP, 100} }, scpKillAssistXp = 50 },
                new XpRewardsRoleID{ team = Team.MTF, survivalXp = 7, escapeAssistXp = 60, captureXp = 35, roleIDKillXp = new Dictionary<int,float>(){ {(int)RoleType.ClassD, 10}}, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 30 }, {Team.SCP, 100} }, scpKillAssistXp = 50 },
                new XpRewardsRoleID{ team = Team.CHI, survivalXp = 7, escapeAssistXp = 20, captureXp = 42, roleIDKillXp = new Dictionary<int,float>(){ {(int)RoleType.Scientist, 5}, { (int)RoleType.FacilityGuard, 20 } }, teamKillXp = new Dictionary<Team, float>() { { Team.MTF, 30 }, {Team.SCP, 10} } },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp173, survivalXp = 2.5f, escapeXp = 30, roleIDKillXp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5}, { (int)RoleType.FacilityGuard, 5 } }, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 3 }, {Team.MTF, 5} } },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp096, survivalXp = 2.5f, escapeXp = 30, roleIDKillXp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5}, { (int)RoleType.FacilityGuard, 5 } }, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 3 }, {Team.MTF, 5} } },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp049, survivalXp = 5, escapeXp = 50, roleIDKillXp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 4 }, { (int)RoleType.Scientist, 4}, { (int)RoleType.FacilityGuard, 4 } }, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 3 }, {Team.MTF, 4} } },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp106, survivalXp = 2, escapeXp = 10 },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp079, survivalXp = 2 },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp93989, survivalXp = 2.5f, escapeXp = 50, roleIDKillXp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5 }, { (int)RoleType.FacilityGuard, 5 } }, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 4 }, {Team.MTF, 5 } } },
                new XpRewardsRoleID{ roleID = (int)RoleType.Scp93953, survivalXp = 2.5f, escapeXp = 50, roleIDKillXp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5 }, { (int)RoleType.FacilityGuard, 5 } }, teamKillXp = new Dictionary<Team, float>() { { Team.CHI, 4 }, {Team.MTF, 5 } } },
            };

        [Description("XP given to connected players in general :")]
        public float xpGeneralPerMinute = 7.5f;

        [Description("Should Facility Guards be considered as a part of the MTF team ? :")]
        public bool guardMTF = false;

        [Description("Can a SCP escape ? :")]
        public bool scpEscape = true;

        [Description("Must the escaped person's allies be at the surface to receive assist escape XP ? :")]
        public bool assistEscapeSurfaceZone = false;

        [Description("XP given to SCP-106 when he grabs someone :")]
        public float scp106GrabXp = 6;

        [Description("Should SCP killed by Alpha Warhead other than instances count as a kill assist on SCP for SCP Foundation staff ? :")]
        public bool warheadScpKillAssist = true;

        [Description("Should player spawning in a specific class be registered ? :")]
        public bool spawnClassRegister = false;

        [Description("Should item usage be registered ? :")]
        public bool itemUseRegister = false;

        [Description("A list of stats that should be private :")]
        public List<string> privateStats = new List<string> { PluginClass.dataConsent, PluginClass.teamkillsData, PluginClass.kicksData, PluginClass.bansData, PluginClass.totalBanDurationData };


        public class XpRewardsRoleID // It could be a struct but they can't be null so I can't easily know if the specified one exists
        {
            public int roleID;
            public Team team;
            public float survivalXp;
            public float escapeXp;
            public float escapeAssistXp;
            public float captureXp;
            public IDictionary<int, float> roleIDKillXp;
            public IDictionary<Team, float> teamKillXp;
            public float scpKillAssistXp;
            public float scpEscapeXp;
            public float warheadXp;

        }
    }
}