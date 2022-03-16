using Synapse.Command;

namespace SpecialSynapseStats.Commands
{
    [CommandInformation(
        Name = "ConsentDNT",
        Aliases = new[] { "" },
        Description = "Allows the server to store some datas linked to you.",
        Permission = "",
        Platforms = new[] { Platform.ClientConsole },
        Usage = ".ConsentDNT"
    )]
    class ConsentementDNTCommand : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            CommandResult result = new CommandResult();

            if (context.Player.GetData(PluginClass.dataConsent) == null || context.Player.GetData(PluginClass.dataConsent) != "true")
            {
                PluginClass.FirstLogin(context.Player);
                context.Player.SetData(PluginClass.dataConsent, "true");
                result.Message = PluginClass.Translation.ActiveTranslation.consentOkMessage;
                result.State = CommandResultState.Ok;
            }

            else
            {
                result.Message = PluginClass.Translation.ActiveTranslation.alreadyGivenConsentErrorMessage;
                result.State = CommandResultState.Error;
            }
            return result;
        }
    }
}