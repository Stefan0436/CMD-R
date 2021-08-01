using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Threading;

namespace CMDR.HelpCmdModule
{
    public class HelpCommand : SystemCommand
    {
        public override string commandid => "help";

        public override string helpsyntax => "[<category/command>]";

        public override string description => "shows a list of commands or a command's syntax";

        public override string permissionnode => "sys.anyone";

        public override bool setNoCmdPrefix => false;

        public override bool allowTerminal => false;

        public override bool allowDiscord => true;

        public CmdCategory c = new CmdCategory("Not-categorized-cmds", "Commands that don't have a category");
        public override CmdCategory Category => c;

        public override async Task OnExecuteFromDiscord(SocketGuild guild, SocketUser user, SocketTextChannel channel, SocketMessage messageobject, string fullmessage, string arguments_string, List<string> arguments)
        {
            int page = 0;
            List<string> pages = new List<string>();

            Emoji em1 = new Emoji("\u23EE");
            Emoji em2 = new Emoji("\u23ED");

            String currentPage = "```A list of known commands:";
            foreach (SystemCommand command in GetBot().commands)
            {
                if (currentPage.Length + 3 >= 2000) {
                    currentPage += "```";
                    pages.Add(currentPage);
                    currentPage = "```A list of known commands (page " + pages.Count + "):";
                }
                currentPage += ("\n - " + command.commandid + (command.helpsyntax == "" ? "" : " " + command.helpsyntax) + " - " + command.description);    
            }
            if (currentPage.Length != ("```A list of known commands (page " + pages.Count + "):").Length)
            {
                currentPage += "```";
                pages.Add(currentPage);
            }

            Discord.Rest.RestUserMessage message = channel.SendMessageAsync(pages[page]).GetAwaiter().GetResult();

            Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> handler = new Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task>((arg1, arg2, arg3) =>
            {
                if (arg3.MessageId == message.Id && arg3.Channel.Id == message.Channel.Id)
                {
                    if (arg3.Emote.Name == em1.Name)
                    {
                        bool changed = false;
                        var v = message.GetReactionUsersAsync(em1, int.MaxValue).GetAsyncEnumerator();
                        while (true)
                        {
                            if (v.Current != null)
                                foreach (IUser usr in v.Current)
                                    if (!usr.Id.Equals(Bot.GetBot().client.CurrentUser.Id))
                                    {
                                        message.RemoveReactionAsync(em1, usr).GetAwaiter().GetResult();
                                        changed = true;
                                    }
                            if (!v.MoveNextAsync().GetAwaiter().GetResult())
                                break;
                        }
                        if (changed)
                        {
                            page--;

                            var oldmsg = message;
                            message = channel.SendMessageAsync(pages[page]).GetAwaiter().GetResult();
                            oldmsg.DeleteAsync().GetAwaiter().GetResult();

                            if (page != 0)
                                message.AddReactionAsync(em1).GetAwaiter().GetResult();
                            if (page != pages.Count - 1)
                                message.AddReactionAsync(em2).GetAwaiter().GetResult();
                        }
                    }
                    if (arg3.Emote.Name == em2.Name)
                    {
                        bool changed = false;
                        var v = message.GetReactionUsersAsync(em2, int.MaxValue).GetAsyncEnumerator();
                        while (true)
                        {
                            if (v.Current != null)
                                foreach (IUser usr in v.Current)
                                    if (!usr.Id.Equals(Bot.GetBot().client.CurrentUser.Id))
                                    {
                                        message.RemoveReactionAsync(em1, usr).GetAwaiter().GetResult();
                                        changed = true;
                                    }
                            if (!v.MoveNextAsync().GetAwaiter().GetResult())
                                break;
                        }
                        if (changed)
                        {
                            page++;

                            var oldmsg = message;
                            message = channel.SendMessageAsync(pages[page]).GetAwaiter().GetResult();
                            oldmsg.DeleteAsync().GetAwaiter().GetResult();

                            if (page != 0)
                                message.AddReactionAsync(em1).GetAwaiter().GetResult();
                            if (page != pages.Count - 1)
                                message.AddReactionAsync(em2).GetAwaiter().GetResult();
                        }
                    }
                }
                return null;
            });

            Bot.GetBot().client.ReactionAdded += handler;
            Bot.GetBot().client.MessageDeleted += (arg11, arg22) =>
            {
                if (arg11.Id == message.Id)
                    Bot.GetBot().client.ReactionAdded -= handler;
                return null;
            };
            
            if (pages.Count != 1)
            {
                if (page != pages.Count)
                    message.AddReactionAsync(em2).GetAwaiter().GetResult();
            }
        }

        public override void OnExecuteFromTerminal(string fullcommand, string arguments_string, List<string> arguments)
        {

        }
    }
}