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
        UnturnedPlayer lastCaller;
        public static CommandGetInventory Instance;

        public CommandGetInventory()
        {
            Instance = this;
        }

        public void Execute(IRocketPlayer caller, string[] command = null)
        {
            try
            {
                lastCaller = (UnturnedPlayer)caller;
                EffectManager.onEffectButtonClicked += new ManageUI().OnEffectButtonClick;// feature
                EffectManager.sendUIEffect(8100, 22, lastCaller.CSteamID, false);
                for (byte i = 0; i < Provider.clients.Count; i++)
                    EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");

                lastCaller.Player.serversideSetPluginModal(true);

                U.Events.OnPlayerConnected += new Refresh(lastCaller.CSteamID).Execute;
                U.Events.OnPlayerDisconnected += new Refresh(lastCaller.CSteamID).Execute;
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("EXCEPTION IN GI EXECUTE!");
                for (byte i = 0; i < Refresh.Refreshes.Length; i++)
                {
                    if (Refresh.Refreshes[i].SteamID.m_SteamID == lastCaller.CSteamID.m_SteamID)
                    {
                        Refresh.Refreshes[i].TurnOff(i);
                        break;
                    }
                }
            }

            System.Console.WriteLine($"/gi executed");
        }
    }
    public class Refresh
    {
        public static Refresh[] Refreshes = new Refresh[1];
        public CSteamID SteamID { get; set; }

        public Refresh(CSteamID steamID)
        {
            SteamID = steamID;
            Refreshes[Refreshes.Length - 1] = this;
            ReSizeUp();
        }

        public async void Execute(UnturnedPlayer connectedPlayer)
        {
            await Task.Run(() => Do());
        }

        public void TurnOff(byte index)
        {
            U.Events.OnPlayerConnected -= this.Execute;
            U.Events.OnPlayerDisconnected -= this.Execute;
            ReSizeDown(index);
        }

        private void Do()
        {
            EffectManager.askEffectClearByID(8100, SteamID);
            EffectManager.sendUIEffect(8100, 22, SteamID, false);
            for (byte i = 0; i < Provider.clients.Count; i++)
                EffectManager.sendUIEffectText(22, SteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
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