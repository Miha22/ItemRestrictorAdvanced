using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    class ManageCloudUI
    {
        byte itemIndex;
        byte pagesCount;
        byte currentPage;
        List<List<MyItem>> MyItemsPages;

        public ManageCloudUI(List<List<MyItem>> myItemsPages, byte pagesCount)
        {
            currentPage = 1;
            MyItemsPages = myItemsPages;
            this.pagesCount = pagesCount;
        }

        public void OnEffectButtonClick8101(Player callerPlayer, string buttonName)
        {
            if (buttonName.Substring(0, 4) == "item")
            {
                byte.TryParse(buttonName.Substring(4), out itemIndex);
                //itemIndex += (byte)((currentPage - 1) * 24);
                buttonName = "item";
            }

            switch (buttonName)
            {
                case "item":
                    if((itemIndex + 1) <= MyItemsPages[currentPage - 1].Count)
                    {
                        MyItem myItem = MyItemsPages[currentPage - 1][itemIndex];
                        for (byte i = 0; i < myItem.Count; i++)
                            callerPlayer.inventory.tryAddItemAuto(new Item(myItem.ID, myItem.X, myItem.Quality, myItem.State), false, false, false, false);
                    }                  
                    break;

                case "ButtonNext":
                    if (currentPage == pagesCount)
                        currentPage = 1;
                    else
                        currentPage++;
                    EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                    ShowItemsUI(callerPlayer, currentPage);
                    break;

                case "ButtonPrev":
                    if (currentPage == 1)
                        currentPage = pagesCount;
                    else
                        currentPage--;
                    EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                    ShowItemsUI(callerPlayer, currentPage);
                    break;

                case "MainPage":
                    goto case "ButtonPrev";

                case "ButtonExit":
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
                    QuitUI(callerPlayer, 8101);
                    break;
                default://non button click
                    return;
            }
        }
        private void ShowItemsUI(Player callPlayer, byte page)//target player idnex in provider.clients
        {
            try
            {
                EffectManager.sendUIEffect(8101, 26, callPlayer.channel.owner.playerID.steamID, false);
                if (MyItemsPages[page - 1].Count != 0)
                    for (byte i = 0; i < MyItemsPages[page - 1].Count; i++)
                        EffectManager.sendUIEffectText(26, callPlayer.channel.owner.playerID.steamID, false, $"item{i}", $"{((ItemAsset)Assets.find(EAssetType.ITEM, MyItemsPages[pagesCount - 1][i].ID)).itemName}\r\nID: {MyItemsPages[pagesCount - 1][i].ID}\r\nCount: {MyItemsPages[pagesCount - 1][i].Count}");
                EffectManager.sendUIEffectText(26, callPlayer.channel.owner.playerID.steamID, false, "page", $"{page}");
                EffectManager.sendUIEffectText(26, callPlayer.channel.owner.playerID.steamID, false, "pagemax", $"{pagesCount}");
                EffectManager.sendUIEffectText(26, callPlayer.channel.owner.playerID.steamID, false, "playerName", $"Cloud: {callPlayer.channel.owner.playerID.characterName}");
            }
            catch (System.Exception e)
            {
                Rocket.Core.Logging.Logger.LogException(e, "Exception in ManageCloudUI.ShowItemsUI(Player, byte)");
                QuitUI(callPlayer, 8101);
                return;
            }
        }
        private void ReturnLoad(List<List<MyItem>> myItems)
        {
            Block block = new Block();
        }
        private void QuitUI(Player callerPlayer, ushort effectId)
        {
            EffectManager.askEffectClearByID(effectId, callerPlayer.channel.owner.playerID.steamID);
            callerPlayer.serversideSetPluginModal(false);
            //ManageUI.UICallers.Remove(callerPlayer);
            System.Console.WriteLine($"caller: {callerPlayer.channel.owner.playerID.characterName} removed from list!");
            MyItemsPages.Clear();
        }
    }

    public class CommandGetVirtual : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "incloud";
        public string Help => "Shows your virtual inventory using UI";
        public string Syntax => "/incloud or /inc";
        public List<string> Aliases => new List<string>() { "inc" };
        public List<string> Permissions => new List<string>() { "rocket.incloud", "rocket.inc"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Block block = Functions.ReadBlock(Plugin.Instance.pathTemp + $"\\{player.CSteamID}\\Heap.dat", 0);
            if (block.block.Length == 0)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, "You do not have items in cloud inventory!");
                return;
            }
            (List<List<MyItem>> myItemsPages, byte pagesCount) = Functions.GetMyItems(block);
            EffectManager.sendUIEffect(8101, 26, player.CSteamID, false);
            for (byte i = 0; i < pagesCount; i++)
                for (byte j = 0; j < myItemsPages[i].Count; j++)
                    EffectManager.sendUIEffectText(26, player.CSteamID, false, $"item{j}", $"{((ItemAsset)Assets.find(EAssetType.ITEM, myItemsPages[i][j].ID)).itemName}\r\nID: {myItemsPages[i][j].ID}\r\nCount: {myItemsPages[j][i].Count}");
            for (byte i = (byte)(myItemsPages[pagesCount - 1].Count); i < 24; i++)
                EffectManager.sendUIEffectText(26, player.CSteamID, false, $"item{i}", $"");
            EffectManager.sendUIEffectText(26, player.CSteamID, false, "playerName", $"Cloud: {player.CharacterName}");
            EffectManager.onEffectButtonClicked += new ManageCloudUI(myItemsPages, pagesCount).OnEffectButtonClick8101;
            player.Player.serversideSetPluginModal(true);  
        }
    }

    public class CommandGetInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "invsee";
        public string Help => "Shows you someone's inventory using UI";
        public string Syntax => "/invsee or /ins";
        public List<string> Aliases => new List<string>() { "ins" };
        public List<string> Permissions => new List<string>() { "rocket.invsee", "rocket.ins" };
        public static CommandGetInventory Instance { get; private set; }

        public CommandGetInventory()
        {
            Instance = this;
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                UnturnedPlayer lastCaller = (UnturnedPlayer)caller;
                EffectManager.sendUIEffect(8100, 22, lastCaller.CSteamID, false);
                for (byte i = 0; i < Provider.clients.Count; i++)
                    EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
                EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, $"page", "1");
                lastCaller.Player.serversideSetPluginModal(true);
                EffectManager.onEffectButtonClicked += new ManageUI((byte)System.Math.Ceiling(Provider.clients.Count / 24.0), lastCaller.Player).OnEffectButtonClick;// feature
                EffectManager.sendUIEffectText(22, lastCaller.CSteamID, false, "pagemax", $"{ManageUI.PagesCount}");
                ManageUI.UICallers.Add(lastCaller.Player);

                U.Events.OnPlayerConnected += new Refresh(lastCaller.CSteamID).OnPlayersChange;
                U.Events.OnPlayerDisconnected += new Refresh(lastCaller.CSteamID).OnPlayersChange;
            }
            catch (System.Exception e)
            {
                Rocket.Core.Logging.Logger.LogException(e, $"Exception in Invsee: caller: {caller.DisplayName}");
                for (byte i = 0; i < Refresh.Refreshes.Length; i++)
                {
                    Refresh.Refreshes[i].TurnOff(i);
                }
            }

            System.Console.WriteLine($"/gi executed");
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.