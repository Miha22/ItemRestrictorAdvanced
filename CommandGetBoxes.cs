using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using System.IO;

namespace ItemRestrictorAdvanced
{
    class CommandGetBoxes : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "listbox";
        public string Help => "tells you what virtual boxes you have";
        public string Syntax => "/listbox or /listbox <player name>";
        public List<string> Aliases => new List<string>() { "lb" };
        public List<string> Permissions => new List<string>() { "rocket.listbox", "rocket.lb" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if(command.Length == 0)
            {
                DirectoryInfo directory = new DirectoryInfo(Plugin.Instance.pathTemp + "\\" + ((UnturnedPlayer)caller).CSteamID.ToString());
                if (!directory.Exists)
                {
                    Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"No boxes exist in your virtual inventory!", Color.red);
                    return;
                }
                string boxes = "";
                foreach (FileInfo file in directory.GetFiles())
                {
                    boxes += file.Name.Split('.')[0] + " ";
                }
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Boxes: {boxes}");
            }
            else if(command.Length == 1)
            {
                if(PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer player))
                {
                    DirectoryInfo directory = new DirectoryInfo(Plugin.Instance.pathTemp + "\\" + player.playerID.steamID.ToString());
                    if (!directory.Exists)
                    {
                        Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"No boxes exist in your virtual inventory!", Color.red);
                        return;
                    }
                    string boxes = "";
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        boxes += file.Name.Split('.')[0] + " ";
                    }
                    Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Boxes: {boxes}");
                }
                else
                    Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Player: {command[0]} was not found!", Color.red);
            }
            else
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.