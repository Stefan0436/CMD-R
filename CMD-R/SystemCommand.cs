using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace CMDR
{
    public abstract class SystemCommand
    {
        public Bot GetBot()
        {
            return Bot.GetBot();
        }

        public abstract CmdCategory Category { get; }
        public abstract string commandid { get; }
        public abstract string helpsyntax { get; }
        public abstract string description { get; }
        public abstract string permissionnode { get; }
        public abstract bool setNoCmdPrefix { get; }
        public abstract bool allowTerminal { get; }
        public abstract bool allowDiscord { get; }

        public abstract Task OnExecuteFromDiscord(SocketGuild guild, SocketUser user, SocketTextChannel channel, SocketMessage messageobject, string fullmessage, string arguments_string, List<string> arguments);
        public abstract void OnExecuteFromTerminal(string fullcommand, string arguments_string, List<string> arguments);
    }
}
