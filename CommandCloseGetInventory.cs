using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class CommandCloseGetInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "closegetinventory";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/closegetinventory <player>  /cgi <player>";
        public List<string> Aliases => new List<string>() { "closegetinventory", "cgi" };
        public List<string> Permissions => new List<string>() { "rocket.closegetinventory", "rocket.cgi" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer lastCaller = (UnturnedPlayer)caller;

            for (byte g = 0; g < 5; g++)
            {
                System.Console.WriteLine("cgi {0}", g);
                EffectManager.sendUIEffect(8100, 22, lastCaller.CSteamID, false);
                for (byte i = 0; i < Provider.clients.Count; i++)
                    EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
                EffectManager.askEffectClearByID(8100, lastCaller.CSteamID);
                EffectManager.sendUIEffect(8100, 22, lastCaller.CSteamID, false);
                for (byte i = 0; i < Provider.clients.Count; i++)
                    EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
            }
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.