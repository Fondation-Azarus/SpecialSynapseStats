using Synapse.Config;
using System.Collections.Generic;
using System.ComponentModel;

namespace SpecialSynapseStats
{
    public class Configs : AbstractConfigSection
    {
        [Description("Before modifying configs, you should read the github, if you have any questions you can always contact me on Discord ! Plug-in disabled ? :")]
        public bool disabled = false;

        [Description("XP multiplicator :")]
        public float expMultiplier = 1;

        [Description("XP needed for first level :")]
        public float firstLevelExpNeeded = 200;

        [Description("Should this plug-in use 'qXpLevel' (if false it will use 'rXpLevel')")]
        public bool xpRorQ = false;

        [Description("Adds to the current level's XP needed to get the next one's :")]
        public float rExpLevel = 30;

        [Description("Multiplies the current level's XP needed to get the next one's :")]
        public float qExpLevel = 1.05f;

        [Description("How many levels are necessary to get one big level :")]
        public int nmbrLevelNeededBigLevel = 35;

        [Description("The amount of XP a specific role or team should get for a specific action :")]
        public List<ExpRewardsRoleID> listExpRewardsRoleID = new List<ExpRewardsRoleID>()
            {
                new ExpRewardsRoleID{ roleID = (int)RoleType.ClassD, survivalExp = 5, escapeExp = 100, roleIDKillExp = new Dictionary<int,float>(){ {(int)RoleType.Scientist, 20}, {(int)RoleType.FacilityGuard, 35}}, teamKillExp = new Dictionary<Team, float>() { { Team.MTF, 40 }, {Team.SCP, 80 } } },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scientist, survivalExp = 6, escapeExp = 100, roleIDKillExp = new Dictionary<int,float>(){ {(int)RoleType.ClassD, 10}}, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 40 }, {Team.SCP, 100} }, scpKillAssistExp = 50 },
                new ExpRewardsRoleID{ roleID = (int)RoleType.FacilityGuard, survivalExp = 7, escapeAssistExp = 50, captureExp = 50, roleIDKillExp = new Dictionary<int,float>(){ {(int)RoleType.ClassD, 8}}, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 35 }, {Team.SCP, 100} }, scpKillAssistExp = 50 },
                new ExpRewardsRoleID{ team = Team.MTF, survivalExp = 7, escapeAssistExp = 60, captureExp = 35, roleIDKillExp = new Dictionary<int,float>(){ {(int)RoleType.ClassD, 10}}, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 30 }, {Team.SCP, 100} }, scpKillAssistExp = 50 },
                new ExpRewardsRoleID{ team = Team.CHI, survivalExp = 7, escapeAssistExp = 20, captureExp = 42, roleIDKillExp = new Dictionary<int,float>(){ {(int)RoleType.Scientist, 5}, { (int)RoleType.FacilityGuard, 20 } }, teamKillExp = new Dictionary<Team, float>() { { Team.MTF, 30 }, {Team.SCP, 10} } },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp173, survivalExp = 2.5f, escapeExp = 30, roleIDKillExp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5}, { (int)RoleType.FacilityGuard, 5 } }, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 3 }, {Team.MTF, 5} } },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp096, survivalExp = 2.5f, escapeExp = 30, roleIDKillExp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5}, { (int)RoleType.FacilityGuard, 5 } }, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 3 }, {Team.MTF, 5} } },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp049, survivalExp = 5, escapeExp = 50, roleIDKillExp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 4 }, { (int)RoleType.Scientist, 4}, { (int)RoleType.FacilityGuard, 4 } }, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 3 }, {Team.MTF, 4} } },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp106, survivalExp = 2, escapeExp = 10 },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp079, survivalExp = 2 },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp93989, survivalExp = 2.5f, escapeExp = 50, roleIDKillExp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5 }, { (int)RoleType.FacilityGuard, 5 } }, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 4 }, {Team.MTF, 5 } } },
                new ExpRewardsRoleID{ roleID = (int)RoleType.Scp93953, survivalExp = 2.5f, escapeExp = 50, roleIDKillExp = new Dictionary<int,float>(){ { (int)RoleType.ClassD, 5 }, { (int)RoleType.Scientist, 5 }, { (int)RoleType.FacilityGuard, 5 } }, teamKillExp = new Dictionary<Team, float>() { { Team.CHI, 4 }, {Team.MTF, 5 } } },
            };

        [Description("XP given to connected players in general :")]
        public float expGeneralPerMinute = 7.5f;

        [Description("Should Facility Guards be considered as a part of the MTF team ? :")]
        public bool guardMTF = false;

        [Description("Can a SCP escape ? :")]
        public bool scpEscape = true;

        [Description("Must the escaped person's allies be at the surface to receive assist escape XP ? :")]
        public bool assistEscapeSurfaceZone = false;

        [Description("XP given to SCP-106 when he grabs someone :")]
        public float scp106GrabExp = 6;

        [Description("Should killing SCP other than instances count as a kill assist on SCP for SCP Foundation staff ? :")]
        public bool warheadScpKillAssist = true;


        [Description("A list of stats that should be private :")]
        public List<string> privateStats = new List<string> { PluginClass.dataConsent, PluginClass.teamkillsData, PluginClass.kicksData, PluginClass.bansData, PluginClass.totalBanDurationData };


        public class ExpRewardsRoleID // It could be a struct but they can't be null so I can't easily know if the specified one exists
        {
            public int roleID;
            public Team team;
            public float survivalExp;
            public float escapeExp;
            public float escapeAssistExp;
            public float captureExp;
            public IDictionary<int, float> roleIDKillExp;
            public IDictionary<Team, float> teamKillExp;
            public float scpKillAssistExp;
            public float scpEscapeExp;
            public float warheadExp;

        }
    }
}