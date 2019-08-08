using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static CommandGetInventory Instance;

        public CommandGetInventory()
        {
            Instance = this;
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
                EffectManager.onEffectButtonClicked += new ManageUI((byte)System.Math.Ceiling((double)Provider.clients.Count / 24.0)).OnEffectButtonClick;// feature
                EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, "pagemax", $"{ManageUI.pagesCount}");
                lastCaller.Player.serversideSetPluginModal(true);

                U.Events.OnPlayerConnected += new Refresh(lastCaller.CSteamID).Execute;
                U.Events.OnPlayerDisconnected += new Refresh(lastCaller.CSteamID).Execute;
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
    public class Refresh
    {
        public static Refresh[] Refreshes = new Refresh[1];
        public CSteamID CallerSteamID { get; set; }
        public byte CurrentPage { get; set; }

        public Refresh(CSteamID steamID)
        {
            CallerSteamID = steamID;
            CurrentPage = 1;
            Refreshes[Refreshes.Length - 1] = this;
            ReSizeUp();
        }

        public async void Execute(UnturnedPlayer connectedPlayer)
        {
            ManageUI.pagesCount = (byte)System.Math.Ceiling((double)Provider.clients.Count / 24.0);
            await Task.Run(() => Do(ManageUI.pagesCount));
        }

        public void TurnOff(byte index)
        {
            U.Events.OnPlayerConnected -= this.Execute;
            U.Events.OnPlayerDisconnected -= this.Execute;
            ReSizeDown(index);
        }

        private void Do(byte pagemax)
        {
            EffectManager.askEffectClearByID(8100, CallerSteamID);
            EffectManager.sendUIEffect(8100, 22, CallerSteamID, false);
            byte multiplier = (byte)((CurrentPage - 1) * 24);
            for (byte i = multiplier; (i < (24 + multiplier)) && (i < (byte)Provider.clients.Count); i++)
                EffectManager.sendUIEffectText(22, CallerSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
            EffectManager.sendUIEffectText(22, CallerSteamID, false, "pagemax", $"{pagemax}");
        }

        private void ReSizeUp()
        {
            Refresh[] newRefreshes = new Refresh[Refresh.Refreshes.Length + 1];
            for (byte i = 0; i < Refreshes.Length; i++)
                newRefreshes[i] = Refreshes[i];
            Refreshes = newRefreshes;
        }

        private void ReSizeDown(byte index)
        {
            Refresh[] newRefreshes = new Refresh[Refresh.Refreshes.Length- 1];
            for (byte i = 0, j = 0; i < Refreshes.Length; i++, j++)
            {
                if (i == index)
                {
                    j--;
                    continue;
                }
                newRefreshes[j] = Refreshes[j];
            }
            Refreshes = newRefreshes;
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