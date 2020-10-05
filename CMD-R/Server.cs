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
        internal bool ___has_changes = false;
        public static void RunSaveAll() { RunSaveAll(false); }
        public static void RunSaveAll(bool force)
        {
            Bot.WriteLine("Saving servers with changes...");
            Bot.WriteLine();
            foreach (Server srv in Bot.GetBot().servers)
            {
                if (srv.___has_changes || force) srv.SaveAll(force);
            }
            Bot.WriteLine();
            Bot.WriteLine("Save completed.");
            _has_changes = false;
        }

        public static void StartAutoSaveThread()
        {
            if (t != null) throw new Exception("AutoSave is already active, you can not start it twice");

            Bot.WriteLine("Starting the autosave system...");

            t = new Thread(() => { 
                while (true)
                {
                    Thread.Sleep(AutoSaveMinutes * 60 * 1000);
                    Bot.WriteLine("Autosave was triggered, checking...");
                    if (_has_changes) new Thread(RunSaveAll).Start();
                    else Bot.WriteLine("No changes.");
                }
            });

            t.Start();

            Bot.WriteLine("Started the autosave system.");
        }

        public static void StopAutoSaveThread()
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
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {

                }

                return lastname;
            }
        }
        public List<Role> roles = new List<Role>();
        public void SaveAll(bool force =false)
        {
            if (Bot.GetBot().servers.Find(t => t.id == id) != null && Bot.GetBot().servers.Find(t => t.id == id) != this) Bot.GetBot().servers[Bot.GetBot().servers.IndexOf(Bot.GetBot().servers.Find(t => t.id == id))] = this;
            
            Bot.WriteLine("Saving server, name: '" + name + "', id: '" + id + "'");
            Directory.CreateDirectory(Bot.GetBot().path + "/Server Configs/" + id);
            File.WriteAllText(Bot.GetBot().path + "/Server Configs/" + id + "/server.info", name);
            if (!UseChangeSave)
            {
                foreach (FileInfo file in new DirectoryInfo(Bot.GetBot().path + "/Server Configs/" + id).GetFiles("*.role"))
                {
                    Role r = Serializer.Deserialize<Role>(File.ReadAllText(file.FullName));
                    if (roles.Find(t => t.roleid == r.roleid) == null)
                    {
                        Bot.WriteLine("  - Deleting role file '" + r.roleid + ".role'...");
                        if (File.Exists(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role")) File.Delete(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role");
                        Bot.WriteLine("  - Deleted role file '" + r.roleid + ".role'");
                    }
                }

                foreach (Role role in roles)
                {
                    if (role.___has_changes || force)
                    {
                        Bot.WriteLine("  - Saving role file '" + role.roleid + ".role'...");
                        File.WriteAllText(Bot.GetBot().path + "/Server Configs/" + id + "/" + role.roleid + ".role", Serializer.Serialize(role));
                        Bot.WriteLine("  - Saved role file '" + role.roleid + ".role'.");
                        role.___has_changes = false;
                    }
                }
            }
            Bot.WriteLine("Saved server, name: '" + name + "', id: '" + id + "'");
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
            if (!File.Exists(Bot.GetBot().path + "/Server Configs/" + id + "/server.info")) SaveAll(true);

            roles.Clear();
            Bot.WriteLine("Loading roles of server " + name + " (" + id + ")");
            foreach (FileInfo file in new DirectoryInfo(Bot.GetBot().path + "/Server Configs/" + id).GetFiles("*.role"))
            {
                Bot.WriteLine();
                Bot.WriteLine("Loading role file '"+file.Name+"'...");
                Role r = Serializer.Deserialize<Role>(File.ReadAllText(file.FullName));
                r.___has_changes = false;
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
            if (UseChangeSave)
            {
                if (roles.Find(t => t.roleid == r.roleid) != null)
                {
                    string serialized1 = File.ReadAllText(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role");
                    string serialized2 = Serializer.Serialize(r);
                    if (serialized1 == serialized2)
                    {
                        Bot.WriteLine("Role file '" + r.roleid + ".role' is exacly the same as the one stored in memory, skipping...");
                        return;
                    }
                }
            }

            if (UseChangeSave) File.WriteAllText(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role", Serializer.Serialize(r));
            else { _has_changes = true; r.___has_changes = true; }

            if (roles.Find(t => t.roleid == r.roleid) == null) roles.Add(r);
            else if (roles.Find(t => t.roleid == r.roleid) != r) roles[roles.IndexOf(roles.Find(t => t.roleid == r.roleid))] = r;

            if (Bot.GetBot().servers.Find(t => t.id == id) != null && Bot.GetBot().servers.Find(t => t.id == id) != this) Bot.GetBot().servers[Bot.GetBot().servers.IndexOf(Bot.GetBot().servers.Find(t => t.id == id))] = this;

            Bot.WriteLine();
            Bot.WriteLine(msg+" role file '" + r.roleid + ".role', role information:");
            Bot.WriteLine("    ID = " + r.roleid);
            Bot.WriteLine("    Name = " + r.rolename);
            Bot.WriteLine("    Permissions = " + r.permissions.Count + " permission node" + (r.permissions.Count == 1 ? "" : "s"));
            Bot.WriteLine("    Blacklisted Permissions = " + r.permissionsblacklist.Count + " permission node" + (r.permissionsblacklist.Count == 1 ? "" : "s"));
            Bot.WriteLine();

            ___has_changes = true;
        }
        public void DeleteRole(Role r)
        {
            if (UseChangeSave) if (File.Exists(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role")) File.Delete(Bot.GetBot().path + "/Server Configs/" + id + "/" + r.roleid + ".role");
            else _has_changes = true;

            if (roles.Find(t => t.roleid == r.roleid) != null) roles.Remove(roles.Find(t => t.roleid == r.roleid));
            Bot.WriteLine();
            if (UseChangeSave) Bot.WriteLine("Deleted role file '" + r.roleid + ".role', role information:");
            else Bot.WriteLine("Scheduled deletion of role file '" + r.roleid + ".role', role information:");
            Bot.WriteLine("    ID = " + r.roleid);
            Bot.WriteLine("    Name = " + r.rolename);
            Bot.WriteLine("    Permissions = " + r.permissions.Count + " permission node" + (r.permissions.Count == 1 ? "" : "s"));
            Bot.WriteLine("    Blacklisted Permissions = " + r.permissionsblacklist.Count + " permission node" + (r.permissionsblacklist.Count == 1 ? "" : "s"));
            Bot.WriteLine();

            ___has_changes = true;
        }
    }

    public class Role
    {
        internal bool ___has_changes = false;
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
