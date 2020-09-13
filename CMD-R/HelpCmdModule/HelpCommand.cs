using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

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
            StringWriter writer = new StringWriter();
            writer.WriteLine("```A list of known commands:");
            foreach (SystemCommand command in GetBot().commands)
            {
                writer.WriteLine(" - " + command.commandid + (command.helpsyntax == "" ? "" : " " + command.helpsyntax) + " - " + command.description);
            }
            writer.Write("```");
            var embed = new EmbedBuilder
            {
                // Embed property can be set within object initializer
                Title = "Hello world!",
                Description = "I am a description set by initializer."
            };
            // Or with methods
            embed.AddField("Field title",
                "Field value. I also support [hyperlink markdown](https://example.com)!")
                .WithAuthor(GetBot().client.CurrentUser)
                .WithFooter(footer => footer.Text = "I am a footer.")
                .WithColor(Color.Blue)
                .WithTitle("I overwrote \"Hello world!\"")
                .WithDescription("I am a description.")
                .WithUrl("https://example.com")
                .WithCurrentTimestamp()
                .Build();
            await channel.SendMessageAsync(writer.ToString(),false, embed.Build());
            writer.Close();
        }

        public override void OnExecuteFromTerminal(string fullcommand, string arguments_string, List<string> arguments)
        {

        }
    }
}
