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

        public CmdCategory c = new CmdCategory("noncategorized", "Commands that don't have a category");
        public override CmdCategory[] Categories => new CmdCategory[] { c };
        
        public override async Task OnExecuteFromDiscord(SocketGuild guild, SocketUser user, SocketTextChannel channel, SocketMessage messageobject, string fullmessage, string arguments_string, List<string> arguments)
        {
            int page = 0;
            List<string> pages = new List<string>();

            Emoji em1 = new Emoji("\u23EE");
            Emoji em2 = new Emoji("\u23ED");

            String currentPage = "```A list of known commands:";
            List<SystemCommand> commands = new List<SystemCommand>();

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    bool found = false;
                    foreach (CmdCategory cat in command.Categories) {
                        if (cat.name.ToLower() == arguments[0].ToLower()) {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    bool found = false;
                    foreach (CmdCategory cat in command.Categories) {
                        if (cat.name.ToLower().StartsWith(arguments[0].ToLower())) {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    bool found = false;
                    foreach (CmdCategory cat in command.Categories) {
                        if (cat.name.ToLower().Contains(arguments[0].ToLower())) {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    if (command.commandid.ToLower() != arguments[0].ToLower())
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    if (!command.commandid.ToLower().StartsWith(arguments[0].ToLower()))
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    if (!command.commandid.ToLower().Contains(arguments[0].ToLower()))
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    if (!command.description.ToLower().StartsWith(arguments[0].ToLower()))
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in GetBot().commands)
            {
                if (arguments.Count != 0) {
                    if (!command.description.ToLower().Contains(arguments[0].ToLower()))
                        continue;
                }

                if (!commands.Contains(command))
                    commands.Add(command);
            }

            foreach (SystemCommand command in commands) {
                if (!Bot.GetBot().CheckPermissions(command.permissionnode, user, guild)) {
                    continue;
                }
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

            Discord.Rest.RestUserMessage message = await channel.SendMessageAsync(pages[page]);

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
                   await message.AddReactionAsync(em2);
            }
        }

        public override void OnExecuteFromTerminal(string fullcommand, string arguments_string, List<string> arguments)
        {

        }
    }
}
