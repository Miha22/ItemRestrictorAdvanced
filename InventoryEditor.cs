using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class InventoryEditor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "getinventory";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/getinventory <player>  /gi <player>";
        public List<string> Aliases => new List<string>() { "getinventory", "gi" };
        public List<string> Permissions => new List<string>() { "rocket.getinventory", "rocket.gi" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer playerCaller = (UnturnedPlayer)caller;
            UnturnedPlayer playerCallee = UnturnedPlayer.FromName(command[0]);
            
        }
    }
}
