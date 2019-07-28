using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class CommandCloseGetInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "closegetinventory";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/closegetinventory <player>  /cgi <player>";
        public List<string> Aliases => new List<string>() { "closegetinventory", "cgi" };
        public List<string> Permissions => new List<string>() { "rocket.closegetinventory", "rocket.cgi" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            ((UnturnedPlayer)caller).Player.serversideSetPluginModal(false);
            EffectManager.askEffectClearByID(8100, ((UnturnedPlayer)caller).CSteamID);
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.