﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMDR
{
    public abstract class BotModule
    {
        public Bot GetBot()
        {
            return Bot.GetBot();
        }

        public void RegisterCommand(SystemCommand command)
        {
            if (GetBot().CmdCategories.Find(t => t.name.ToLower() == command.commandid.ToLower()) != null)
                throw new Exception("Error: a category exists with that command id.");

            if (command.Categories.Any(t => t.name.ToLower() == command.commandid.ToLower()))
                throw new Exception("Error: a category exists with that command id.");
            
            if (command.commandid.Contains(" "))
                throw new FormatException("Command id cannot contain space symbols.");

            Bot.WriteLine();
            Bot.WriteLine("Registering command " + command.commandid + " from module " + id + "...");
            Bot.WriteLine("    ID = " + command.commandid);
            if (command.helpsyntax != "" ) Bot.WriteLine("    Syntax = " + command.helpsyntax);
            Bot.WriteLine("    Description = " + command.description);
            Bot.WriteLine("    Permission node = " + command.permissionnode);
            Bot.WriteLine("    Allow call from discord = " + command.allowDiscord);
            if (command.allowDiscord)  Bot.WriteLine("    Allow call from discord without command specifier = " + command.setNoCmdPrefix);
            Bot.WriteLine("    Allow call from terminal = " + command.allowTerminal);

            string catPretty = "";
            foreach (CmdCategory c in command.Categories) 
            {
                if (catPretty.Length != 0)
                    catPretty += ", ";
                catPretty += c.name;

                if (GetBot().CmdCategories.Find(t => t.name == c.name) == null)
                {
                    GetBot().CmdCategories.Add(c); 
                }
            }

            Bot.WriteLine("    Categories = " + catPretty);

            Bot.WriteLine();

            GetBot().commands.Add(command);
        }

        public string modulepath = "";
        public string storagepath = "";

        internal ConfigDictionary<String, Object> botconfig;
        public ConfigDictionary<String, Object> GetConfig() => botconfig;

        public void SaveConfig()
        {
            File.WriteAllText(storagepath+"/config.xml", Serializer.Serialize(GetConfig()));
        }

        public void LoadConfig()
        {
            if (File.Exists(storagepath + "/config.xml"))
            {
                botconfig = Serializer.Deserialize<ConfigDictionary<String, Object>>(File.ReadAllText(storagepath + "/config.xml"));
            }
            else botconfig = new ConfigDictionary<String, Object>();
        }

        public abstract string id { get; }
        public abstract string moduledesctiption { get; }

        public abstract void PreInit(Bot bot);
        public abstract void Init(Bot bot);
        public abstract void RegisterCommands(Bot bot);
        public abstract void PostInit(Bot bot);
    }
}
