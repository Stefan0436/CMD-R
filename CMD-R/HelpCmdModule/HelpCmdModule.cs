using System;

namespace CMDR.HelpCmdModule
{
    public class HelpCmdModule : BotModule
    {
        public override string id => "HelpCmdModule";

        public override string moduledesctiption => "Adds the help discord command";

        public override void Init(Bot bot)
        {

        }

        public override void PostInit(Bot bot)
        {
            RegisterCommand(new HelpCommand());
        }

        public override void PreInit(Bot bot)
        {
            GetConfig().AddIfAbsent("test 1", 2523);
            GetConfig().AddIfAbsent("test 2", 25123);
            GetConfig().AddIfAbsent("test", "Hi");
        }

        public override void RegisterCommands(Bot bot)
        {

        }
    }
}
