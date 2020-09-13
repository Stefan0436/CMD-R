using System;
namespace CMDR
{
    public class CmdCategory
    {
        public string name = "";
        public string description = "";
        public CmdCategory(string name, string description)
        {
            if (name.Contains(" ")) throw new FormatException("Name cannot contain space symbols.");
            if (Bot.GetBot().commands.Find(t => t.commandid == name) != null) throw new Exception("Error: a command exists with that id.");

            if  (Bot.GetBot().CmdCategories.Find(t=>t.name==name) != null)
            {
                if (Bot.GetBot().CmdCategories.Find(t => t.name == name).description != description)
                {
                    ConsoleColor b = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Bot.WriteLine("[WARNING, CMD CAT CREATION] During creation of the category " + name + ", another was found with the same name and different description, please troubleshoot");
                    Console.ForegroundColor = b;
                }
            }

            this.name = name;
            this.description = description;
        }
    }
}
