using Synapse.Command;
using Synapse;
using System.Linq;
using Synapse.Database;
using System;

namespace SpecialSynapseStats.Commands
{
    [CommandInformation(
        Name = "RevokeConsentDNT",
        Aliases = new[] { "" },
        Description = "Revokes your consent which means we can't store any data from you. REVOKING YOUR CONSENT WILL IMMEDIATELY DELETE ALL YOUR DATA, WE CAN'T RESTORE THEM AFTERWARDS SO BE SURE YOU REALLY WANT T0 DO THIS BEFORE ENTERING THIS COMMAND.",
        Permission = "",
        Platforms = new[] { Platform.ClientConsole },
        Usage = ".RevokeConsentDNT"
    )]
    class RevokeConsentDNTCommand : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            CommandResult result = new CommandResult();

            if (!context.Player.DoNotTrack)
            {
                result.Message = PluginClass.Translation.ActiveTranslation.DNTNotEnabledErrorMessage;
                result.State = CommandResultState.Error;
            }

            else if (context.Arguments.FirstOrDefault() == "Confirmation")
            {
                PluginClass.DeleteData(context.Player, "all");
                result.Message = PluginClass.Translation.ActiveTranslation.revokeDNTOkMessage;
                result.State = CommandResultState.Ok;
            }

            else
            {
                result.Message = PluginClass.Translation.ActiveTranslation.revokeDNTAddConfirmationErrorMessage;
                result.State = CommandResultState.Error;
            }

            return result;
        }
    }
}