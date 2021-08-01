using System;
using System.Collections.Generic;

namespace ACA.Config.CCFG // TODO: Move to ACA
{
    public class CCFG : Category
    {
        public Dictionary<string,Dictionary<string,string>> maps = new Dictionary<string, Dictionary<string,string>>();

        public CCFG() : base("main")
        {

        }

        public Dictionary<string,string> GetMap(string name)
        {
            if (!maps.ContainsKey(name)) return null;
            return maps[name];
        }

        public Dictionary<string,string> AddOrGetMap(string name)
        {
            if(maps.ContainsKey(name)) return maps[name];
            Dictionary<string,string> map = new System.Collections.Generic.Dictionary<string,string>();
            maps.Add(name, map);

            return map;
        }
        
        public Dictionary<string,string> SetMap(string name, Dictionary<string,string> map)
        {
            if (!maps.ContainsKey(name))maps.Add(name,map);
            else maps[name]=map;
            return GetMap(name);
        }

        public override string ToString()
        {
            string content = "";
            // Use Environment.NewLine for newline

            // TODO

            return content;
        }

        public static CCFG FromString(string content)
        {
            CCFG conf = new CCFG();
            content = content.Replace("\r", "");
            string mname = "Default";
            var map = new Dictionary<string, string>();
            foreach (string line2 in content.Split('\n'))
            {
                string line = line2;
                if (line != "" && !line.StartsWith("#", StringComparison.CurrentCulture) && line.Contains("="))
                {
                    while (line.StartsWith(" ", StringComparison.CurrentCulture))
                    {
                        line = line.Substring(line.IndexOf(" ", StringComparison.CurrentCulture) + 1);
                    }
                    string key = "";
                    string val = "";
                    string last = "";
                    bool escape = false;
                    bool inkey = true;
                    bool escapespace = false;
                    bool iscategorytag = false;
                    bool newval=false;
                    foreach (char c in line)
                    {
                        if (!escape)
                        {
                            if (!escapespace || iscategorytag)
                            {
                                if (c == '>' && inkey && last != "" && !iscategorytag)
                                {
                                    if (!escapespace)
                                    {
                                        while (last.EndsWith(" ", StringComparison.CurrentCulture))
                                        {
                                            last = last.Remove(last.IndexOf(" ", StringComparison.CurrentCulture));
                                        }
                                    }
                                    key = last;
                                    inkey = false;
                                    newval = true;
                                    last = "";
                                    continue;
                                }
                                else if (c == '#' && last != "" && !iscategorytag)
                                {
                                    if (!escapespace)
                                    {
                                        while (last.EndsWith(" ", StringComparison.CurrentCulture))
                                        {
                                            last = last.Remove(last.IndexOf(" ", StringComparison.CurrentCulture));
                                        }
                                    }
                                    if (inkey)
                                    {
                                        key = last;
                                        inkey = false;
                                        newval = true;
                                    }
                                    else
                                    {
                                        val = last;
                                        inkey = true;
                                    }
                                    last = "";
                                    break;
                                }
                                else if (c == '\\')
                                {
                                    escape = true;
                                    continue;
                                }
                                else if(newval && c == ' ' && !iscategorytag){
                                    continue;
                                }
                                else if (c == '(' && inkey && last == "")
                                {
                                    iscategorytag = true;

                                    if (map != null)
                                    {
                                        conf.maps.Add(mname, map);
                                    }

                                    map = null;
                                    mname = "";
                                }
                                else if (c == ')' && iscategorytag)
                                {
                                    iscategorytag = false;

                                    if (last != "")
                                    {
                                        while (last.EndsWith(" ", StringComparison.CurrentCulture))
                                        {
                                            last = last.Remove(last.IndexOf(" ", StringComparison.CurrentCulture));
                                        }
                                        while (last.StartsWith(" ", StringComparison.CurrentCulture))
                                        {
                                            last = last.Substring(last.IndexOf(" ", StringComparison.CurrentCulture) + 1);
                                        }
                                        map = new Dictionary<string, string>();
                                        mname = last;
                                        last = "";
                                    }
                                }
                            }
                            if (c == '"' && !iscategorytag)
                            {
                                escapespace = !escapespace;
                            }
                        }

                        escape = false;
                        newval=false;
                        last += c;
                    }

                    if (last != "" && !iscategorytag)
                    {
                        if (!escapespace)
                        {
                            while (last.EndsWith(" ", StringComparison.CurrentCulture))
                            {
                                last = last.Remove(last.IndexOf(" ", StringComparison.CurrentCulture));
                            }
                        }
                        if (inkey)
                        {
                            key = last;
                            inkey = false;
                        }
                        else
                        {
                            val = last;
                        }
                        last = "";
                    }

                    if (!inkey && !newval && !iscategorytag)
                    {
                        inkey = true;
                        map.Add(key, val);
                    }
                }
            }

            if (map != null && mname != "") {
                conf.maps.Add(mname, map);
            }

            return conf;
        }
    }
}
