using System;
namespace CMDR.GetPermCmdModule
{
    public class GetPermCmdModule : BotModule
    {
        public override string id => "GetPermCmdModule";

        public override string moduledesctiption => "Adds the GetPerm command";


        public override void Init(Bot bot)
        {

        }

        public override void PostInit(Bot bot)
        {
            RegisterCommand(new GetPermCommand());
        }

        public override void PreInit(Bot bot)
        {

        }

        public override void RegisterCommands(Bot bot)
        {

        }
    }
}
