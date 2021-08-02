using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace CMDR.GetPermCmdModule
{
    public class GetPermCommand : SystemCommand
    {
        public CmdCategory c1 = new CmdCategory("utility", "Utility commands");
        public CmdCategory c2 = new CmdCategory("admin", "Administrative utility commands");
        public CmdCategory c3 = new CmdCategory("permissions", "Utility commands for permission management");

        public override CmdCategory[] Categories => new CmdCategory[]{ c1, c2, c3 };

        public override string commandid => "getperm";

        public override string helpsyntax => "<command>";

        public override string description => "gets the permission node of a command";

        public override string permissionnode => "sys.administration";

        public override bool setNoCmdPrefix => false;

        public override bool allowTerminal => true;

        public override bool allowDiscord => true;

        public override async Task OnExecuteFromDiscord(SocketGuild guild, SocketUser user, SocketTextChannel channel, SocketMessage messageobject, string fullmessage, string arguments_string, List<string> arguments)
        {
            if (arguments.Count == 1)
            {
                string id = arguments_string;
                if (GetBot().commands.Find(t => t.commandid.ToLower() == id.ToLower()) != null && GetBot().commands.Find(t => t.commandid.ToLower() == id.ToLower()).allowDiscord)
                {
                    await channel.SendMessageAsync("**Permission node of command "+ GetBot().commands.Find(t => t.commandid.ToLower() == id.ToLower()).commandid +": "+ GetBot().commands.Find(t => t.commandid == id).permissionnode+"**");
                }
                else
                {
                    await channel.SendMessageAsync("**Command not recognized.**");
                }
            }
            else
            {
                await channel.SendMessageAsync("**Invalid input.**");
            }
        }

        public override void OnExecuteFromTerminal(string fullcommand, string arguments_string, List<string> arguments)
        {
            if (arguments.Count == 1)
            {
                string id = arguments_string;
                if (GetBot().commands.Find(t => t.commandid.ToLower() == id.ToLower()) != null && GetBot().commands.Find(t => t.commandid.ToLower() == id.ToLower()).allowDiscord)
                {
                    Bot.WriteLine("Permission node of command " + GetBot().commands.Find(t => t.commandid.ToLower() == id.ToLower()).commandid + ": " + GetBot().commands.Find(t => t.commandid == id).permissionnode);
                }
                else
                {
                    Bot.WriteLine("Command not recognized. (this does not work with all commands, only discord commands)");
                }
            }
            else
            {
                Bot.WriteLine("Invalid input.");
            }
        }
    }
}
