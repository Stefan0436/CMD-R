using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CMDR
{
    public static class Terminal
    {
        public static BetterLogger betterLogger = new BetterLogger();

        public static void WriteLine(string msg="")
        {
            betterLogger.ConsoleWriteLine(msg);
        }
        public static string ReadLine()
        {
            return betterLogger.GetInputLine("TERM");
        }

        public static string ReadLine(string str)
        {
            return betterLogger.GetInputLine(str);
        }
    }

    public class BetterLogger
    {
        // Expose Clipboard as a TextReader.

        public static class ClipboardTextReaderFactory

        {

            static public TextReader GetReader()

            {

                IDataObject iData = Clipboard.GetDataObject();

                string s = null;
                try
                {
                    s = (string)iData.GetData(DataFormats.Text);
                }
                catch
                {

                }
                if (s == null) { s = ""; }

                return new StringReader(s);

            }

        }

        List<string> cmds = new List<string>();
        int cmdnum = 0;

        string memory = "";
        string pref = "";
        bool firstafterpress = false;
        bool usefirstafterpress = false;
        List<string> queuedinput = new List<string>();
        [STAThread]
        public string GetInputLine(string prefix)
        {
            return GetInputLineNoSuffix(prefix + "> ");
        }
        [STAThread]
        public string GetInputLineNoSuffix(string prefix)
        {
            pref = prefix;
            Prompting = true;
            if (pref == "" == false) Console.Write(pref.Replace("$<path>", Environment.CurrentDirectory) + characters);
            if (queuedinput.Count == 0 == false)
            {
                string line2 = queuedinput[0];
                queuedinput.RemoveAt(0);
                bool run = false;
                if (line2.EndsWith("\n"))
                {
                    line2 = line2.Remove(line2.LastIndexOf("\n"));
                    run = true;
                }
                characters += line2;
                Console.Write(line2);
                if (cmdnum == cmds.Count == false)
                {
                    cmdnum = cmds.Count;
                }
                usefirstafterpress = false;
                firstafterpress = false;
                if (run)
                {
                    string s = characters;
                    try
                    {
                        if (cmdnum == cmds.Count)
                        {
                            cmds.Add(characters);
                            cmdnum++;
                            memory = "";
                            usefirstafterpress = false;
                        }
                        else
                        {
                            firstafterpress = true;
                            usefirstafterpress = true;
                            cmdnum++;
                            memory = "";
                        }
                    }
                    catch
                    {

                    }
                    characters = "";
                    Console.WriteLine();
                    Prompting = false;
                    return s;
                }
            }
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.UpArrow && key.Key != ConsoleKey.DownArrow && key.Key != ConsoleKey.LeftArrow && key.Key != ConsoleKey.RightArrow)
                {
                    if (key.KeyChar == '\0' == false && key.Key == ConsoleKey.Tab == false && key.Modifiers.HasFlag(ConsoleModifiers.Control) == false)
                    {
                        characters += key.KeyChar;
                        Console.Write(key.KeyChar);
                        if (cmdnum == cmds.Count == false)
                        {
                            cmdnum = cmds.Count;
                        }
                        usefirstafterpress = false;
                        firstafterpress = false;
                    }
                    else if (key.Key == ConsoleKey.V && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        string data = ClipboardTextReaderFactory.GetReader().ReadToEnd();

                        data = data.Replace("\r", "");
                        if (data.Contains("\n"))
                        {
                            bool endswithrun = data.EndsWith("\n");

                            int i = 0;
                            foreach (string line in data.Split('\n'))
                            {
                                if ((data.Split('\n').Count() - 1) == i++)
                                {
                                    if (endswithrun)
                                    {
                                        queuedinput.Add(line + "\n");
                                    }
                                    else
                                    {
                                        queuedinput.Add(line);
                                    }
                                }
                                else
                                {
                                    queuedinput.Add(line + "\n");
                                }
                            }

                            string line2 = queuedinput[0];
                            queuedinput.RemoveAt(0);
                            bool run = false;
                            if (line2.EndsWith("\n"))
                            {
                                line2 = line2.Remove(line2.LastIndexOf("\n"));
                                run = true;
                            }
                            characters += line2;
                            Console.Write(line2);
                            if (cmdnum == cmds.Count == false)
                            {
                                cmdnum = cmds.Count;
                            }
                            usefirstafterpress = false;
                            firstafterpress = false;
                            if (run)
                            {
                                string s = characters;
                                try
                                {
                                    if (cmdnum == cmds.Count)
                                    {
                                        cmds.Add(characters);
                                        cmdnum++;
                                        memory = "";
                                        usefirstafterpress = false;
                                    }
                                    else
                                    {
                                        firstafterpress = true;
                                        usefirstafterpress = true;
                                        cmdnum++;
                                        memory = "";
                                    }
                                }
                                catch
                                {

                                }
                                characters = "";
                                Console.WriteLine();
                                Prompting = false;
                                return s;
                            }
                        }
                        else
                        {
                            queuedinput.Add(data);
                            string line2 = queuedinput[0];
                            queuedinput.RemoveAt(0);
                            characters += line2;
                            Console.Write(line2);
                            if (cmdnum == cmds.Count == false)
                            {
                                cmdnum = cmds.Count;
                            }
                            usefirstafterpress = false;
                            firstafterpress = false;
                        }
                    }
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && characters.Length > 0)
                    {
                        characters = characters.Substring(0, (characters.Length - 1));
                        if (Console.CursorLeft == 0)
                        {
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        }
                        else Console.Write("\b \b");
                        if (cmdnum == cmds.Count == false)
                        {
                            cmdnum = cmds.Count;
                        }
                        usefirstafterpress = false;
                        firstafterpress = false;
                    }
                    else if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (cmdnum == 0 == false)
                        {
                            if (cmds.Count == cmdnum)
                            {
                                memory = characters;
                            }
                            while (characters == "" == false)
                            {
                                characters = characters.Substring(0, (characters.Length - 1));
                                if (Console.CursorLeft == 0)
                                {
                                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                                    Console.Write(" ");
                                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                                }
                                else Console.Write("\b \b");
                            }

                            usefirstafterpress = false;
                            firstafterpress = false;
                            cmdnum--;
                            characters = cmds[cmdnum];
                            Console.Write(cmds[cmdnum]);
                        }
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (usefirstafterpress)
                        {
                            if (firstafterpress)
                            {
                                int c = cmdnum - 1;
                                if (c == cmds.Count + 1 == false)
                                {
                                    cmdnum--;
                                }
                            }
                        }
                        if (cmdnum == cmds.Count == false)
                        {
                            while (characters == "" == false)
                            {
                                characters = characters.Substring(0, (characters.Length - 1));
                                if (Console.CursorLeft == 0)
                                {
                                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                                    Console.Write(" ");
                                    Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                                }
                                else Console.Write("\b \b");
                            }
                            firstafterpress = false;
                            usefirstafterpress = false;
                            if ((cmdnum + 1) == cmds.Count)
                            {
                                cmdnum++;
                                Console.Write(memory);
                                characters = memory;
                            }
                            else
                            {
                                cmdnum++;
                                characters = cmds[cmdnum];
                                Console.Write(cmds[cmdnum]);
                            }
                        }
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        string s = characters;
                        try
                        {
                            if (cmdnum == cmds.Count)
                            {
                                cmds.Add(characters);
                                cmdnum++;
                                memory = "";
                                usefirstafterpress = false;
                            }
                            else
                            {
                                firstafterpress = true;
                                usefirstafterpress = true;
                                cmdnum++;
                                memory = "";
                            }
                        }
                        catch
                        {

                        }
                        characters = "";
                        Console.WriteLine();
                        Prompting = false;
                        return s;
                    }
                }
            } while (true);
        }

        public bool overrideprompt = false;
        public bool overridestate = false;

        public void Update()
        {
            try
            {
                if (Prompting)
                {
                    string str = pref.Replace("$<path>", Environment.CurrentDirectory) + "> ";

                    if (pref == "") str = "";

                    int l = (str + characters).Length;

                    for (int i = 0; i < l; i++)
                    {
                        if (Console.CursorLeft == 0)
                        {
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                            Console.Write(" ");
                            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        }
                        else
                        {
                            Console.Write("\b \b");
                        }
                    }

                    Console.SetCursorPosition(0, Console.CursorTop);
                }

                if (overrideprompt)
                {
                    overrideprompt = false;
                    Prompting = overridestate;
                }

                if (Prompting)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);

                    if (pref == "" == false) Console.Write(pref.Replace("$<path>", Environment.CurrentDirectory) + characters);
                    else { Console.Write(characters); }
                }
            }
            catch
            {

            }
        }

        string characters = "";
        public StreamWriter writer2;
        public bool Prompting = false;
        public void ConsoleWriteLine(string msg)
        {
            try
            {
                writer2.WriteLine(msg);
            }
            catch
            {

            }
            try
            {
                if (Prompting)
                {
                    string str = pref.Replace("$<path>", Environment.CurrentDirectory) + "> ";

                    if (pref == "") str = "";

                    int l = (str + characters).Length;

                    for (int i = 0; i < l; i++)
                    {
                        if (Console.CursorLeft != 0)
                        {
                            Console.Write("\b \b");
                        }
                    }

                    Console.SetCursorPosition(0, Console.CursorTop);
                }

                if (overrideprompt)
                {
                    overrideprompt = false;
                    Prompting = overridestate;
                }

                Console.WriteLine(msg);

                if (Prompting)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);

                    if (pref == "" == false) Console.Write(pref.Replace("$<path>", Environment.CurrentDirectory) + characters);
                    else { Console.Write(characters); }
                }
            }
            catch
            {

            }
        }

        public void ConsoleWrite(string msg)
        {
            Console.WriteLine("Log.Write is not supported!");
        }

        public void ChangeTilte(string text)
        {
            try
            {
                Console.Title = text;
            }
            catch
            {

            }
        }
    }
}
