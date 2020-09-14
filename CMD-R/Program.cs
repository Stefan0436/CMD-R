using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.WebSocket;

namespace CMDR
{
    public partial class Bot
    {
        public static void WriteLine(string msg = "")
        {
            Terminal.WriteLine(msg);
        }

        public Bot()
        {
            _bot = this;
        }

        public List<string> DefaultPermissions = new List<string>() { "sys.anyone" };
        static Bot _bot;
        public List<Server> servers;

        public List<BotModule> modules = new List<BotModule>();
        public List<SystemCommand> commands = new List<SystemCommand>();
        public List<CmdCategory> CmdCategories = new List<CmdCategory>();
        public static Bot GetBot()
        {
            return _bot;
        }
        public string token = "";
        public string path;
        public ulong guild = 0;
        public DiscordSocketClient client;
        public static void Main(string[] args)
        => new Bot().MainAsync().GetAwaiter().GetResult();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task MainAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ConsoleColor b = Console.ForegroundColor;
            Console.ForegroundColor = b;

            path = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf(Path.DirectorySeparatorChar));
            Bot.WriteLine("Booting CMD-R...");
            Bot.WriteLine("AerialWorks CMD-R is Free(Libre) and Open Source Software (FLOSS),\nand will always be licensed under the GPL license.");
            Bot.WriteLine("------------------------------------------------------------------");
            Bot.WriteLine();
            Bot.WriteLine("System path: " + path);
            Directory.CreateDirectory(path + "/Modules");

            Bot.WriteLine();
            Bot.WriteLine("Loading token...");
            if (File.Exists(path + "/Bot.cfg"))
            {
                token = File.ReadAllText(path + "/Bot.cfg").Replace("\n", "");
            }
            else
            {
                token = Terminal.ReadLine("Bot token").Replace("\n", "");
                Bot.WriteLine("\nSaved to file Bot.cfg, edit it to change the token.");
                File.WriteAllText(path + "/Bot.cfg", token);
            }

            Bot.WriteLine("Starting Discord.NET framework...");
            client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("Starting CMD-R...");

            while (client.ConnectionState == ConnectionState.Connecting || client.ConnectionState == ConnectionState.Disconnected) { Thread.Sleep(100); }
            while (client.Guilds.Count == 0 || client.Guilds.FirstOrDefault().Name == null || client.Guilds.FirstOrDefault().Name == "") { Thread.Sleep(100); }
            Bot.WriteLine("Preparing server config directories...");
            Directory.CreateDirectory(path + "/Server Configs");

            Bot.WriteLine("Reading default permission list...");
            if (File.Exists(path + "/permsdefault.cfg")) DefaultPermissions = File.ReadAllLines(path + "/permsdefault.cfg").ToList();
            else File.WriteAllLines(path + "/permsdefault.cfg", DefaultPermissions);
            int i2 = 0;
            foreach (string str in new List<string>(DefaultPermissions))
            {
                if (str == "") DefaultPermissions.RemoveAt(i2);
                i2++;
            }
            Bot.WriteLine("Loaded " + DefaultPermissions.Count + " permission node"+(DefaultPermissions.Count == 1 ? "" : "s"));

            Server.LoadAllServers(out servers);

            client.RoleCreated += Client_RoleCreated;
            client.JoinedGuild += Client_JoinedGuild;
            client.LeftGuild += Client_LeftGuild;

            Bot.WriteLine("Synchronizing servers attatched to the bot...");
            foreach (SocketGuild server in client.Guilds)
            {
                SyncServer(server);
            }

            Bot.WriteLine();
            Bot.WriteLine("Loading modules...");
            Bot.WriteLine("Loading build-in modules...");

            foreach (Type module in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (module.BaseType == typeof(BotModule))
                {
                    Bot.WriteLine("Loading module class: " + module.Name);
                    var script = Activator.CreateInstance(module) as BotModule;
                    Bot.WriteLine("Loading module: " + script.id + "...");
                    Bot.WriteLine("Module description: " + script.moduledesctiption);
                    modules.Add(script);
                    Bot.WriteLine("Pre-initializing module: " + script.id + "...");
                    script.PreInit(this);
                    Bot.WriteLine("Loaded module: " + script.id + "...");
                }
            }
            Bot.WriteLine();
            Bot.WriteLine("Loading modules from the Modules directory...");
            foreach (DirectoryInfo info in new DirectoryInfo(path + "/Modules").GetDirectories())
            {
                if (File.Exists(info.FullName + "/" + info.Name + ".dll"))
                {
                    Bot.WriteLine("Loading module file: " + info.FullName + "/" + info.Name + ".dll");
                    Assembly asm = Assembly.LoadFrom(info.FullName + "/" + info.Name + ".dll");
                    Type type = asm.GetType(info.Name + "." + info.Name);
                    var script = Activator.CreateInstance(type) as BotModule;

                    if (script == null)
                    {
                        Bot.WriteLine("Module file: '" + info.FullName + "/" + info.Name + ".dll' does not have a class named " + info.Name + " in a namespace named " + info.Name + ", cannot load it, skipping...");
                    }
                    else
                    {
                        Bot.WriteLine("Loading module: " + script.id + "...");
                        Bot.WriteLine("Module description: " + script.moduledesctiption);
                        modules.Add(script);
                        Bot.WriteLine("Pre-initializing module: " + script.id + "...");
                        script.PreInit(this);
                        Bot.WriteLine("Loaded module: " + script.id + "...");
                    }
                }
            }
            Bot.WriteLine();
            Bot.WriteLine("Initializing modules...");

            foreach (BotModule mod in modules)
            {
                Bot.WriteLine("Initializing module: " + mod.id + "...");
                mod.Init(this);
            }
            Bot.WriteLine();
            Bot.WriteLine("Registering module commands...");

            foreach (BotModule mod in modules)
            {
                Bot.WriteLine("Registering commands of " + mod.id + "...");
                mod.RegisterCommands(this);
            }
            Bot.WriteLine();
            Bot.WriteLine("Post-initializing modules...");

            foreach (BotModule mod in modules)
            {
                Bot.WriteLine("Post-initializing module: " + mod.id + "...");
                mod.PostInit(this);
            }
            Bot.WriteLine();
            Bot.WriteLine("Module loading completed. " + modules.Count + " modules loaded.");

            Bot.WriteLine();
            Bot.WriteLine("Execute quit or exit to close.");

            client.MessageReceived += onMessage;

            await client.SetGameAsync("");
            while (true)
            {
                string command = Terminal.ReadLine().Replace("\0","");
                if (command == "quit" || command == "exit")
                {
                    await client.SetGameAsync("Shutting down...");

                    await client.SetStatusAsync(UserStatus.Invisible);
                    await client.StopAsync();
                    client.Dispose();
                    Environment.Exit(0);
                }
                else if (command == "clrgame")
                {
                    await client.SetGameAsync("");
                }
                else if (command.StartsWith("setgame ", StringComparison.CurrentCulture))
                {
                    string status = command.Substring("setgame ".Length);
                    await client.SetGameAsync(status);
                }
                else if (command.StartsWith("setstatus ", StringComparison.CurrentCulture))
                {
                    string status = command.Substring("setstatus ".Length);
                    if (status == "online") await client.SetStatusAsync(UserStatus.Online);
                    else if (status == "offline (broken, sets to invisible)") await client.SetStatusAsync(UserStatus.Invisible); // Offline does nothing, this works better
                    else if (status == "invisible") await client.SetStatusAsync(UserStatus.Invisible);
                    else if (status == "do-not-disturb") await client.SetStatusAsync(UserStatus.DoNotDisturb);
                    else if (status == "idle") await client.SetStatusAsync(UserStatus.Idle);
                    else if (status == "afk") await client.SetStatusAsync(UserStatus.AFK);
                    else
                    {
                        Bot.WriteLine("Status types:");
                        Bot.WriteLine("online");
                        Bot.WriteLine("offline");
                        Bot.WriteLine("invisible");
                        Bot.WriteLine("do-not-disturb");
                        Bot.WriteLine("idle");
                        Bot.WriteLine("afk");
                    }
                }
                else if (command.StartsWith("msg ", StringComparison.CurrentCulture))
                {
                    if (guild != 0)
                    {
                        string str = command.Substring("str ".Length);
                        if (str.Split(' ').Count() >= 2)
                        {
                            string channel = str.Remove(str.IndexOf(" ", StringComparison.CurrentCulture));
                            str = str.Substring(str.IndexOf(" ", StringComparison.CurrentCulture) + 1);
                            if (client.GetGuild(guild).TextChannels.Count(t => t.Name == channel) > 1)
                            {
                                b = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Bot.WriteLine("[WARNING, MSG CMD] Multiple channels with the same name¸ sending message to "+ client.GetGuild(guild).TextChannels.Count(t => t.Name == channel)+" channels, recommend troubleshooting.");
                                Console.ForegroundColor = b;
                            }

                            foreach (SocketTextChannel channel2 in client.GetGuild(guild).TextChannels)
                            {
                                if (channel2.Name == channel) await channel2.SendMessageAsync(str);
                            }
                        }
                    }
                    else
                    {
                        Bot.WriteLine("Server not set, please run selectserver first.");
                    }
                }
                else if (command == "selectserver")
                {
                    List<SocketGuild> guilds = new List<SocketGuild>(client.Guilds);
                    SocketGuild guild3 = null;
                    int i = 1;
                    Bot.WriteLine("Please select a guild number:");
                    foreach (SocketGuild guild2 in guilds)
                    {
                        Bot.WriteLine(i++ + ") " + guild2.Name + " (" + guild2.Id + ")");
                    }
                    Bot.WriteLine(i + ") Close");
                    string str = Terminal.ReadLine("Option number");
                    if (str != i.ToString())
                    {
                        try
                        {
                            int num = int.Parse(str);
                            if (num > 0 && num < guilds.Count + 1)
                            {
                                guild3 = guilds[num - 1];
                                Bot.WriteLine("Selected guild: " + guild3.Name);
                                guild = guild3.Id;
                            }
                            else
                            {
                                Bot.WriteLine("Invalid option, please specify a valid option.");
                            }
                        }
                        catch
                        {
                            Bot.WriteLine("Invalid option, please specify a valid option.");
                        }
                    }
                }
                else if (command == "activeserver")
                {
                    if (guild != 0)
                    {
                        Bot.WriteLine("Active server: " + client.GetGuild(guild).Name + " (" + guild + ")");
                    }
                    else
                    {
                        Bot.WriteLine("Server not set, please run selectserver first.");
                    }
                }
                else
                {
                    bool found = false;

                    foreach (SystemCommand cmd in commands)
                    {
                        if (cmd.allowTerminal)
                        {
                            string fullcmd = command;
                            string cmdid = fullcmd;
                            string arguments = "";
                            if (fullcmd.Contains(" "))
                            {
                                arguments = cmdid.Substring(cmdid.IndexOf(" ") + 1);
                                cmdid = cmdid.Remove(cmdid.IndexOf(" "));
                            }

                            if (cmd.commandid.ToLower() == cmdid.ToLower())
                            {
                                found = true;
                                cmd.OnExecuteFromTerminal(fullcmd, arguments, GetArgumentListFromString(arguments));
                            }
                        }
                    }

                    if (!found)
                    {
                        Bot.WriteLine("Commands:\nquit - stop bot\nexit - stop bot\nsetgame <game> - set the game name (used often as status)\nclrgame - clear the game setting\nsetstatus <status> - set status (online/offline/invisible/do-not-disturb/idle/afk)\nselectserver - choose a guild (this includes servers)\nmsg <channel> <msg> - send a message to a text channel\nactiveserver - print the active guild name and id");
                        foreach (SystemCommand cmd in commands)
                        {
                            if (cmd.allowTerminal)
                            {
                                Bot.WriteLine(cmd.commandid + (cmd.helpsyntax == "" ? "" : " " + cmd.helpsyntax) + " - " + cmd.description);
                            }
                        }
                    }
                }
                Bot.WriteLine();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task Client_LeftGuild(SocketGuild arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            Task.Run(async delegate
            {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                DeleteServer(arg.Id, arg.Name);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void DeleteServer(ulong server, string name)
        {
            if (servers.Find(t => t.id == server) != null)
            {
                Bot.WriteLine("Bot left server " + name + " (" + server + ")");
                Bot.WriteLine("Deleting config of the server " + name + " (" + server + ")");

                ulong id = server;
                servers.Remove(servers.Find(t => t.id == server));
                try
                {
                    Directory.Delete(path + "/Server Configs/" + id, true);
                }
                catch
                {
                    try
                    {
                        Directory.Delete(path + "/Server Configs/" + id, true);
                    }
                    catch
                    {

                    }
                }

                Bot.WriteLine("Deleted server config of the server " + name + " (" + server + ")");
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task Client_JoinedGuild(SocketGuild arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            SyncServer(arg);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task Client_RoleCreated(SocketRole arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            SocketGuild server = arg.Guild;
            SocketRole socketRole = arg;

            SyncRole(server, socketRole);
        }

        public void SyncRole(SocketGuild server, SocketRole socketRole)
        {
            Bot.WriteLine("Synchronizing server " + server.Name + " (" + server.Id + ")");
            Server srv = null;
            if (servers.Find(t => t.id == server.Id) == null) srv = new Server(server.Id, server.Name);
            else srv = servers.Find(t => t.id == server.Id);
            Bot.WriteLine("Synchronizing role file...");
            if (srv.roles.Find(t => t.roleid == socketRole.Id) == null)
            {
                Role r = new Role(socketRole.Name, socketRole.Id);
                srv.roles.Add(r);
                r.permissions = new List<string>(DefaultPermissions);
                srv.SaveRole(r);
            }
        }

        public void SyncServer(SocketGuild server)
        {
            Bot.WriteLine("Synchronizing server " + server.Name + " (" + server.Id + ")");
            Server srv = null;
            if (servers.Find(t => t.id == server.Id) == null) srv = new Server(server.Id, server.Name);
            else srv = servers.Find(t => t.id == server.Id);
            Bot.WriteLine("Synchronizing role files...");
            foreach (SocketRole socketRole in server.Roles)
            {
                if (srv.roles.Find(t => t.roleid == socketRole.Id) == null)
                {
                    Role r = new Role(socketRole.Name, socketRole.Id);
                    srv.roles.Add(r);
                    r.permissions = new List<string>(DefaultPermissions);
                    srv.SaveRole(r);
                }
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task onMessage(SocketMessage msg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            if (msg.Content.StartsWith("+", StringComparison.CurrentCulture))
            {
                bool found = false;
                foreach (SystemCommand cmd in commands)
                {
                    if (cmd.allowDiscord)
                    {
                        string fullcmd = message.Content;
                        string cmdid = fullcmd.Substring(1);
                        string arguments = "";
                        if (fullcmd.Contains(" "))
                        {
                            arguments = cmdid.Substring(cmdid.IndexOf(" ") + 1);
                            cmdid = cmdid.Remove(cmdid.IndexOf(" "));
                        }
                        
                        if (cmd.commandid.ToLower() == cmdid.ToLower())
                        {
                            SocketTextChannel ch = message.Channel as SocketTextChannel;
                            found = true;

                            if (CheckPermissions(cmd.permissionnode, msg.Author, ch.Guild))
                            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                Task.Run(async delegate
                                {
                                    await cmd.OnExecuteFromDiscord(ch.Guild, msg.Author, ch, msg, fullcmd, arguments, GetArgumentListFromString(arguments));
                                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync("```diff\n-I am sorry, but you are not allowed to run that command```");
                            }
                        }
                    }
                }
                if (!found)
                {
                     await message.Channel.SendMessageAsync("I am sorry, but i don't recognize that command, use +help for commands.");
                }
            }
        }

        public void TempMessage(string message, SocketTextChannel channel, int durationsec)
        {
            Task.Run(async delegate
            {
                Task<Discord.Rest.RestUserMessage> t = channel.SendMessageAsync(message);
                await t;
                await Task.Delay(durationsec*1000);
                try
                {
                    await t.GetAwaiter().GetResult().DeleteAsync();
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                }
            });
        }
    }
}
