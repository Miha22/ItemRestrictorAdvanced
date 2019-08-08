using System;
using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using System.Globalization;
using Logger = Rocket.Core.Logging.Logger;

namespace ItemRestrictorAdvanced
{
    class ManageUI
    {
        private byte playerIndex;
        private byte itemIndex;
        private byte currentPage;
        public static byte pagesCount;
        private byte pagesCountInv;

        public ManageUI(byte pagesCount)
        {
            currentPage = 1;
            ManageUI.pagesCount = pagesCount;// !
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
                if (Provider.clients.Count < ((playerIndex + 1) * pagesCount))
                    return;

                ClickPlayer(callerPlayer, playerIndex);
                for (byte i = 0; i < Refresh.Refreshes.Length; i++)
                {
                    if (Refresh.Refreshes[i].CallerSteamID.m_SteamID == callerPlayer.channel.owner.playerID.steamID.m_SteamID)
                    {
                        Refresh.Refreshes[i].TurnOff(i);
                        break;
                    }
                }
                EffectManager.onEffectButtonClicked -= OnEffectButtonClick;
                EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
            }
            else if(buttonName == "ButtonNext")
            {
                if(currentPage >= pagesCount)
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
                    EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, false, "page", $"{pagesCount}");
                    currentPage = pagesCount;
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
                QuitUI(callerPlayer, 8101);
                CommandGetInventory.Instance.Execute(UnturnedPlayer.FromPlayer(callerPlayer));
            }
            else
                SaveExitAddItem(callerPlayer);

            EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
        }

        public void OnEffectButtonClick8102(Player callerPlayer, string buttonName)
        {

        }

        private void QuitUI(Player callerPlayer, ushort effectId)
        {
            EffectManager.askEffectClearByID(effectId, UnturnedPlayer.FromPlayer(callerPlayer).CSteamID);
            callerPlayer.serversideSetPluginModal(false);
        }

        private void ClickPlayer(Player callerPlayer, byte index)//target player idnex in provider.clients
        {
            //UnturnedPlayer unturnedPlayerTarget = UnturnedPlayer.FromSteamPlayer(Provider.clients[index]);
            Items[] pages = new Items[Provider.clients[playerIndex].player.channel.owner.player.inventory.items.Length];
            try
            {
                for (byte i = 0; i < pages.Length; i++)
                    pages[i] = Provider.clients[playerIndex].player.channel.owner.player.inventory.items[i];
            }
            catch (Exception)
            {
                Logger.LogException(new Exception($"Internal exception: Player not found: {Provider.clients[playerIndex].player.channel.owner.playerID.characterName} has just left the server.")); //make then a write to Inventory.dat (to do)
            }

            EffectManager.askEffectClearByID(8100, callerPlayer.channel.owner.playerID.steamID);
            EffectManager.sendUIEffect(8101, 23, callerPlayer.channel.owner.playerID.steamID, false);
            //EffectManager.sendUIEffectText(23, callerPlayer.channel.owner.playerID.steamID, false)
            List<MyItem> myItems = new List<MyItem>();
            Console.WriteLine($"target inv null? {pages == null}");
            Console.WriteLine($"");

            Console.WriteLine($"pages count: {pages.Length}");
            foreach (var page in pages)
            {
                if (page == null)
                    continue;
                Console.WriteLine($"items in page: {page.items.Count}");
                foreach (var item in page.items)
                {
                    Console.WriteLine("step 1");
                    MyItem myItem = new MyItem(item.item.id, item.item.amount, item.item.quality, item.item.state);
                    Console.WriteLine("step 2");
                    if (ItemRestrictor.Instance.HasItem(myItem, myItems))
                        continue;
                    else
                        myItems.Add(myItem);
                    Console.WriteLine("step 3");
                }
            }
            Console.WriteLine("step 4");
            foreach (var item in myItems)
            {
                Console.WriteLine($"item: {item.ID}, {item.x}");
            }
            Console.WriteLine("step 5");
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
