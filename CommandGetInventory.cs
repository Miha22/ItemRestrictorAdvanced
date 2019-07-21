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
            //MyEffectManager.tellUIEffectParams(steamID, 8100, 231, "nelson - 0", "nolson - 1", "lolson - 2", "nullson - 3", "nullson - 4");
            //EffectManager.createAndFormatUIEffect()
            System.Console.WriteLine("/gi executed!");
            new MyEffectManager().sendUIEffect(8100, 1231, steamID, false, "0nelson - 0", "1nolson - 1", "2lolson - 2", "3nullson - 3", "4nillson - 4");
            //System.Console.WriteLine($"SteamChannelMethods: ");
            //EffectManager.createAndFormatUIEffect(8100, 231, "nelson", "nolson", "lolson", "nullson", "nelsing", "nelson sexton", "sexy nelson", "nelson sex", "nelson gay", $"Players here: {Provider.clients.Count.ToString()}", ((UnturnedPlayer)caller).CSteamID.ToString(), caller.DisplayName);
            //MyEffectManager.tellUIEffectParamsArgs(((UnturnedPlayer)caller).CSteamID, 8100, 12333, "nelson", "nolson", "lolson", "nullson", "nelsing", "nelson sexton", "sexy nelson", "nelson sex", "nelson gay", $"Players here: {Provider.clients.Count.ToString()}", ((UnturnedPlayer)caller).CSteamID.ToString(), caller.DisplayName);
            //EffectManager.createAndFormatUIEffect(8100, 231, (object)"nelson");
            //EffectManager.sendUIEffect(8100, 1233, ((UnturnedPlayer)caller).CSteamID, false, "neson gay");
            //EffectManager.createUIEffect(8100, 21232); false,
            //EffectManager.sendUIEffectText(123, steamID, false, "childName", "Some text here and again some text more and more");
        }
    }
}
