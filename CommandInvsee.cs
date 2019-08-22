using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class CommandGetVirtual : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "incloud";
        public string Help => "Shows your virtual inventory using UI";
        public string Syntax => "/incloud";
        public List<string> Aliases => new List<string>() { "invsee" };
        public List<string> Permissions => new List<string>() { "rocket.incloud", "rocket.inc"};
        public static CommandGetInventory Instance { get; private set; }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Block block = Functions.ReadBlock(Plugin.Instance.pathTemp + $"\\{player.CSteamID}\\Heap.dat", 0);
            if(block.block.Length == 0)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, "You do not have a cloud inventory");
                return;
            }
            byte currentPage = 1;
            (List<List<MyItem>> myItemsPages, byte pagesCount) = Functions.GetMyItems(block);
            EffectManager.sendUIEffect(8101, 25, player.CSteamID, false);
            for (byte i = 0; i < myItemsPages[pagesCount - 1].Count; i++)
                EffectManager.sendUIEffectText(25, player.CSteamID, false, $"item{i}", $"{((ItemAsset)Assets.find(EAssetType.ITEM, myItemsPages[pagesCount - 1][i].ID)).itemName}\r\nID: {myItemsPages[pagesCount - 1][i].ID}\r\nCount: {myItemsPages[pagesCount - 1][i].Count}");

        }
        public void OnEffectButtonClick8101(Player callerPlayer, string buttonName)
        {
            if (buttonName.Substring(0, 4) == "item")
            {
                byte.TryParse(buttonName.Substring(4), out byte itemIndex);
                //itemIndex += (byte)((currentPage - 1) * 24); 
                buttonName = "item";
            }

            switch (buttonName)
            {
                case "item":
                    //show 8102
                    //EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                    //if (UIitemsPages[currentPage - 1].Count >= (itemIndex + 1))
                    //    selectedItem = UIitemsPages[currentPage - 1][itemIndex];
                    //else
                    //    return;    
                    //selectedItem = (UIitemsPages[currentPage - 1].Count >= (itemIndex + 1))?(selectedItem = UIitemsPages[currentPage - 1][itemIndex]):(selectedItem = null);
                    //if (UIitemsPages[currentPage - 1].Count >= (itemIndex + 1))// editing item
                    //{
                    //    selectedItem = UIitemsPages[currentPage - 1][itemIndex];
                    //    //backUpItem = UIitemsPages[currentPage - 1][itemIndex];
                    //}
                    //else
                    //selectedItem = null;// + button

                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
                    EffectManager.onEffectButtonClicked += OnEffectButtonClick8102;
                    EffectManager.onEffectTextCommitted += OnTextCommited;
                    EffectManager.sendUIEffect(8102, 24, callerPlayer.channel.owner.playerID.steamID, false);
                    break;

                case "ButtonNext":
                    if (currentPage == PagesCountInv)
                        currentPage = 1;
                    else
                        currentPage++;
                    GetTargetItems();
                    EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                    ShowItemsUI(callerPlayer, currentPage);
                    break;

                case "ButtonPrev":
                    if (currentPage == 1)
                        currentPage = PagesCountInv;
                    else
                        currentPage--;
                    GetTargetItems();
                    EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                    ShowItemsUI(callerPlayer, currentPage);
                    break;

                case "MainPage":
                    if (targetPlayer != null)
                    {
                        targetPlayer.inventory.onInventoryAdded -= OnInventoryChange;
                        targetPlayer.inventory.onInventoryRemoved -= OnInventoryChange;
                    }
                    EffectManager.askEffectClearByID(8101, UnturnedPlayer.FromPlayer(callerPlayer).CSteamID);
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
                    CommandGetInventory.Instance.Execute(UnturnedPlayer.FromPlayer(callerPlayer), null);
                    break;

                case "ButtonExit":
                    if (targetPlayer != null)
                    {
                        targetPlayer.inventory.onInventoryAdded -= OnInventoryChange;
                        targetPlayer.inventory.onInventoryRemoved -= OnInventoryChange;
                    }
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
                    QuitUI(callerPlayer, 8101);
                    break;
                default://non button click
                    return;
            }
        }
    }
    public class CommandGetInventory : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "invsee";
        public string Help => "Shows you someone's inventory using UI";
        public string Syntax => "/invsee or /ins";
        public List<string> Aliases => new List<string>() { "invsee", "ins" };
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