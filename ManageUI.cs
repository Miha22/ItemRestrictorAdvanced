using System;
using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using System.Globalization;
using Logger = Rocket.Core.Logging.Logger;

namespace ItemRestrictorAdvanced
{
    sealed class ManageUI
    {
        public static List<ManageUI> Instances;
        public static List<Player> UICallers;
        internal static byte PagesCount { get; set; }
        public byte PagesCountInv { get; set; }
        private byte playerIndex;
        private byte itemIndex;
        private byte currentPage;
        private Player targetPlayer;
        private List<List<MyItem>> UIitemsPages;

        static ManageUI()
        {
            Instances = new List<ManageUI>();
            UICallers = new List<Player>();
        }

        public ManageUI(byte pagesCount)
        {
            currentPage = 1;
            ManageUI.PagesCount = pagesCount;// !
            UIitemsPages = new List<List<MyItem>>();
            Instances.Add(this);
        }

        public static void UnLoad()
        {
            if(ManageUI.Instances.Count != 0)
            {
                foreach (ManageUI manageUI in ManageUI.Instances)
                    manageUI.UnLoadEvents();
            }
            
            foreach (Player caller in UICallers)
            {
                EffectManager.askEffectClearByID(8100, caller.channel.owner.playerID.steamID);
                EffectManager.askEffectClearByID(8101, caller.channel.owner.playerID.steamID);
                EffectManager.askEffectClearByID(8102, caller.channel.owner.playerID.steamID);
                caller.serversideSetPluginModal(false);
            }
        }

        private void UnLoadEvents()
        {
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick;
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick8101;
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick8102;
        }

        private void SetCurrentPage(byte page, ulong mSteamID)
        {
            for (byte i = 0; i < Refresh.Refreshes.Length; i++)
            {
                if (Refresh.Refreshes[i].CallerSteamID.m_SteamID == mSteamID)
                {
                    Refresh.Refreshes[i].CurrentPage = page;
                    return;
                }
            }
        }

        private void SetPlayersList(byte CurrentPage, Steamworks.CSteamID CallerSteamID)
        {
            byte multiplier = (byte)((CurrentPage - 1) * 24);
            for (byte i = multiplier; (i < (24 + multiplier)) && (i < (byte)Provider.clients.Count); i++)
                EffectManager.sendUIEffectText(22, CallerSteamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
        }

        public void OnEffectButtonClick(Player callerPlayer, string buttonName)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"text[0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);
            byte.TryParse(buttonName.Substring(4), out playerIndex);
            if (regex.IsMatch(buttonName))
            {
                if (Provider.clients.Count < ((playerIndex + 1) * PagesCount))
                    return;
                playerIndex = (byte)((currentPage - 1) * 24);
                currentPage = 1;
                try
                {
                    targetPlayer = Provider.clients[playerIndex].player;
                }
                catch (Exception)
                {
                    Logger.LogError($"Internal error: Player not found: player has just left the server.");
                    return;
                }
                targetPlayer.inventory.onInventoryAdded += OnInventoryChange;
                targetPlayer.inventory.onInventoryRemoved += OnInventoryChange;
                GetItems();
                PlayerClick(callerPlayer, currentPage);

                EffectManager.onEffectButtonClicked -= OnEffectButtonClick;
                EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
                for (byte i = 0; i < Refresh.Refreshes.Length; i++)
                {
                    if (Refresh.Refreshes[i].CallerSteamID.m_SteamID == callerPlayer.channel.owner.playerID.steamID.m_SteamID)
                    {
                        Refresh.Refreshes[i].TurnOff(i);
                        break;
                    }
                }
            }
            else if(buttonName == "ButtonNext")
            {
                if(currentPage >= PagesCount)
                {
                    EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, false, "page", $"1");
                    currentPage = 1;
                    SetCurrentPage(1, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                    SetPlayersList(currentPage, callerPlayer.channel.owner.playerID.steamID);
                }
                else
                {
                    EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, false, "page", $"{currentPage++}");
                    SetCurrentPage(currentPage, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                    SetPlayersList(currentPage, callerPlayer.channel.owner.playerID.steamID);
                    //send current page
                }   
            }
            else if(buttonName == "ButtonPrev")
            {
                if (currentPage <= 1)
                {
                    EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, false, "page", $"{PagesCount}");
                    currentPage = PagesCount;
                    SetCurrentPage(currentPage, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                    SetPlayersList(currentPage, callerPlayer.channel.owner.playerID.steamID);
                    //send 1st page
                }
                else
                {
                    EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, false, "page", $"{currentPage--}");
                    SetCurrentPage(currentPage, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                    SetPlayersList(currentPage, callerPlayer.channel.owner.playerID.steamID);
                    //send current page
                }
            }
            else
            {
                EffectManager.onEffectButtonClicked -= OnEffectButtonClick;
                QuitUI(callerPlayer, 8100);
            }
            //Logger.LogException(new MissingMethodException("Internal exception: Missing Method: a method is missing in Dictionary or button name mismatched. \n"));
        }

        public void OnEffectButtonClick8101(Player callerPlayer, string buttonName)
        {
            byte.TryParse(buttonName.Substring(4), out itemIndex);
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"item[0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);
            if (regex.IsMatch(buttonName))
            {
                return;
            }
                
            else if (buttonName == "ButtonExit")
            {
                QuitUI(callerPlayer, 8101);
            }
            else if (buttonName == "MainPage")
            {
                EffectManager.askEffectClearByID(8101, UnturnedPlayer.FromPlayer(callerPlayer).CSteamID);
                CommandGetInventory.Instance.Execute(UnturnedPlayer.FromPlayer(callerPlayer));
            }
            else
                SaveExitAddItem(callerPlayer);

            EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
        }

        private void PlayerClick(Player callerPlayer, byte page)//target player idnex in provider.clients
        {
            EffectManager.askEffectClearByID(8100, callerPlayer.channel.owner.playerID.steamID);
            EffectManager.sendUIEffect(8101, 23, false);
            for (byte i = 0; i < UIitemsPages[page - 1].Count; i++)
            {
                EffectManager.sendUIEffectText(23, callerPlayer.channel.owner.playerID.steamID, false, $"item{i}", $"{((ItemAsset)Assets.find(EAssetType.ITEM, UIitemsPages[page - 1][i].ID)).itemName} \r\n Count: {UIitemsPages[page][i].Count}");
            }
            //foreach (var myItems in UIitemsPages)
            //{
            //    Console.WriteLine($"count: {myItems.Count}");
            //}
        }

        private void GetItems()
        {
            Items[] pages;
            try
            {
                pages = targetPlayer.inventory.items;// array (reference type in stack => by value)
            }
            catch (Exception)
            {
                // load to Inventory.dat and maybe to virtual inventory
                return;
            }
            List<MyItem> items = new List<MyItem>();

            foreach (var page in pages)
            {
                if (page == null)
                    continue;
                foreach (ItemJar item in page.items)
                {
                    MyItem myItem = new MyItem(item.item.id, item.item.amount, item.item.quality, item.item.state);
                    if (ItemRestrictor.Instance.HasItem(myItem, items))
                        continue;
                    else
                        items.Add(myItem);
                }
            }

            ushort counter = 0;
            for (byte i = 0; i < (byte)Math.Ceiling(items.Count / 24.0); i++)
            {
                List<MyItem> myPage = new List<MyItem>();
                for (ushort j = 0; (j < 24) && (counter < (ushort)items.Count); j++, counter++)
                    myPage.Add(items[j + (24 * i)]);
                UIitemsPages.Add(myPage);
            }
            PagesCountInv = (byte)UIitemsPages.Count;
        }

        public void OnEffectButtonClick8102(Player callerPlayer, string buttonName)
        {

        }

        private void QuitUI(Player callerPlayer, ushort effectId)
        {
            EffectManager.askEffectClearByID(effectId, UnturnedPlayer.FromPlayer(callerPlayer).CSteamID);
            callerPlayer.serversideSetPluginModal(false);
            ManageUI.UICallers.Remove(callerPlayer);
            Console.WriteLine($"caller: {callerPlayer.channel.owner.playerID.characterName} removed from list!");
            UIitemsPages.Clear();
        }

        private async void OnInventoryChange(byte page, byte index, ItemJar item)
        {
            Console.WriteLine($"page: {page}, index: {index}, id: {item.item.id}");
        }

        private void SaveExitAddItem(Player callerPlayer)
        {
            string id = "";
            string x = "";
            TextInfo text = CultureInfo.CurrentCulture.TextInfo;
            EffectManager.sendEffectTextCommitted("ID", id);
            EffectManager.sendEffectTextCommitted("x", x);
            Console.WriteLine();
            Console.WriteLine($"ID: {id}, x: {x}");
            Console.WriteLine();
        }
    }
}
