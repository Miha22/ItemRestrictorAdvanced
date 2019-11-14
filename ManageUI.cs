﻿using System;
using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using Rocket.Unturned.Chat;
using System.Linq;
using Rocket.API;

namespace ItemRestrictorAdvanced
{
    class ManageUI
    {
        internal static List<ManageUI> Instances;
        internal static List<Player> UICallers;
        internal static byte PagesCount { get; set; }
        //private static CancellationTokenSource cts;
        //private static CancellationToken token;
        internal byte PagesCountInv { get; set; }
        private byte playerIndex;
        private Rocket.API.IRocketPlayer caller;
        private byte itemIndex;
        private byte currentPage;
        private Player targetPlayer;
        private string targetName;
        private string playerSteamID;
        private readonly Player callerPlayer;
        private List<List<MyItem>> UIitemsPages;
        private ushort selectedId;
        private string id;
        private string count;
        private EOverload overload;

        static ManageUI()
        {
            Instances = new List<ManageUI>();
            UICallers = new List<Player>();
            //cts = new CancellationTokenSource();
            //token = cts.Token;
        }

        public ManageUI(byte pagesCount, Player caller, Rocket.API.IRocketPlayer callerP)
        {
            currentPage = 1;
            callerPlayer = caller;
            ManageUI.PagesCount = pagesCount;// !
            UIitemsPages = new List<List<MyItem>>();
            Instances.Add(this);
            overload = EOverload.None;
            selectedId = 0;
            this.caller = callerP;
        }

        internal static void UnLoad()
        {
            //cts.Cancel();
            if (ManageUI.Instances != null)
            {
                foreach (ManageUI manageUI in ManageUI.Instances)
                    manageUI.UnLoadEvents();
            }
            
            foreach (Player caller in UICallers)
            {
                EffectManager.askEffectClearByID(8100, caller.channel.owner.playerID.steamID);
                EffectManager.askEffectClearByID(8101, caller.channel.owner.playerID.steamID);
                EffectManager.askEffectClearByID(8102, caller.channel.owner.playerID.steamID);
                EffectManager.askEffectClearByID(8103, caller.channel.owner.playerID.steamID);
                caller.serversideSetPluginModal(false);
            }
        }

        private void UnLoadEvents()
        {
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick;
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick8101;
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick8102;
            EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick8103;
            EffectManager.onEffectTextCommitted -= this.OnTextCommited;
            EffectManager.onEffectTextCommitted -= this.OnTextCommited8103;
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
            EffectManager.sendUIEffect(8100, 22, true);
            byte multiplier = (byte)((CurrentPage - 1) * 24);
            for (byte i = multiplier; (i < (24 + multiplier)) && (i < (byte)Provider.clients.Count); i++)
                EffectManager.sendUIEffectText(22, CallerSteamID, true, $"text{i}", $"{Provider.clients[i].playerID.characterName}");
        }

        public void OnEffectButtonClick(Player callerPlayer, string buttonName)
        {
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"text[0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);
            
            if(buttonName.Substring(0, 4) == "text")
            {
                byte.TryParse(buttonName.Substring(4), out playerIndex);
                buttonName = "text";
            }
            switch (buttonName)
            {
                case "text":
                    if (Provider.clients.Count < ((playerIndex + 1) * PagesCount))
                        return;
                    playerIndex += (byte)((currentPage - 1) * 24);
                    currentPage = 1;
                    try
                    {
                        targetPlayer = Provider.clients[playerIndex].player;
                        playerSteamID = targetPlayer.channel.owner.playerID.steamID.ToString();
                        targetName = targetPlayer.channel.owner.playerID.characterName;
                    }
                    catch (Exception)
                    {
                        Logger.Log($"Player not found: player has just left the server. UI call from: {callerPlayer.channel.owner.playerID.characterName}");
                        return;
                    }
                    targetPlayer.inventory.onInventoryAdded += OnInventoryChange;
                    targetPlayer.inventory.onInventoryRemoved += OnInventoryChange;
                    EffectManager.askEffectClearByID(8100, callerPlayer.channel.owner.playerID.steamID);
                    GetTargetItems();
                    ShowItemsUI(callerPlayer, currentPage);

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
                    break;

                case "ButtonNext":
                    EffectManager.askEffectClearByID(8100, callerPlayer.channel.owner.playerID.steamID);
                    if (currentPage == PagesCount)
                    {
                        EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, true, "page", $"{PagesCount}");
                        SetCurrentPage(PagesCount, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                        SetPlayersList(PagesCount, callerPlayer.channel.owner.playerID.steamID);
                        currentPage = 1;
                    }
                    else
                    {
                        EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, true, "page", $"{currentPage}");
                        SetCurrentPage(currentPage, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                        SetPlayersList(currentPage, callerPlayer.channel.owner.playerID.steamID);
                        currentPage++;
                        //sends current page
                    }
                    break;
                case "ButtonPrev":
                    EffectManager.askEffectClearByID(8100, callerPlayer.channel.owner.playerID.steamID);
                    if (currentPage == 1)
                    {
                        EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, true, "page", $"1");
                        SetCurrentPage(1, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                        SetPlayersList(1, callerPlayer.channel.owner.playerID.steamID);
                        currentPage = PagesCount;
                        //sends 1st page
                    }
                    else
                    {
                        EffectManager.sendUIEffectText(22, callerPlayer.channel.owner.playerID.steamID, true, "page", $"{currentPage}");
                        SetCurrentPage(currentPage, callerPlayer.channel.owner.playerID.steamID.m_SteamID);
                        SetPlayersList(currentPage, callerPlayer.channel.owner.playerID.steamID);
                        currentPage--;
                        //sends current page
                    }
                    break;
                case "ButtonExit":
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick;
                    QuitUI(callerPlayer, 8100);
                    break;
                default://non button click
                    return;
            }
        }

        public void OnEffectButtonClick8101(Player callerPlayer, string buttonName)
        {
            if(buttonName.Substring(0, 4) == "item")
            {
                if (!caller.HasPermission("rocket.invsee.edit"))
                    return;
                byte.TryParse(buttonName.Substring(4), out itemIndex);
                //itemIndex += (byte)((currentPage - 1) * 24); 
                buttonName = "item";
                id = "";
                count = "";
            }
                
            switch (buttonName)
            {
                case "item":
                    //show 8102
                    //EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                    if (UIitemsPages[currentPage - 1].Count >= itemIndex + 1)
                        selectedId = UIitemsPages[currentPage - 1][itemIndex].ID;
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

                    EffectManager.onEffectButtonClicked -= this.OnEffectButtonClick8101;
                    EffectManager.onEffectButtonClicked += OnEffectButtonClick8102;
                    EffectManager.onEffectTextCommitted += OnTextCommited;
                    EffectManager.sendUIEffect(8102, 24, callerPlayer.channel.owner.playerID.steamID, true);
                    overload = EOverload.One;
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
                    overload = EOverload.None;
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
                    overload = EOverload.None;
                    if (targetPlayer != null)
                    {
                        targetPlayer.inventory.onInventoryAdded -= OnInventoryChange;
                        targetPlayer.inventory.onInventoryRemoved -= OnInventoryChange;
                    }
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8101;
                    QuitUI(callerPlayer, 8101);
                    break;
                default://non button click
                    break;
            }
        }

        private void RemoveItem(Player player, PlayerInventory inventory, ushort id)
        {
            if (targetPlayer != null)
            {
                targetPlayer.inventory.onInventoryAdded -= OnInventoryChange;
                targetPlayer.inventory.onInventoryRemoved -= OnInventoryChange;
            }
            //List<ItemsPair> PageIndexPair = new List<ItemsPair>();
            //ushort counter = 0;
            ////Console.WriteLine($"iteratuin started");
            //foreach (Items page in inventory.items)
            //{
            //    if (page == null)
            //        continue;
            //    foreach (ItemJar item in page.items)
            //    {
            //        if (item.item.id == id)
            //            counter++;
            //            //PageIndexPair.Add(new ItemsPair() { page = page, index = page.getIndex(item.x, item.y)});
            //    }
            //}
            //Console.WriteLine($"PageIndexPair.Count: {PageIndexPair.Count}");
            //foreach (var pair in PageIndexPair)
            //    pair.page.removeItem(pair.index);
            for (ushort i = 0; i < (ushort)inventory.items.Length; i++)
            {
                if (inventory.items[i] == null)
                    continue;
                if (inventory.items[i].items.Count == 0)
                    continue;
                ushort len2 = (ushort)inventory.items[i].items.Count;
                for (ushort j = 0; j < len2; j++)
                {
                    if(inventory.items[i].items[j].item.id == id)
                    {
                        inventory.items[i].removeItem(inventory.items[i].getIndex(inventory.items[i].items[j].x, inventory.items[i].items[j].y));
                        len2--;
                        j--;
                    }
                }
            }

            if (targetPlayer != null)
            {
                GetTargetItems();
                EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                ShowItemsUI(player, currentPage);
                targetPlayer.inventory.onInventoryAdded += OnInventoryChange;
                targetPlayer.inventory.onInventoryRemoved += OnInventoryChange;
            }
        }

        private void RemoveItem(Player player, PlayerInventory inventory, ushort id, ushort times)//as much as possible will delete
        {
            if (targetPlayer != null)
            {
                targetPlayer.inventory.onInventoryAdded -= OnInventoryChange;
                targetPlayer.inventory.onInventoryRemoved -= OnInventoryChange;
            }
            ushort itemsRemoved = 0;
            for (ushort i = 0; i < (ushort)inventory.items.Length && itemsRemoved < times; i++)
            {
                if (inventory.items[i] == null)
                    continue;
                if (inventory.items[i].items.Count == 0)
                    continue;
                ushort len2 = (ushort)inventory.items[i].items.Count;
                for (ushort j = 0; j < len2 && itemsRemoved < times; j++)
                {
                    if (inventory.items[i].items[j].item.id == id)
                    {
                        inventory.items[i].removeItem(inventory.items[i].getIndex(inventory.items[i].items[j].x, inventory.items[i].items[j].y));
                        len2--;
                        j--;
                        itemsRemoved++;
                    }
                }
            }
            if (targetPlayer != null)
            {
                targetPlayer.inventory.onInventoryAdded += OnInventoryChange;
                targetPlayer.inventory.onInventoryRemoved += OnInventoryChange;
                GetTargetItems();
                EffectManager.askEffectClearByID(8101, callerPlayer.channel.owner.playerID.steamID);
                ShowItemsUI(player, currentPage);
            }
        }

        public void OnEffectButtonClick8102(Player callerPlayer, string buttonName)
        {
            //Console.WriteLine($"button clicked: {buttonName}");
            if (!Directory.Exists(Plugin.Instance.pathTemp + $"\\{playerSteamID}"))
                Directory.CreateDirectory(Plugin.Instance.pathTemp + $"\\{playerSteamID}");
            if (buttonName == "SaveExit" && id != "" && count != "")
            {
                overload = EOverload.None;
                if (!ushort.TryParse(id, out ushort newID) || !short.TryParse(count, out short newCount))
                {
                    UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"Provided ID or amount is not a number", UnityEngine.Color.red);
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
                    EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
                    EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                    EffectManager.onEffectTextCommitted -= OnTextCommited;

                    return;
                }

                if (Assets.find(EAssetType.ITEM, newID) == null)
                {
                    UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"ID: {newID} cannot be found, check if your id exists", UnityEngine.Color.red);
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
                    EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
                    EffectManager.onEffectTextCommitted -= OnTextCommited;
                    EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);

                    return;
                }
                    
                ItemAsset item = (ItemAsset)Assets.find(EAssetType.ITEM, newID);
                Item newitem = new Item(item.id, item.amount, 100, item.getState());
                if (targetPlayer != null && newCount >= 0)
                {
                    for (short i = 0; i < newCount; i++)
                    {
                        if (!targetPlayer.inventory.tryAddItemAuto(newitem, false, false, false, false))
                        {
                            UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"{(targetPlayer != null ? targetPlayer.channel.owner.playerID.characterName + "'s" : "player's")} inventory is full, loading item: {item.name} to his virtual inventory");
                            Functions.WriteItem(newitem, Plugin.Instance.pathTemp + $"\\{playerSteamID}\\Heap.dat");
                        }
                        //else
                        //{
                        //    OnInventoryChange triggers => sends 8101 update
                        //}
                    }
                }
                else if (targetPlayer != null && newCount < 0)
                    RemoveItem(callerPlayer, targetPlayer.inventory, newID, (ushort)newCount);
                else
                {
                    UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"Player has just left the server, loading to player's virtual inventory..");
                    for (ushort i = 0; i < Convert.ToUInt16(count); i++)
                        Functions.WriteItem(newitem, Plugin.Instance.pathTemp + $"\\{playerSteamID}\\Heap.dat");// if player is offline load to virtual heap
                }
                EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
                EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
                EffectManager.onEffectTextCommitted -= OnTextCommited;
                EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);

                return;
            }
            switch (buttonName)
            {
                case "ButtonRemove":
                    if (selectedId == 0)
                        break;
                    if (targetPlayer != null)
                    {
                        RemoveItem(callerPlayer, targetPlayer.inventory, selectedId);
                        UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"All ID: {selectedId} items were removed from {targetPlayer.channel.owner.playerID.characterName}");
                    }
                    else
                        UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"Cannot remove items from player, because player left the server");
                    //EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                    //EffectManager.sendUIEffect(8102, 24, callerPlayer.channel.owner.playerID.steamID, true);
                    break;
                case "ButtonAdd":
                    if (selectedId == 0)
                        break;
                    ItemAsset item = (ItemAsset)Assets.find(EAssetType.ITEM, selectedId);
                    Item newitem = new Item(item.id, item.amount, 100, item.getState());
                    if (targetPlayer != null)
                    {
                        if (!targetPlayer.inventory.tryAddItemAuto(newitem, false, false, false, false))
                        {
                            UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"{(targetPlayer != null ? targetPlayer.channel.owner.playerID.characterName + "'s" : "player's")} inventory is full, loading item: {item.name} to his virtual inventory");
                            Functions.WriteItem(newitem, Plugin.Instance.pathTemp + $"\\{playerSteamID}\\Heap.dat");
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"Player has just left the server, loading to player's virtual inventory..");
                        for (ushort i = 0; i < Convert.ToUInt16(count); i++)
                            Functions.WriteItem(newitem, Plugin.Instance.pathTemp + $"\\{playerSteamID}\\Heap.dat");// if player is offline load to virtual heap
                    }
                    //EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                    //EffectManager.sendUIEffect(8102, 24, callerPlayer.channel.owner.playerID.steamID, true);
                    break;
                case "ButtonDel":
                    if (selectedId == 0)
                        break;
                    if (targetPlayer != null)
                    {
                        RemoveItem(callerPlayer, targetPlayer.inventory, selectedId, 1);
                        //UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"ID: {selectedId} item was removed from {targetPlayer.channel.owner.playerID.characterName}");
                    }
                    else
                        UnturnedChat.Say(callerPlayer.channel.owner.playerID.steamID, $"Cannot remove item from player, because player left the server");
                    EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                    EffectManager.sendUIEffect(8102, 24, callerPlayer.channel.owner.playerID.steamID, true);
                    break;
                case "MainPage":
                    overload = EOverload.None;
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
                    EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
                    EffectManager.onEffectTextCommitted -= OnTextCommited;
                    EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                    break;
                case "ButtonHelp":
                    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
                    EffectManager.onEffectButtonClicked += OnEffectButtonClick8103;
                    EffectManager.onEffectTextCommitted += OnTextCommited8103;
                    EffectManager.sendUIEffect(8103, 27, callerPlayer.channel.owner.playerID.steamID, true);
                    overload = EOverload.Two;
                    break;
                //case "ButtonOk":
                //    EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
                //    EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
                //    EffectManager.onEffectTextCommitted -= OnTextCommited;
                //    EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                //    break;
                default://non button click
                    //EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
                    //EffectManager.sendUIEffect(8102, 24, callerPlayer.channel.owner.playerID.steamID, false);
                    break;
            }
            //Console.WriteLine("IF AND ELSE PASSED");
            //EffectManager.onEffectButtonClicked -= OnEffectButtonClick8102;
            //EffectManager.onEffectButtonClicked += OnEffectButtonClick8101;
            //EffectManager.askEffectClearByID(8102, callerPlayer.channel.owner.playerID.steamID);
            //EffectManager.onEffectTextCommitted -= OnTextCommited;
        }

        private void OnTextCommited(Player player, string field, string text)
        {
            if (field == "ID")
                id = text;
            else
                count = text;
        }

        private void OnTextCommited8103(Player player, string field, string text)
        {
            //Console.WriteLine($"field: {field}");
            if (field == "Input")
                SearcherIdFinder(player.channel.owner.playerID.steamID, text);
        }

        private void SearcherIdFinder(Steamworks.CSteamID steamID, string itemString)
        {
            //EffectManager.askEffectClearByID(8103, callerPlayer.channel.owner.playerID.steamID);
            ItemAsset itemAsset = new List<ItemAsset>(Assets.find(EAssetType.ITEM).Cast<ItemAsset>()).Where<ItemAsset>((Func<ItemAsset, bool>)(i => i.itemName != null)).OrderBy<ItemAsset, int>((Func<ItemAsset, int>)(i => i.itemName.Length)).Where<ItemAsset>((Func<ItemAsset, bool>)(i => i.itemName.ToLower().Contains(itemString.ToLower()))).FirstOrDefault<ItemAsset>();
            if (itemAsset == null)
            {
                EffectManager.sendUIEffect(8103, 27, callerPlayer.channel.owner.playerID.steamID, true);
                EffectManager.sendUIEffectText(27, steamID, true, "Output", "Failed to find ID");
            }
            else
            {
                EffectManager.sendUIEffect(8103, 27, callerPlayer.channel.owner.playerID.steamID, true);
                EffectManager.sendUIEffectText(27, steamID, true, "Output", $"{itemAsset.id}");
                EffectManager.sendUIEffectText(27, steamID, true, "OutputName", $"{itemAsset.itemName}");
            }
        }

        private void OnEffectButtonClick8103(Player callerPlayer, string button)
        {
            if(button == "ButtonOk")
            {
                overload = EOverload.One;
                EffectManager.onEffectButtonClicked -= OnEffectButtonClick8103;
                EffectManager.onEffectButtonClicked += OnEffectButtonClick8102;
                EffectManager.askEffectClearByID(8103, callerPlayer.channel.owner.playerID.steamID);
            }
        }

        private void OnInventoryChange(byte page, byte index, ItemJar item)//if player exits this automatically removed
        {
            //if (token.IsCancellationRequested)
            //    return;
            //EffectManager.askEffectClearByID(8101, this.callerPlayer.channel.owner.playerID.steamID);
            GetTargetItems();
            //await System.Threading.Tasks.Task.Run(()=> ShowItemsUI(this.callerPlayer, currentPage));
            ShowItemsUI(this.callerPlayer, currentPage);
            //ShowItemsUI(this.callerPlayer, currentPage);
        }

        private void ShowItemsUI(Player callPlayer, byte page)//target player idnex in provider.clients
        {
            Console.WriteLine();
            //foreach (var pag in UIitemsPages)
            //{
            //    Console.WriteLine($"page count in ShowUI: {pag.Count}");
            //}
            Console.WriteLine();
            try
            {
                EffectManager.sendUIEffect(8101, 23, callPlayer.channel.owner.playerID.steamID, true);
                if(UIitemsPages[page - 1].Count != 0)
                    for (byte i = 0; i < UIitemsPages[page - 1].Count; i++)
                        EffectManager.sendUIEffectText(23, callPlayer.channel.owner.playerID.steamID, true, $"item{i}", $"{((ItemAsset)Assets.find(EAssetType.ITEM, UIitemsPages[page - 1][i].ID)).itemName}\r\nID: {UIitemsPages[page - 1][i].ID}\r\nCount: {UIitemsPages[page - 1][i].Count}");
                EffectManager.sendUIEffectText(23, callPlayer.channel.owner.playerID.steamID, true, "page", $"{page}");
                EffectManager.sendUIEffectText(23, callPlayer.channel.owner.playerID.steamID, true, "pagemax", $"{PagesCountInv}");
                EffectManager.sendUIEffectText(23, callPlayer.channel.owner.playerID.steamID, true, "playerName", $"{targetName}");
                if (overload == EOverload.One)
                {
                    EffectManager.askEffectClearByID(8102, callPlayer.channel.owner.playerID.steamID);
                    EffectManager.sendUIEffect(8102, 24, callPlayer.channel.owner.playerID.steamID, true);
                }
                    
                else if (overload == EOverload.Two)
                {
                    EffectManager.askEffectClearByID(8102, callPlayer.channel.owner.playerID.steamID);
                    EffectManager.askEffectClearByID(8103, callPlayer.channel.owner.playerID.steamID);
                    EffectManager.sendUIEffect(8102, 24, callPlayer.channel.owner.playerID.steamID, true);
                    EffectManager.sendUIEffect(8103, 27, callPlayer.channel.owner.playerID.steamID, true);
                }
                    
            }
            catch (Exception e)
            {
                Console.WriteLine("exception in show ui");
                Console.WriteLine(e);
                QuitUI(callPlayer, 8101);
                return;
            }
        }

        private void GetTargetItems()
        {
            UIitemsPages.Clear();
            Items[] pages;
            try
            {
                pages = targetPlayer.inventory.items;// array (reference type in stack => by value)
            }
            catch (Exception e)
            {
                // load to Inventory.dat and maybe to virtual inventory
                Console.WriteLine("exception in target items");
                Console.WriteLine(e);
                QuitUI(callerPlayer, 8101);
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
                    if (Plugin.Instance.HasItem(myItem, items))
                        continue;
                    else
                        items.Add(myItem);
                }
            }

            if (items.Count == 0)
            {
                Console.WriteLine("items.count in get gettarget is 0");
                PagesCountInv = 1;
                UIitemsPages.Add(items);
                return;
            }
                
            ushort counter = 0;
            for (byte i = 0; i < (byte)Math.Ceiling(items.Count / 24.0); i++)
            {
                List<MyItem> myPage = new List<MyItem>();
                for (ushort j = 0; j < 24 && counter < (ushort)items.Count; j++, counter++)
                    myPage.Add(new MyItem(items[counter].ID, items[counter].X, items[counter].Quality, items[counter].State, items[counter].Count));
                UIitemsPages.Add(myPage);
            }
            PagesCountInv = (byte)UIitemsPages.Count;
            //Console.WriteLine("in get target items");
            //Console.WriteLine($"UIitemsPages.Count: {UIitemsPages.Count}");
            //Console.WriteLine($"PagesCountInv: {PagesCountInv}");
        }

        private void QuitUI(Player callerPlayer, ushort effectId)
        {
            EffectManager.askEffectClearByID(effectId, callerPlayer.channel.owner.playerID.steamID);
            callerPlayer.serversideSetPluginModal(false);
            ManageUI.UICallers.Remove(callerPlayer);
            //Console.WriteLine($"caller: {callerPlayer.channel.owner.playerID.characterName} removed from list!");
            UIitemsPages.Clear();
        }
    }
}