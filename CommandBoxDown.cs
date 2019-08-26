using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using System.IO;

namespace ItemRestrictorAdvanced
{
    public class CommandBoxDown : IRocketCommand
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
            if (command.Length != 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            try
            {
                string path = $@"{Plugin.Instance.pathTemp}\{player.CSteamID.ToString()}\{command[0]}.dat";
                FileStream fs = new FileStream(path, FileMode.Open);
                fs.Close();
                //fs.Dispose();
            }
            catch(System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"{command[0]} does not exist in your virtual inventory!", Color.red);
                return;
            }
            GiveBox(player, player.CSteamID.ToString(), command[0]);
        }

        private static void GiveBox(UnturnedPlayer player, string steamID, string boxName)
        {
            Block block = Functions.ReadBlock(Plugin.Instance.pathTemp + $@"\{steamID}\{boxName}.dat", 0);
            ushort version = block.readUInt16();
            ushort barricadeID = block.readUInt16();
            ushort barricadeHE = block.readUInt16();
            byte[] numArray = block.readByteArray();
            Item barricade = new Item(barricadeID, 1, 100, numArray);
            Item item = new Item(15, false);
            //Barricade barricade = new Barricade(barricadeID, barricadeHE, numArray, new ItemBarricadeAsset())
            //player.GiveItem(barricade);
            //player.Inventory.tryAddItemAuto(barricade, false, false, false, false);
            //player.Player.inventory.forceAddItemAuto(barricade, false, true, false);
            player.Inventory.forceAddItemAuto(item, false, true, false);
            System.Console.WriteLine("box was given!");
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.