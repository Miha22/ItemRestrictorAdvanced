using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    class CommandGetInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "getinventory";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/getinventory <player>  /gi <player>";
        public List<string> Aliases => new List<string>() { "getinventory", "gi" };
        public List<string> Permissions => new List<string>() { "rocket.getinventory", "rocket.gi" };
        public static CommandGetInventory Instance { get; private set; }

        public CommandGetInventory()
        {
            Instance = this;
        }
        private async void OnInventoryChange(byte page, byte index, ItemJar item)
        {
            System.Console.WriteLine($"page: {page}, index: {index}, id: {item.item.id}");
        }

        public void Execute(IRocketPlayer caller, string[] command = null)
        {
            try
            {
                UnturnedPlayer lastCaller = (UnturnedPlayer)caller;
                EffectManager.sendUIEffect(8100, 22, lastCaller.CSteamID, false);
                for (byte i = 0; i < Provider.clients.Count; i++)
                    EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
                EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"page", "1");
                lastCaller.Player.serversideSetPluginModal(true);
                EffectManager.onEffectButtonClicked += new ManageUI((byte)System.Math.Ceiling((double)Provider.clients.Count / 24.0), lastCaller.Player).OnEffectButtonClick;// feature
                EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, "pagemax", $"{ManageUI.PagesCount}");
                ManageUI.UICallers.Add(lastCaller.Player);
                //lastCaller.Player.inventory.onInventoryAdded += OnInventoryChange;
                //lastCaller.Player.inventory.onInventoryRemoved += OnInventoryChange;

                U.Events.OnPlayerConnected += new Refresh(lastCaller.CSteamID).OnPlayersChange;
                U.Events.OnPlayerDisconnected += new Refresh(lastCaller.CSteamID).OnPlayersChange;
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("EXCEPTION IN GI EXECUTE!");
                for (byte i = 0; i < Refresh.Refreshes.Length; i++)
                {
                    Refresh.Refreshes[i].TurnOff(i);
                }
            }

            System.Console.WriteLine($"/gi executed");
        }
    }
    //public class RefreshOnD
    //{
    //    private CSteamID _steamID;
    //    public RefreshOnD(CSteamID steamID)
    //    {
    //        this._steamID = steamID;
    //    }
    //    public void Execute(UnturnedPlayer connectedPlayer)
    //    {
    //        EffectManager.askEffectClearByID(8100, _steamID);
    //        EffectManager.sendUIEffect(8100, 22, _steamID, false);
    //        for (byte i = 0; i < Provider.clients.Count; i++)
    //            EffectManager.sendUIEffectText(22, _steamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");

    //        U.Events.OnPlayerDisconnected -= Execute;
    //    }
    //}
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.