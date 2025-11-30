using System;
using System.Linq;
using CommandSystem;
using System.Collections.Generic;

namespace ProjectSCRAMBLE.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ScramblesInfo : ICommand
    {
        public string Command => "activescrambles";
        public string[] Aliases => [];
        public string Description => "Show the complete list of ActiveScramblePlayers.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            if (ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.Count == 0)
            {
                response = "No active scramble player.";
                return true;
            }

            IEnumerable<string> lines = ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.Select(data =>
            {
                string mainPlayer = data.Key.Nickname;
                List<string> scrambled = [.. data.Value.Select(p => p.Nickname)];
                return $"{mainPlayer} -> [{string.Join(", ", scrambled)}]";
            });

            response = "Active Scramble List:\n" + string.Join("\n", lines);
            return true;
        }
    }
}
