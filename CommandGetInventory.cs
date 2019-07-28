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
            UnturnedPlayer player = (UnturnedPlayer)caller;
            EffectManager.sendUIEffect(8100, 22, player.CSteamID, false);
            List<SteamPlayer> steamPlayers = new List<SteamPlayer>();
            foreach (var steamPlayer in Provider.clients)
                steamPlayers.Add(steamPlayer);
            steamPlayers.Sort(new SteamPlayerCompaper());
            for (byte i = 0; i < Provider.clients.Count; i++)
            {
                EffectManager.sendUIEffectText(22, player.CSteamID, false, $"text{i}", $"{steamPlayers[i].playerID.characterName}");
            }
            player.Player.serversideSetPluginModal(true);
            System.Console.WriteLine($"/gi executed by {player.CharacterName}");
        }
    }
    public class CommandInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "showui";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/closegetinventory <player>  /cgi <player>";
        public List<string> Aliases => new List<string>() { "su" };
        public List<string> Permissions => new List<string>() { "rocket.showui", "rocket.su" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Asset item = Assets.find(EAssetType.ITEM, 15);
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.