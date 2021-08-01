using System;
using System.Collections.Generic;

namespace CMDR
{
    public partial class Bot
    {
        public List<string> GetArgumentListFromString(string args)
        {
            List<string> args3 = new List<string>();

            bool ignorespaces = false;
            string last = "";
            int i = 0;
            foreach (char c in args)
            {
                if (c == '"' && (i == 0 || args[i - 1] != '\\'))
                {
                    if (ignorespaces) ignorespaces = false;
                    else ignorespaces = true;
                }
                else if (c == ' ' && !ignorespaces && (i == 0 || args[i - 1] != '\\'))
                {
                    args3.Add(last);
                    last = "";
                }
                else if (c != '\\' || (i + 1 < args.Length && args[i + 1] != '"' && (args[i + 1] != ' ' || ignorespaces)))
                {
                    last += c;
                }

                i++;
            }

            if (last == "" == false) args3.Add(last);

            return args3;
        }
    }
}
