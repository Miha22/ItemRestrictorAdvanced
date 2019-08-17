using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using System.IO;
using Steamworks;

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
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            FileInfo file = new FileInfo(Plugin.Instance.pathTemp + player.CSteamID.ToString() + command[0]);
            if (!file.Exists)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"{command[0]} does not exist in your virtual inventory!", Color.red);
                return;
            }

        }

        private static byte[] RiverToState(string steamID, string boxName)
        {
            River river = new River(Plugin.Instance.pathTemp + "\\" + steamID + "\\" + boxName);

        }
    }
    class CommandBoxUp : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sendbox";
        public string Help => "watch on special loot box and execute /sendbox or /sendbox <prefered box name>";
        public string Syntax => "/sendbox or /sendbox <name of your box>";
        public List<string> Aliases => new List<string>() { "db" };
        public List<string> Permissions => new List<string>() { "rocket.sendbox", "rocket.sb" };
        //string path = Plugin.Instance.pathTemp;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length > 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, 10, RayMasks.BARRICADE_INTERACT))
            {
                if (BarricadeManager.tryGetInfo(hit.transform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion r))
                {
                    BarricadeData bdata = r.barricades[index];
                    if (bdata.barricade.id != 3280 || bdata.owner != player.CSteamID.m_SteamID)
                    {
                        Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Error occured: this barricade is not a virtual inventory box or box is not yours.", Color.red);
                        Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Owner steamID: {bdata.owner}\r\nYour steamID: {player.CSteamID.ToString()}");
                        return;
                    }
                    StateToBlock(bdata.barricade, player.CSteamID, (command.Length == 0) ? SetBoxName(Plugin.Instance.pathTemp + "\\" + player.CSteamID.ToString()) : command[1]);
                }
            }
        }

        private static void StateToBlock(Barricade barricade, CSteamID steamID, string boxName)
        {
            Block block = new Block();
            block.writeUInt16(barricade.id);
            block.writeUInt16(barricade.health);
            Plugin.Instance.WriteSpell(block);
            block.writeByteArray(barricade.state);
            Functions.WriteBlock(Plugin.Instance.pathTemp + "\\" + steamID.ToString() + "\\" + boxName, block);
        }

        private string SetBoxName(string path)
        {
            DirectoryInfo[] directories = new DirectoryInfo(path).GetDirectories();
            if (directories == null)
                return "box_0";

            //Directory.CreateDirectory(path + $@"\box_{directories.Length - 1}");
            return $"box_{directories.Length - 1}";
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.