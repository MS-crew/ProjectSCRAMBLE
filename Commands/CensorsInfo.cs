using System;
using System.Linq;
using CommandSystem;
using System.Collections.Generic;
using ProjectSCRAMBLE.Extensions;

namespace ProjectSCRAMBLE.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CensorsInfo : ICommand
    {
        public string Command => "scp96censors";
        public string[] Aliases => [];
        public string Description => "Lists active Scp96 censors.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (PlayerExtensions.Scp96Censors.Count == 0)
            {
                response = "No active SCP-96 sensors.";
                return true;
            }

            IEnumerable<string> lines = PlayerExtensions.Scp96Censors.Select(data =>
            {
                string schematicName = data.Value?.name ?? "null";
                return $"{data.Key.Nickname} -> {schematicName}";
            });

            response = "Active SCP-96 Sensor List:\n" + string.Join("\n", lines);
            return true;
        }
    }
}
