using System;
using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using System.Globalization;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.API;

namespace ItemRestrictorAdvanced
{
    sealed class CommandUIEmergency : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "EmergencyUIstop";
        public string Help => "Use this command in case if UI craches and everybody has non clearable UI";
        public string Syntax => "/emuistop";
        public List<string> Aliases => new List<string>() { "emuistop", "emergencyuistop" };
        public List<string> Permissions => new List<string>() { "rocket.emergencyuistop", "rocket.emuistop" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            ManageUI.UnLoad();
            Console.WriteLine(ManageUI.Instances == null);
            Logger.Log("Inventory UI stoped");
        }
    }
    sealed class ManageUI
    {
        public static List<ManageUI> Instances;
        internal static byte PagesCount { get; set; }
        public byte PagesCountInv { get; set; }
        private byte playerIndex;
        private byte itemIndex;
        private byte currentPage;
        private Player targetPlayer;
        private List<MyItem> myItems;

        public ManageUI(byte pagesCount)
        {
            currentPage = 1;
            ManageUI.PagesCount = pagesCount;// !
            myItems = new List<MyItem>();
            Instances.Add(this);
        }

        public static void UnLoad()
        {
            if(ManageUI.Instances != null)
            {
                foreach (ManageUI manageUI in ManageUI.Instances)
                    manageUI.UnLoadEvents();
            }
            
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                EffectManager.askEffectClearByID(8100, steamPlayer.playerID.steamID);
                EffectManager.askEffectClearByID(8101, steamPlayer.playerID.steamID);
                EffectManager.askEffectClearByID(8102, steamPlayer.playerID.steamID);
                steamPlayer.player.serversideSetPluginModal(false);
            }
        }

        private void UnLoadEvents()
        {
            EffectManager.onEffectButtonClicked -= OnEffectButtonClick;
            EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
            EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
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
                ClickPlayer(callerPlayer);
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
            myItems.Clear();
        }

        private async void OnInventoryChange(byte page, byte index, ItemJar item)
        {
            
        }

        private void ClickPlayer(Player callerPlayer)//target player idnex in provider.clients
        {
            //UnturnedPlayer unturnedPlayerTarget = UnturnedPlayer.FromSteamPlayer(Provider.clients[index]);
            Items[] pages;
            try
            {
                pages = targetPlayer.inventory.items;// array (reference type in stack => by value)
            }
            catch (Exception)
            {
                Logger.LogError($"Internal error: Player not found: player has just left the server."); //make then a write to Inventory.dat (to do)
                return;
            }
            EffectManager.askEffectClearByID(8100, callerPlayer.channel.owner.playerID.steamID);
            EffectManager.sendUIEffect(8101, 23, callerPlayer.channel.owner.playerID.steamID, false);

            foreach (var page in pages)
            {
                if (page == null)
                    continue;
                foreach (ItemJar item in page.items)
                {
                    MyItem myItem = new MyItem(item.item.id, item.item.amount, item.item.quality, item.item.state);
                    if (ItemRestrictor.Instance.HasItem(myItem, myItems))
                        continue;
                    else
                        myItems.Add(myItem);
                }
            }
            PagesCountInv = (byte)Math.Ceiling((myItems.Count / 24.0));
            //EffectManager.sendUIEffectText(23, callerPlayer.channel.owner.playerID.steamID, false)
            // viewing myItems
            //Console.WriteLine("step 4");
            //foreach (var item in myItems)
            //{
            //    Console.WriteLine($"item: {item.ID}, {item.x}");
            //}
            //Console.WriteLine("step 5");
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
