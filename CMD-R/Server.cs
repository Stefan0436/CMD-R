using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Discord.WebSocket;

namespace CMDR
{
    public class Server
    {
        static Thread t = null;
        public static bool IsAutoSaveActive() { return t != null; }

        public static void RunSaveAll()
        { 
            foreach (Server srv in Bot.GetBot().servers)
            {
                srv.SaveAll();
            }
        }

        public static void StartAutoSaveHandler()
        {
            if (t != null) throw new Exception("AutoSave is already active, you can not run it twice");

            Bot.WriteLine("Starting the autosave system...");

            t = new Thread(() => { 
                while (true)
                {
                    if (_has_changes) new Thread(RunSaveAll).Start();
                    Thread.Sleep(AutoSaveMinutes * 60 * 1000);
                }
            });

            t.Start();

            Bot.WriteLine("Started the AutoSave system.");
        }

        public static void StopAutoSaveHandler()
        {
            if (t == null) throw new Exception("AutoSave is not active, you can not stop it if it is not running");
            Bot.WriteLine("Stopping the autosave system...");
            t.Abort();
            t = null;
            Bot.WriteLine("Stopped the autosave system.");
        }

        public static bool UseChangeSave = true;
        public static int AutoSaveMinutes = 60;
        public static bool _has_changes = false;

        public static void LoadAllServers(out List<Server> servers)
        {
            servers = new List<Server>();
            Bot.WriteLine("Loading all servers...");
            foreach (DirectoryInfo server in new DirectoryInfo(Bot.GetBot().path + "/Server Configs").GetDirectories())
            {
                bool deleted = true;
                foreach (SocketGuild guild in Bot.GetBot().client.Guilds)
                {
                    if (guild.Id.ToString() == server.Name)
                    {
                        deleted = false;
                        break; 
                    }
                }
                if (deleted)
                {
                    Bot.GetBot().DeleteServer(ulong.Parse(server.Name), File.ReadAllText(server.FullName + "/server.info"));
                }
                else
                {
                    Bot.WriteLine("Loading server folder " + server.Name + "...");
                    Server s = new Server(ulong.Parse(server.Name));
                    servers.Add(s);
                    Bot.WriteLine("Folder " + server.Name + " loaded, server name: " + s.name);
                }
            }
            Bot.WriteLine("Loaded " + servers.Count + " server" + (servers.Count == 0 ? "" : "s"));
        }
        public ulong id;
        string lastname = "";
        public string name
        {
            get
            {
                try
                {
                    lastname = Bot.GetBot().client.GetGuild(id).Name;
                }
                catch
                {
                }

                return lastname;
            }
        }
        public List<Role> roles = new List<Role>();
        public void SaveAll()
        {
            Directory.CreateDirectory(Bot.GetBot().path + "/Server Configs/" + id);
            File.WriteAllText(Bot.GetBot().path + "/Server Configs/" + id + "/server.info", name);
            foreach (Role role in roles)
            {
                SaveRole(role);
            }
        }
        public Server(ulong id)
        {
            this.id = id;
            LoadAll();
        }

        public Server(ulong id, string name)
        {
            this.id = id;
            lastname = name;
            LoadAll();
        }
        public void LoadAll()
        {
            Directory.CreateDirectory(Bot.GetBot().path + "/Server Configs/" + id);
            if (!File.Exists(Bot.GetBot().path + "/Server Configs/" + id + "/server.info")) SaveAll();

            roles.Clear();
            Bot.WriteLine("Loading roles of server " + name + " (" + id + ")");
            foreach (FileInfo file in new DirectoryInfo(Bot.GetBot().path + "/Server Configs/" + id).GetFiles("*.role"))
            {
                Bot.WriteLine();
                Bot.WriteLine("Loading role file "+file.Name+"...");
                Role r = Serializer.Deserialize<Role>(File.ReadAllText(file.FullName));
                Bot.WriteLine("Role loaded, role information:");
                Bot.WriteLine("    ID = "+r.roleid);
                Bot.WriteLine("    Name = " + r.rolename);
                Bot.WriteLine("    Permissions = " + r.permissions.Count + " permission node" + (r.permissions.Count == 1 ? "" : "s"));
                Bot.WriteLine("    Blacklisted Permissions = " + r.permissionsblacklist.Count + " permission node" + (r.permissionsblacklist.Count == 1 ? "" : "s"));
                Bot.WriteLine();
                roles.Add(r);
            }
            Bot.WriteLine("Loaded "+roles.Count+" role"+(roles.Count==1?"":"s"));
        }

        public void SaveRole(Role r, string msg = "Saved")
        {
            File.WriteAllText(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role", Serializer.Serialize(r));
            Bot.WriteLine();
            Bot.WriteLine(msg+" role file '" + r.rolename + "', role information:");
            Bot.WriteLine("    ID = " + r.roleid);
            Bot.WriteLine("    Name = " + r.rolename);
            Bot.WriteLine("    Permissions = " + r.permissions.Count + " permission node" + (r.permissions.Count == 1 ? "" : "s"));
            Bot.WriteLine("    Blacklisted Permissions = " + r.permissionsblacklist.Count + " permission node" + (r.permissionsblacklist.Count == 1 ? "" : "s"));
            Bot.WriteLine();
        }
        public void DeleteRole(Role r)
        {
            if (roles.Find(t => t.roleid == r.roleid) != null) roles.Remove(roles.Find(t => t.roleid == r.roleid));
            if (File.Exists(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role")) File.Delete(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role");
            Bot.WriteLine();
            Bot.WriteLine("Deleted role file '" + r.rolename + "', role information:");
            Bot.WriteLine("    ID = " + r.roleid);
            Bot.WriteLine("    Name = " + r.rolename);
            Bot.WriteLine("    Permissions = " + r.permissions.Count + " permission node" + (r.permissions.Count == 1 ? "" : "s"));
            Bot.WriteLine("    Blacklisted Permissions = " + r.permissionsblacklist.Count + " permission node" + (r.permissionsblacklist.Count == 1 ? "" : "s"));
            Bot.WriteLine();
        }
    }

    public class Role
    {
        Role()
        {

        }
        public Role(string name, ulong id)
        {
            roleid = id;
            rolename = name;
        }

        public ulong roleid;
        public string rolename = "";
        public List<string> permissions = new List<string>();
        public List<string> permissionsblacklist = new List<string>();
    }
}
