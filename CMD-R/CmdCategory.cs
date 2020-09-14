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
            if (name.ToLower() == "page") throw new FormatException("Reserved category.");

            if (Bot.GetBot().commands.Find(t => t.commandid.ToLower() == name.ToLower()) != null) throw new Exception("Error: a command exists with that id.");

            if  (Bot.GetBot().CmdCategories.Find(t=>t.name.ToLower() == name.ToLower()) != null)
            {
                if (Bot.GetBot().CmdCategories.Find(t => t.name.ToLower() == name.ToLower()).description != description)
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
