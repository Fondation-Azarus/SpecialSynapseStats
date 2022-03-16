using Synapse.Command;
using Synapse;
using System.Linq;
using Synapse.Database;

namespace SpecialSynapseStats.Commands
{
    [CommandInformation(
        Name = "SSSProfil",
        Aliases = new[] { "" },
        Description = "Shows a SSS profil.",
        Permission = "",
        Platforms = new[] { Platform.ClientConsole },
        Usage = ".SSSProfil [Someone's nickname or ID] [Put 'staff' here if you want to see someone else's private stats]"
    )]
    class SSSProfilCommand : ISynapseCommand
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
                player = Server.Get.Players.FirstOrDefault(p => p.NickName == context.Arguments.FirstOrDefault());
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
                if (context.Arguments.ElementAt(1).ToLower() == "staff")
                {
                    if (!context.Player.HasPermission("vanilla.PlayerSensitiveDataAccess"))
                    {
                        result.Message = PluginClass.Translation.ActiveTranslation.noPermissionErrorMessage;
                        result.State = CommandResultState.Error;
                        return result;
                    }
                    else
                        seePrivateStats = true;
                }
            }

            PlayerDbo pbo = DatabaseManager.PlayerRepository.FindByGameId(playerUserId);
            string reportWindowMessage = Server.Get.GetPlayer(playerUserId).NickName;

            foreach (var v in pbo.Data)
            {
                if (seePrivateStats)
                {
                    if (PluginClass.Translation.ActiveTranslation.translationDictionnary.TryGetValue(v.Key, out string translation))
                        reportWindowMessage += "\n" + translation + " : " + v.Value;
                    else
                        reportWindowMessage += "\n" + v.Key + " : " + v.Value;
                }

                else if (!PluginClass.Config.privateStats.Contains(v.Key))
                    reportWindowMessage += "\n" + v.Key + " : " + v.Value;
            }

            context.Player.OpenReportWindow(reportWindowMessage);
            result.Message = PluginClass.Translation.ActiveTranslation.seeProfilOkMessage;
            result.State = CommandResultState.Ok;
            return result;
        }
    }
}