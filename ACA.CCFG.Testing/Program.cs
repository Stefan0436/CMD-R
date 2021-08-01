using System;
using System.IO;
using System.Linq;
using ACA.Config.CCFG;
using ACA.FSTools.Extensions;

namespace ACA.CCFGTest
{
    class MainClass // TODO: Move to a ACA Dedicated Project and API (and also a second project based on it, Remote Advanced Testing Suite - RATS!, a CI, CD and tests system/server, free and open source)
    {
        public static string versiontype = "HARDCODED";
        public static ConsoleColor versiontypeColor = ConsoleColor.Red;
        public static string version = "0.0.0.0";
        public static ConsoleColor versionColor = ConsoleColor.DarkYellow;
        public static string description = "TODO: MOVE TO ACA";
        public static ConsoleColor descriptionColor = ConsoleColor.DarkGray;

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("AerialWorks Code API");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" (ACA)");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(" - ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Program version: ");
            Console.ForegroundColor = versiontypeColor;
            Console.Write(versiontype);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(" - ");
            Console.ForegroundColor = versionColor;
            Console.Write(version);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(" - ");
            Console.ForegroundColor = descriptionColor;
            Console.Write(description);
            Console.ResetColor();


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Starting tests...");
            if (args.Count() < 2)
            {
                Console.WriteLine("Enter parameters:");
                args = new string[2];
                Console.WriteLine("Param 1:");
                args[0] = Console.ReadLine();
                Console.WriteLine("Param 2:");
                args[1] = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            // TODO: Test file loading (tasks and subtasks through ccfg, including variable and object storage, bool for allowing the task to fail and continue running, minimal cmd line parameters, cmd line in variables)
            // TODO: Test assembly loading (the assembly containing the tests)

            Console.WriteLine();
            CCFG conf;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("===> Construct config api...");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            try
            {
                conf = new CCFG();
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("<=== PASS");
                Console.WriteLine();
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("<=== FAILED AT CONSTRUCT_CONFIG_API");
                Console.WriteLine("<=== THROW EXCEPTION");
                Console.ResetColor();

                throw ex;
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("===> Load config file from param 1...");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            try
            {
                conf.LoadFile(args[0]);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("<=== PASS");
                Console.WriteLine();
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("<=== FAILED AT LOAD_FILE");
                Console.WriteLine("<=== THROW EXCEPTION");
                Console.ResetColor();
                throw ex;
            }

            //TODO: Test item integrity
            Console.WriteLine("Items: " + conf.GetItems().Count);
            Console.WriteLine("Categories: " + conf.GetCategories().Count);
            void check(Category c, string suffix = "")
            {
                Console.WriteLine("Category Name: " + (suffix == "" ? "" : $"{suffix}/") + c.GetName());
                Console.WriteLine("Item Count: " + c.GetItems().Count);
                foreach (Item i in c.GetItems())
                {
                    Console.WriteLine(i.id + " = " + i.value);
                }
                foreach (Category c2 in c.GetCategories())
                {
                    Console.WriteLine();
                    check(c2, suffix + "/" + c.GetName());
                }
                Console.WriteLine();
            }

            check(conf);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("===> Testing saving...");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            try
            {
                conf.SaveFile(args[1]);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("<=== PASS");
                Console.WriteLine();
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("<=== FAILED AT SAVE_TEST");
                Console.WriteLine("<=== THROW EXCEPTION");
                Console.ResetColor();
                throw ex;
            }


            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("===> Testing loading from string...");
            Console.ResetColor();
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            try
            {
                CCFG cf2 = CCFG.FromString(File.ReadAllText(args[1]));
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("     ===> Checking integrity...");
                    //TODO: Test integrity

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("     <=== PASS");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("     <=== FAILED AT INTEGRITY_CHECK");
                    Console.WriteLine("     <=== THROW EXCEPTION");
                    Console.ResetColor();
                    throw ex;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("<=== PASS");
                Console.ResetColor();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("<=== FAILED AT LOAD_STRING_TEST");
                Console.WriteLine("<=== THROW EXCEPTION");
                Console.ResetColor();
                throw ex;
            }
        }
    }
}

