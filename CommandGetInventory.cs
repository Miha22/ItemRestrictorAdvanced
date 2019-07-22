using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class CommandGetInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "getinventory";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/getinventory <player>  /gi <player>";
        public List<string> Aliases => new List<string>() { "getinventory", "gi" };
        public List<string> Permissions => new List<string>() { "rocket.getinventory", "rocket.gi" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            CSteamID steamID = ((UnturnedPlayer)caller).CSteamID;
            EffectManager.sendUIEffect(8100, 22, steamID, false);
            for (byte i = 0; i < 23; i++)
            {
                EffectManager.sendUIEffectText(22, steamID, false, $"text{i}", "someTextThere");
            }
            EffectManager.sendUIEffectText(22, steamID, false, $"text{23}", "");
            //new MyEffectManager().sendUIEffect(8100, 1231, steamID, false, "0nelson - 0", "1nolson - 1", "2lolson - 2", "3nullson - 3", "4nillson - 4");
            System.Console.WriteLine("/gi executed!");
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.