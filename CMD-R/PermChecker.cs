using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace CMDR
{
    public partial class Bot
    {
        public void GetPermsOfUser(SocketUser usr, SocketGuild server, out List<string> PermissionsAllowed, out List<string> PermissionsBlacklist)
        {
            PermissionsAllowed = new List<string>(DefaultPermissions);
            PermissionsBlacklist = new List<string>();

            if (server.OwnerId == usr.Id)
            {
                PermissionsAllowed.Add("*");
            }
            SocketGuildUser user = usr as SocketGuildUser;

            List<SocketRole> roles = new List<SocketRole>(server.Roles).FindAll(t => t.Members.Contains(user));
            foreach (Role role in servers.Find(t => t.id == server.Id).roles.FindAll(t => roles.Find(t2 => t2.Id == t.roleid) != null))
            {
                foreach (string perm in role.permissions)
                {
                    if (!PermissionsAllowed.Contains(perm))
                    {
                        PermissionsAllowed.Add(perm);
                    }
                }
                foreach (string perm in role.permissionsblacklist)
                {
                    if (!PermissionsBlacklist.Contains(perm))
                    {
                        PermissionsBlacklist.Add(perm);
                    }
                }
            }
        }
        public bool CheckPermissions(string perm, SocketUser usr, SocketGuild server)
        {
            List<string> permissions;
            List<string> permissionblacklist;
            GetPermsOfUser(usr, server, out permissions, out permissionblacklist);

            bool output = false;

            foreach (string p in permissions)
            {
                if (p.Contains("*"))
                {
                    if (p == "*")
                    {
                        output = true;
                        break;
                    }
                    else
                    {
                        string data = p.Remove(p.IndexOf("*", StringComparison.CurrentCulture));
                        if (perm.StartsWith(data, StringComparison.CurrentCulture))
                        {
                            output = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (p == perm)
                    {
                        output = true;
                        break;
                    }
                }
            }

            foreach (string p in permissionblacklist)
            {
                if (p.Contains("*"))
                {
                    if (p == "*")
                    {
                        output = false;
                        break;
                    }
                    else
                    {
                        string data = p.Remove(p.IndexOf("*", StringComparison.CurrentCulture));
                        if (perm.StartsWith(data, StringComparison.CurrentCulture))
                        {
                            output = false;
                            break;
                        }
                    }
                }
                else
                {
                    if (p == perm)
                    {
                        output = false;
                        break;
                    }
                }
            }

            return output;
        }

    }
}
