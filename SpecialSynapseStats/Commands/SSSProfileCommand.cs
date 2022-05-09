using Synapse.Command;
using Synapse;
using System.Linq;
using Synapse.Database;

namespace SpecialSynapseStats.Commands
{
    [CommandInformation(
        Name = "SSSProfile",
        Aliases = new[] { "" },
        Description = "Shows a SSS profile.",
        Permission = "",
        Platforms = new[] { Platform.ClientConsole },
        Usage = ".SSSProfile [Someone's nickname or ID] [Put 'staff' here if you want to see someone else's private stats]"
    )]
    internal class SSSProfileCommand : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            CommandResult result = new CommandResult();
            string playerUserId = context.Player.UserId;
            Synapse.Api.Player player = context.Player;

            if (context.Arguments.FirstOrDefault() != null && int.TryParse(context.Arguments.FirstOrDefault(), out int id))
            {
                player = Server.Get.GetPlayer(id);
                if (player == null)
                {
                    result.Message = PluginClass.Translation.ActiveTranslation.noPlayerWithIDErrorMessage;
                    result.State = CommandResultState.Error;
                    return result;
                }
                else
                    playerUserId = player.UserId;
            }

            else if (context.Arguments.FirstOrDefault() != null)
            {
                //string nickname = ""; Find solution later
                //foreach (string word in context.Arguments.ToList().FindAll(s => s != context.Arguments.Last()))
                //    nickname += word;

                player = Server.Get.Players.FirstOrDefault(p => p.NickName == context.Arguments.FirstOrDefault());
                //player = Server.Get.Players.FirstOrDefault(p => p.NickName == nickname);
                if (player == null)
                {
                    result.Message = PluginClass.Translation.ActiveTranslation.noPlayerWithNicknameErrorMessage;
                    result.State = CommandResultState.Error;
                    return result;
                }
                else
                    playerUserId = player.UserId;
            }

            bool seePrivateStats = context.Player == player; // If it's the same player he should be allowed to see his private stats.

            if (context.Arguments.Count > 1)
            {
                if (context.Arguments.Last().ToLower() == "staff")
                {
                    if (!context.Player.HasPermission("vanilla.PlayerSensitiveDataAccess"))
                    {
                        result.Message = PluginClass.Translation.ActiveTranslation.noPermissionErrorMessage;
                        result.State = CommandResultState.NoPermission;
                        return result;
                    }
                    else
                        seePrivateStats = true;
                }
            }

            PlayerDbo pbo = DatabaseManager.PlayerRepository.FindByGameId(playerUserId);
            string resultMessage = Server.Get.GetPlayer(playerUserId).NickName;

            foreach (var v in pbo.Data)
            {
                if (PluginClass.Config.secretStats.Contains(v.Key) && (!PluginClass.Config.securityStatsEnabled || !PluginClass.Config.securityStats.Contains(v.Key)))
                    continue;

                if (seePrivateStats)
                {
                    if (PluginClass.Translation.ActiveTranslation.translationDictionnary.TryGetValue(v.Key, out string translation))
                        resultMessage += "\n" + translation + " : " + v.Value;
                    else
                        resultMessage += "\n" + v.Key + " : " + v.Value;
                }

                else if (pbo.Data.TryGetValue(PluginClass.dataConsent, out string dataConsent) && dataConsent == "false")
                {
                    result.Message = PluginClass.Translation.ActiveTranslation.seeProfilErrorMessage;
                    result.State = CommandResultState.NoPermission;
                    return result;
                }

                else if (!PluginClass.Config.privateStats.Contains(v.Key))
                {
                    if (PluginClass.Translation.ActiveTranslation.translationDictionnary.TryGetValue(v.Key, out string translation))
                        resultMessage += "\n" + translation + " : " + v.Value;
                    else
                        resultMessage += "\n" + v.Key + " : " + v.Value;
                }
            }
            
            result.Message = PluginClass.Translation.ActiveTranslation.seeProfilOkMessage;
            //MEC.Timing.CallDelayed(1, () => context.Player.OpenReportWindow(reportWindowMessage)); Limited amount of lines.
            context.Player.SendConsoleMessage(resultMessage, "white");

            result.State = CommandResultState.Ok;
            return result;
        }
    }
}