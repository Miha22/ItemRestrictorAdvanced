using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;

namespace ItemRestrictorAdvanced
{
    class CommandBoxDown : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "dropbox";
        public string Help => "drops your vitrual box into a real box";
        public string Syntax => "/dropbox box_<index> or /dropbox <name of your box>";
        public List<string> Aliases => new List<string>() { "db" };
        public List<string> Permissions => new List<string>() { "rocket.dropbox", "rocket.db" };
        //string path = Plugin.Instance.pathTemp;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length > 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, (IRocketCommand)this);
            }
        }
    }
    class CommandBoxUp : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sendbox";
        public string Help => "send your real box to vitrual box";
        public string Syntax => "/sendbox or /sendbox <name of your box>";
        public List<string> Aliases => new List<string>() { "db" };
        public List<string> Permissions => new List<string>() { "rocket.sendbox", "rocket.sb" };
        //string path = Plugin.Instance.pathTemp;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length > 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, (IRocketCommand)this);
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            List<Transform> barricades = new List<Transform>();
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, 10, RayMasks.BARRICADE_INTERACT))
            {
                Transform transform = hit.transform;

                if (BarricadeManager.tryGetInfo(transform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion r))
                {
                    BarricadeData bdata = r.barricades[index];
                    //bdata.barricade.stat
                }
            }
        }
        private static void ByteToBlock(byte[] state)
        {

        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.