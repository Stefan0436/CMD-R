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
                if (c is '"')
                {
                    bool skip = false;
                    try
                    {
                        if (args[i - 1] is '\\')
                        {
                            skip = true;
                        }
                    }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                    catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                    {

                    }
                    if (skip == false)
                    {
                        if (ignorespaces) ignorespaces = false;
                        else ignorespaces = true;
                    }
                    else
                    {
                        last += c;
                    }
                }
                else
                {
                    if (c is ' ')
                    {
                        bool skip = false;
                        try
                        {
                            if (args[i - 1] is '\\')
                            {
                                skip = true;
                            }
                        }
                        catch
                        {

                        }
                        if (skip == false)
                        {
                            if (ignorespaces)
                            {
                                last += c;
                            }
                            else
                            {
                                args3.Add(last);
                                last = "";
                            }
                        }
                        else
                        {
                            last += c;
                        }
                    }
                    else if (c is '\\' == false)
                    {
                        last += c;
                    }
                    else
                    {
                        bool skip = false;
                        try
                        {
                            if (args[i + 1] is '"')
                            {
                                skip = true;
                            }
                            else if (args[i + 1] is ' ')
                            {
                                if (ignorespaces == false)
                                {
                                    skip = true;
                                }
                            }
                        }
                        catch
                        {

                        }
                        if (skip == false)
                        {
                            last += c;
                        }
                    }
                }

                i++;
            }

            if (last == "" == false) args3.Add(last);

            return args3;
        }
    }
}
