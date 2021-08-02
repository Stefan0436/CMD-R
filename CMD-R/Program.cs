using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO.Compression;

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
        public string prefix = "+";
        public string token = "";
        public string path;
        public ulong guild = 0;
        public DiscordSocketClient client;
        static bool debugenabled = false;
        static bool debugbreach = false;
        public static bool GetDebug() => debugenabled;
        public static bool GetDebugEnabledBefore() => debugbreach;
        public static void Main(string[] args)
        {
            ConsoleColor b = Console.ForegroundColor;
            Console.ForegroundColor = b;
            foreach (string argument in args)
            {
                if (argument == "--enable-debug" && !debugenabled)
                {
                    debugenabled = true;
                    debugbreach = false;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Bot.WriteLine("--{=-- POSSIBLE SECURITY RISK --=}-- ==>>>  Debug Enabled, ASMLD Unlocked.");
                    Console.ForegroundColor = b;
                }
                else if (argument == "--disable-debug" && debugenabled)
                {
                    debugenabled = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("--{=-- DEBUG SYSTEM DISABLED --=}-- ==>>>  Debug Disabled, ASMLD Locked.");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" [ WARNING: POSSIBLE SECURITY RISK ]");
                    Console.ForegroundColor = b;
                }
                else if (argument.StartsWith("ASMLD:{", StringComparison.CurrentCulture) && debugenabled)
                {
                    string path = argument.Substring("ASMLD:{".Length);
                    path = path.Remove(path.IndexOf("}", StringComparison.CurrentCulture));
                    if (!File.Exists(path) && File.Exists(path + "/" + Path.GetFileName(path) + ".dll"))
                    {
                        path = path + Path.DirectorySeparatorChar + Path.GetFileName(path);
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Bot.WriteLine("--{=-- AMSLD --=}-- ==>>>  Inject Assembly: " + Path.GetFileName(path));
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

                    Assembly LoadFromSameFolder(object sender, ResolveEventArgs args2)
                    {
                        string assemblyPath = Path.Combine(Path.GetDirectoryName(path), new AssemblyName(args2.Name).Name + ".dll");
                        if (!File.Exists(assemblyPath)) return null;
                        b = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Bot.WriteLine("--{=-- AMSLD --=}-- ==>>>  Load DLL Reference: " + assemblyPath);
                        Assembly assembly = Assembly.LoadFrom(assemblyPath);
                        Bot.WriteLine("--{=-- AMSLD --=}-- ==>>>  Loaded Assembly: " + assembly.GetName().Name + " (" + assembly.GetTypes().Count() + " Type(s) Loaded)");
                        Console.ForegroundColor = b;
                        return assembly;
                    }

                    Assembly asm = Assembly.LoadFrom(path);
                    Bot.WriteLine("--{=-- AMSLD --=}-- ==>>>  Loaded Assembly: " + asm.GetName().Name + " (" + asm.GetTypes().Count() + " Type(s) Loaded)");
                    Console.ForegroundColor = b;
                }
            }
            new Bot().MainAsync().GetAwaiter().GetResult();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task MainAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ConsoleColor b = Console.ForegroundColor;
            Console.ForegroundColor = b;

            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Bot.WriteLine("Booting CMD-R...");
            Bot.WriteLine("AerialWorks CMD-R is Free(Libre) and Open Source Software (FLOSS),\nand will always be licensed under the GPL-2.0 license.");
            Bot.WriteLine("------------------------------------------------------------------");
            Bot.WriteLine();
            Bot.WriteLine("System path: " + path);
            Directory.CreateDirectory(path + "/Modules");
            Directory.CreateDirectory(path + "/Module Packages");
            Directory.CreateDirectory(path + "/Embedded Modules");

            Bot.WriteLine();
            Bot.WriteLine("Loading token...");
            bool newToken = false;
            if (File.Exists(path + "/Bot.cfg"))
            {
                token = File.ReadAllText(path + "/Bot.cfg").Replace("\n", "");
            }
            else
            {
                token = Terminal.ReadLine("Bot token").Replace("\n", "");
                newToken = true;
            }

            Console.CancelKeyPress += new ConsoleCancelEventHandler(delegate (Object sender, ConsoleCancelEventArgs e) {
                if (e.SpecialKey == ConsoleSpecialKey.ControlBreak) {
                    Bot.WriteLine("Shutting down CMD-R, CTRL+BREAK detected...");
                    client.SetGameAsync("Shutting down...").GetAwaiter().GetResult();
                    client.SetStatusAsync(UserStatus.Invisible).GetAwaiter().GetResult();
                    client.StopAsync().GetAwaiter().GetResult();
                    client.Dispose();
                    Server.RunSaveAll(true);
                    Server.StopAutoSaveThread();
                    Environment.Exit(0);
                }
                e.Cancel = true;
            });
            Bot.WriteLine("Starting Discord.NET framework...");
            client = new DiscordSocketClient();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("Starting CMD-R...");

            if (newToken) {
                Bot.WriteLine("\nSaved to file Bot.cfg, edit it to change the token.");
                File.WriteAllText(path + "/Bot.cfg", token);
            }

            while (client.ConnectionState == ConnectionState.Connecting || client.ConnectionState == ConnectionState.Disconnected) { Thread.Sleep(100); }
            while (client.Guilds.Count == 0 || client.Guilds.FirstOrDefault().Name == null || client.Guilds.FirstOrDefault().Name == "") { Thread.Sleep(100); }

            Bot.WriteLine("Loading modules...");
            Bot.WriteLine("Loading embedded/injected modules...");
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type module in asm.GetTypes())
                {
                    if (module.BaseType == typeof(BotModule))
                    {
                        Bot.WriteLine("Loading module class: " + module.Name);
                        var script = Activator.CreateInstance(module) as BotModule;
                        Bot.WriteLine("Loading module: " + script.id + "...");
                        Bot.WriteLine("Module description: " + script.moduledesctiption);
                        modules.Add(script);
                        Directory.CreateDirectory(path + "/Embedded Modules/" + script.id);
                        script.modulepath = "ASM:{" + module.FullName + "}";
                        script.storagepath = path + "/Embedded Modules/" + script.id;
                        if (!File.Exists(Bot.GetBot().path + "/Embedded Modules/" + script.id + "/Storage/config.xml") && File.Exists(Path.GetDirectoryName(module.Assembly.Location) + "/config-defaults.xml") && module.Assembly.FullName != Assembly.GetCallingAssembly().FullName)
                        {
                            File.Copy(Bot.GetBot().path + "/Modules/" + script.id + "/config-defaults.xml", Bot.GetBot().path + "/Embedded Modules/" + script.id + "/Storage/config.xml");
                        }
                        script.LoadConfig();
                        Bot.WriteLine("Pre-initializing module: " + script.id + "...");
                        script.PreInit(this);
                        script.SaveConfig();
                        Bot.WriteLine("Loaded module: " + script.id + "...");
                    }
                }
            }

            Bot.WriteLine();
            Bot.WriteLine("Loading modules from the Modules directory...");
            foreach (DirectoryInfo info in new DirectoryInfo(path + "/Modules").GetDirectories())
            {
                if (File.Exists(info.FullName + "/" + info.Name + ".dll"))
                {
                    LoadBotModule(info.Name, info.FullName);
                }
                else
                {
                    b = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Bot.WriteLine(" -- WARNING -- ==>>> Bot module folder '" + info.Name + "' has no valid module file, no files match the folder name.");
                    Console.ForegroundColor = b;
                }
            }
            Bot.WriteLine("Loading modules from the packages...");
            foreach (FileInfo info in new DirectoryInfo(path + "/Module Packages").GetFiles("*.cpkg"))
            {
                loadPackage(info);
            }
            
            // Load CMF (Cyan Modfile) formatted packages
            // CMF are zips too, just like jars
            //
            // By allowing CMFs to be loaded in  CMD-R, one can create a one-fits-all package
            // When you create a Connective module, you can add a pointer file, dll, and mod.manifest.ccfg document
            // to make it compatible with CMD-R, Connective and Cyan. (if you combine the packages)
            foreach (FileInfo info in new DirectoryInfo(path + "/Module Packages").GetFiles("*.cmf"))
            {
                loadPackage(info);
            }
            
            void loadPackage(FileInfo info) {
                bool update = false;
                
                if (!Directory.Exists(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName))) update = true; // Make sure the package gets extracted if new
                else
                {
                    // Check if package archive is newer than extracted version
                    string oldpatch = (File.Exists(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/patch.ver") ? File.ReadAllText(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/patch.ver") : "");
                    // If the patch.ver file does not exist, the string is empty
                    
                    string newpatch = "";
                    
                    // Read the archive for getting the new version file, skip if patch.ver is not in the archive
                    ZipArchive file = ZipFile.OpenRead(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName));
                    if (file.Entries.ToList().Find(t => t.Name == "patch.ver") != null)
                    {
                        try
                        {
                            StreamReader r = new StreamReader(file.Entries.ToList().Find(t => t.Name == "patch.ver").Open());
                            newpatch = r.ReadLine();
                            r.Close();
                        }
                        catch
                        {
                            
                        }
                    }
                    
                    int newv = 0;
                    int oldv = 0;
                    
                    // Parse if the strings are not empty
                    if (newpatch != "") newv = int.Parse(newpatch);
                    if (oldpatch != "") oldv = int.Parse(oldpatch);
                    
                    update = (oldv < newv); // Check if the archive is newer than the folder
                }
                
                // Extract files if new/updated
                if (update)
                {
                    if (Directory.Exists(path + "/temp"))
                    {
                        Bot.WriteLine("Removing existing temp folder...");
                        Directory.Delete(path + "/temp", true);
                    }
                    Bot.WriteLine("Install module package: " + info.FullName);
                    Bot.WriteLine("Extracting package to temp...");
                    ZipFile.ExtractToDirectory(info.FullName, path + "/temp");
                    Bot.WriteLine("Preparing update...");
                    if (Directory.Exists(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/Storage"))
                    {
                        Bot.WriteLine("Moving existing storage folder...");
                        Directory.Move(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/Storage", path + "/temp/Storage");
                    }
                    try
                    {
                        Bot.WriteLine("Installing package...");
                        Directory.Delete(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName), true);
                        Directory.Move(path + "/temp", path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName));
                        Bot.WriteLine("Package file " + info.Name + " has been installed.");
                    }
                    catch (Exception e)
                    {
                        Directory.CreateDirectory(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName));
                        Bot.WriteLine("Installation failed, moving module storage back to install folder and exiting...");
                        if (Directory.Exists(Path.GetFileNameWithoutExtension(info.FullName) + "/Storage"))
                        {
                            b = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Bot.WriteLine("CRITICAL ERROR: Unable to restore or save storage folder, one already exists in both temp and install output!");
                            Bot.WriteLine("DO NOT RESTART CMD-R BEFORE FIXING THIS ERROR OR FILES WILL GET LOST!");
                            Console.ForegroundColor = b;
                            throw new FieldAccessException("Unable to move storage back to origin, already exists, consider CMD-R crashed.");
                        }
                        if (Directory.Exists(path + "/temp/Storage"))
                        {
                            Directory.Move(path + "/temp/Storage", path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/Storage");
                        }
                        
                        #pragma warning disable CA2200
                        throw e;
                        #pragma warning restore CA2200
                    }
                }
                
                ConfigDictionary<String, String> conf = new ConfigDictionary<string, string>();
                if (File.Exists(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/pointer.targets"))
                {
                    conf = Serializer.Deserialize<ConfigDictionary<String, String>>(File.ReadAllText(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/pointer.targets"));
                }
                if (!File.Exists(path + "/Module Packages/" + Path.GetFileNameWithoutExtension(info.FullName) + "/pointer.targets") ||
                    conf.Count == 0 || !conf.ContainsKey("FileName") || !conf.ContainsKey("ClassName") || !conf.ContainsKey("Namespace"))
                {
                    b = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Bot.WriteLine(" -- WARNING -- ==>>> Extracted bot module package '" + info.Name + "' does not contain a valid pointer file, unable to load it.");
                    Console.ForegroundColor = b;
                }
                LoadBotModule(conf.GetValue("FileName"),
                              path + Path.DirectorySeparatorChar + "Module Packages" + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(info.FullName),
                              conf.GetValue("ClassName"),
                              conf.GetValue("Namespace")
                );
            }

            Bot.WriteLine();
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
            Bot.WriteLine("Loaded " + DefaultPermissions.Count + " permission node" + (DefaultPermissions.Count == 1 ? "" : "s"));

            Server.LoadAllServers(out servers);

            client.RoleCreated += Client_RoleCreated;
            client.JoinedGuild += Client_JoinedGuild;
            client.RoleUpdated += Client_RoleUpdated;
            client.RoleDeleted += Client_RoleDeleted;
            client.LeftGuild += Client_LeftGuild;

            Bot.WriteLine("Synchronizing servers attatched to the bot...");
            foreach (SocketGuild server in client.Guilds)
            {
                SyncServer(server);
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


            Bot.WriteLine("Switching to auto-save instead of change-save...");
            Server.UseChangeSave = false;
            Bot.WriteLine();
            Bot.WriteLine("Loading autosave config: " + path + Path.DirectorySeparatorChar + "AutoSaveMinutes.save...");
            if (File.Exists(path + "/AutoSaveMinutes.save")) Server.AutoSaveMinutes = int.Parse(File.ReadAllLines(path + "/AutoSaveMinutes.save")[0]);
            else File.WriteAllText(path + "/AutoSaveMinutes.save", Server.AutoSaveMinutes.ToString());
            Bot.WriteLine("Set the autosave interval to " + Server.AutoSaveMinutes + " minutes.");
            Server.StartAutoSaveThread();

            Bot.WriteLine();
            Bot.WriteLine("Execute quit or exit to close.");

            client.MessageReceived += onMessage;
            await client.SetGameAsync("");
            while (true)
            {
                string command = Terminal.ReadLine().Replace("\0", "");
                if (command == "quit" || command == "exit")
                {
                    await client.SetGameAsync("Shutting down...");

                    await client.SetStatusAsync(UserStatus.Invisible);
                    await client.StopAsync();
                    client.Dispose();
                    Server.RunSaveAll(true);
                    Server.StopAutoSaveThread();
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
                else if (command.StartsWith("setautosaveinterval ", StringComparison.CurrentCulture))
                {
                    string mins = command.Substring("setautosaveinterval ".Length);
                    try
                    {
                        Server.AutoSaveMinutes = int.Parse(mins);
                        File.WriteAllText(path + "/AutoSaveMinutes.save", Server.AutoSaveMinutes.ToString());
                        Bot.WriteLine("Saved the autosave interval.");
                        Bot.WriteLine();
                        if (Server.IsAutoSaveActive()) Server.StopAutoSaveThread();
                        Server.StartAutoSaveThread();
                        Bot.WriteLine();
                        Bot.WriteLine("Reloaded autosave, autosave interval: " + Server.AutoSaveMinutes + " minutes.");
                    }
                    catch
                    {
                        Bot.WriteLine("Invalid input");
                    }
                }
                else if (command == "enableautosave")
                {
                    if (!Server.IsAutoSaveActive())
                    {
                        Server.StartAutoSaveThread();
                        Bot.WriteLine();
                        Bot.WriteLine("Enabled autosave, disabled changesave.");
                        Bot.WriteLine("Autosave interval: " + Server.AutoSaveMinutes + " minutes.");
                    }

                }
                else if (command == "disableautosave")
                {
                    if (Server.IsAutoSaveActive())
                    {
                        Server.StopAutoSaveThread();
                        Bot.WriteLine();
                        Bot.WriteLine("Disabled autosave, enabled changesave.");
                    }

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
                    if (guild != 0 && client.GetGuild(guild) == null)
                        guild = 0;
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
                                Bot.WriteLine("[WARNING, MSG CMD] Multiple channels with the same name¸ sending message to " + client.GetGuild(guild).TextChannels.Count(t => t.Name == channel) + " channels, recommend troubleshooting.");
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
                    Bot.WriteLine("Please select a server:");
                    foreach (SocketGuild guild2 in guilds)
                    {
                        Bot.WriteLine(i++ + ") " + guild2.Name + " (" + guild2.Id + ")");
                    }
                    Bot.WriteLine(i + ") Cancel");
                    string str = Terminal.ReadLine("Option number");
                    if (str != i.ToString())
                    {
                        try
                        {
                            int num = int.Parse(str);
                            if (num > 0 && num < guilds.Count + 1)
                            {
                                guild3 = guilds[num - 1];
                                Bot.WriteLine("Selected server: " + guild3.Name);
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
                    if (guild != 0 && client.GetGuild(guild) == null)
                        guild = 0;
                    if (guild != 0)
                    {
                        Bot.WriteLine("Active server: " + client.GetGuild(guild).Name + " (" + guild + ")");
                    }
                    else
                    {
                        Bot.WriteLine("Server not set, please run selectserver first.");
                    }
                }
                else if (command == "saveall")
                {
                    Server.RunSaveAll(true);
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
                                arguments = cmdid.Substring(cmdid.IndexOf(" ", StringComparison.CurrentCulture) + 1);
                                cmdid = cmdid.Remove(cmdid.IndexOf(" ", StringComparison.CurrentCulture));
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
                        Bot.WriteLine("Commands:\nquit - stop bot\nexit - stop bot\nsetgame <game> - set the game name (used often as status)\nclrgame - clear the game setting\nsetstatus <status> - set status (online/offline/invisible/do-not-disturb/idle/afk)\nselectserver - choose a guild (this includes servers)\nmsg <channel> <msg> - send a message to a text channel\nactiveserver - print the active guild name and id\nsaveall - save all servers\nsetautosaveinterval <minutes> - set the autosave interval\nenableautosave - enable autosave\ndisableautosave - disable autosave");
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

        public static void LoadBotModule(string filename, string filefolder, string classname = "{auto}", string namespacepath = "{auto}")
        {
            if (classname == "{auto}") classname = filename;
            if (namespacepath == "{auto}") namespacepath = filename;
            Bot.WriteLine("Loading module file: " + filefolder + Path.DirectorySeparatorChar + filename + ".dll");
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

            Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
            {
                string assemblyPath = Path.Combine(filefolder, new AssemblyName(args.Name).Name + ".dll");
                if (!File.Exists(assemblyPath)) return null;
                Bot.WriteLine("Loading DLL Assembly: " + assemblyPath);
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                return assembly;
            }
            Assembly asm = Assembly.LoadFrom(filefolder + Path.DirectorySeparatorChar + filename + ".dll");
            Type type = asm.GetType(namespacepath + "." + classname);

            var script = Activator.CreateInstance(type) as BotModule;

            if (script == null)
            {
                Bot.WriteLine("Module file: '" + filefolder + Path.DirectorySeparatorChar + filename + ".dll' does not have a class named " + classname + " in a namespace named " + namespacepath + ", cannot load it, skipping...");
            }
            else
            {

                Bot.WriteLine("Loading module: " + script.id + "...");
                Bot.WriteLine("Module description: " + script.moduledesctiption);
                Bot.GetBot().modules.Add(script);
                Directory.CreateDirectory(Bot.GetBot().path + "/Modules/" + script.id + "/Storage");
                if (!File.Exists(Bot.GetBot().path + "/Modules/" + script.id + "/Storage/config.xml") && File.Exists(Bot.GetBot().path + "/Modules/" + script.id + "/config-defaults.xml"))
                {
                    File.Copy(Bot.GetBot().path + "/Modules/" + script.id + "/config-defaults.xml", Bot.GetBot().path + "/Modules/" + script.id + "/Storage/config.xml");
                }
                script.modulepath = Path.GetFullPath(filefolder + Path.DirectorySeparatorChar + filename + ".dll");
                script.storagepath = Path.GetFullPath(Bot.GetBot().path + Path.DirectorySeparatorChar + "Modules" + Path.DirectorySeparatorChar + script.id + Path.DirectorySeparatorChar + "Storage");
                script.LoadConfig();
                Bot.WriteLine("Pre-initializing module: " + script.id + "...");
                script.PreInit(Bot.GetBot());
                script.SaveConfig();
                Bot.WriteLine("Loaded module: " + script.id + "...");
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task Client_RoleDeleted(SocketRole arg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            DeleteRole(arg.Guild, arg);
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task Client_RoleUpdated(SocketRole arg1, SocketRole arg2)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            SyncRole(arg2.Guild, arg2);
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
            if (Directory.Exists(path + "/Server Configs/" + server))
            {
                Bot.WriteLine("Bot left server " + name + " (" + server + ")");
                Bot.WriteLine("Deleting config of the server " + name + " (" + server + ")");

                ulong id = server;
                if (servers.Find(t => t.id == server) != null) servers.Remove(servers.Find(t => t.id == server));
                if (Directory.Exists(path + "/Server Configs/" + id)) Directory.Delete(path + "/Server Configs/" + id, true);

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

        public Server GetServerFromSocketGuild(SocketGuild guild)
        {
            return servers.Find(t => t.id == guild.Id);
        }

        public void SyncRole(SocketGuild server, SocketRole socketRole)
        {
            Server srv = null;
            if (servers.Find(t => t.id == server.Id) == null)
            {
                srv = new Server(server.Id, server.Name);
                servers.Add(srv);

                if (Server.UseChangeSave) srv.SaveAll();
                else { srv.___has_changes = true; Server._has_changes = true; }
            }
            else srv = servers.Find(t => t.id == server.Id);
            Bot.WriteLine("Synchronizing role file...");
            if (srv.roles.Find(t => t.roleid == socketRole.Id) == null)
            {
                Role r = new Role(socketRole.Name, socketRole.Id);
                srv.roles.Add(r);
                r.permissions = new List<string>(DefaultPermissions);

                if (Server.UseChangeSave) srv.SaveRole(r);
                else srv.SaveRole(r, "Scheduled save of");
            }
            else
            {
                Role r = srv.roles.Find(t => t.roleid == socketRole.Id);
                r.rolename = socketRole.Name;
                if (Server.UseChangeSave) srv.SaveRole(r, "Updated");
                else srv.SaveRole(r, "Scheduled update of");
            }
        }

        public void DeleteRole(SocketGuild server, SocketRole socketRole)
        {
            Server srv = null;
            if (servers.Find(t => t.id == server.Id) == null)
            {
                srv = new Server(server.Id, server.Name);
                servers.Add(srv);

                if (Server.UseChangeSave) srv.SaveAll();
                else { srv.___has_changes = true; Server._has_changes = true; }
            }
            else srv = servers.Find(t => t.id == server.Id);
            Bot.WriteLine("Deleting role file...");
            if (srv.roles.Find(t => t.roleid == socketRole.Id) != null)
            {
                srv.DeleteRole(srv.roles.Find(t => t.roleid == socketRole.Id));
            }
        }

        public void SyncServer(SocketGuild server)
        {
            Bot.WriteLine("Synchronizing server " + server.Name + " (" + server.Id + ")");
            Server srv = null;
            if (servers.Find(t => t.id == server.Id) == null)
            {
                srv = new Server(server.Id, server.Name);
                servers.Add(srv);

                if (Server.UseChangeSave) srv.SaveAll();
                else { srv.___has_changes = true; Server._has_changes = true; }
            }

            else srv = servers.Find(t => t.id == server.Id);
            Bot.WriteLine("Synchronizing role files...");
            foreach (SocketRole socketRole in server.Roles)
            {
                if (srv.roles.Find(t => t.roleid == socketRole.Id) == null)
                {
                    Role r = new Role(socketRole.Name, socketRole.Id);
                    srv.roles.Add(r);
                    r.permissions = new List<string>(DefaultPermissions);

                    if (Server.UseChangeSave) srv.SaveRole(r);
                    else srv.SaveRole(r, "Scheduled save of");
                }
                else
                {
                    Role r = srv.roles.Find(t => t.roleid == socketRole.Id);
                    r.rolename = socketRole.Name;
                    if (Server.UseChangeSave) srv.SaveRole(r, "Updated");
                    else srv.SaveRole(r, "Scheduled update of");
                }
            }
            foreach (FileInfo file in new DirectoryInfo(Bot.GetBot().path + "/Server Configs/" + srv.id).GetFiles("*.role"))
            {
                Role r = Serializer.Deserialize<Role>(File.ReadAllText(file.FullName));
                bool deleted = true;
                foreach (SocketRole socketRole in server.Roles)
                {
                    if (r.roleid == socketRole.Id)
                    {
                        deleted = false;
                        break;
                    }
                }
                if (deleted)
                {
                    srv.DeleteRole(r);
                }
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task onMessage(SocketMessage msg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;
            SocketTextChannel ch = message.Channel as SocketTextChannel;
            if (msg.Content.StartsWith(prefix, StringComparison.CurrentCulture) && ch != null && !msg.Content.StartsWith(prefix + " ", StringComparison.CurrentCulture) && msg.Content != prefix)
            {
                bool found = false;
                foreach (SystemCommand cmd in commands)
                {
                    if (cmd.allowDiscord)
                    {
                        string fullcmd = message.Content;
                        string cmdid = fullcmd.Substring(prefix.Length);
                        string arguments = "";
                        if (fullcmd.Contains(" "))
                        {
                            arguments = cmdid.Substring(cmdid.IndexOf(" ", StringComparison.CurrentCulture) + 1);
                            cmdid = cmdid.Remove(cmdid.IndexOf(" ", StringComparison.CurrentCulture));
                        }

                        if (cmd.commandid.ToLower() == cmdid.ToLower())
                        {
                            found = true;

                            if (CheckPermissions(cmd.permissionnode, msg.Author, ch.Guild))
                            {
                                await cmd.OnExecuteFromDiscord(ch.Guild, msg.Author, ch, msg, fullcmd, arguments, GetArgumentListFromString(arguments));
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync("```diff\n- I am sorry, but you are not allowed to run that command```");
                            }
                        }
                    }
                }
                if (!found)
                {
                    await message.Channel.SendMessageAsync("I am sorry, but i don't recognize that command, use " + prefix + "help for commands.");
                }
            }
            else if (message.Channel is SocketDMChannel && message.Author.Id != client.CurrentUser.Id) await msg.Channel.SendMessageAsync("```diff\n- I am sorry, but CMD-R do not support direct messages yet.\n- Please go to a server and run " + prefix + "help for a list of commands```");
            else {
                foreach (SystemCommand cmd in commands)
                {
                    if (cmd.allowDiscord && cmd.setNoCmdPrefix)
                    {
                        string fullcmd = message.Content;
                        string cmdid = fullcmd;
                        string arguments = "";
                        if (fullcmd.Contains(" "))
                        {
                            arguments = cmdid.Substring(cmdid.IndexOf(" ", StringComparison.CurrentCulture) + 1);
                            cmdid = cmdid.Remove(cmdid.IndexOf(" ", StringComparison.CurrentCulture));
                        }

                        if (cmd.commandid.ToLower() == cmdid.ToLower())
                        {
                            if (CheckPermissions(cmd.permissionnode, msg.Author, ch.Guild))
                            {
                                await cmd.OnExecuteFromDiscord(ch.Guild, msg.Author, ch, msg, fullcmd, arguments, GetArgumentListFromString(arguments));
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync("```diff\n- I am sorry, but you are not allowed to run that command```");
                            }
                        }
                    }
                }
            }
        }

        public void TempMessage(string message, SocketTextChannel channel, int durationsec)
        {
            Task.Run(async delegate
            {
                Task<Discord.Rest.RestUserMessage> t = channel.SendMessageAsync(message);
                await t;
                await Task.Delay(durationsec * 1000);
                // TODO: Test if the message needs to exist to call delete, if it does, find a way to do it right and not through try/catch!
                await t.GetAwaiter().GetResult().DeleteAsync();
            });
        }
    }
}
