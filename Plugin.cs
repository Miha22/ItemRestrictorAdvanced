using System;
using Rocket.Core.Plugins;
using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using System.Threading.Tasks;
using Rocket.Core.Commands;
using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Newtonsoft.Json;
using SDG.Framework.IO.Serialization;

namespace ItemRestrictorAdvanced
{
    class ItemRestrictor : RocketPlugin<PluginConfiguration>
    {
        static string path = $@"Plugins\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}\Inventories\{SDG.Unturned.Provider.map}";
        internal static ItemRestrictor Instance;
        public static System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
        System.Threading.CancellationToken token = cts.Token;

        public ItemRestrictor()
        {
            Provider.onServerShutdown += OnServerShutdown;
        }

        protected override void Load()
        {
            if (Configuration.Instance.Enabled)
            {
                Instance = this;
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory.Attributes == FileAttributes.ReadOnly)
                    directory.Attributes &= ~FileAttributes.ReadOnly;

                try
                {
                    LoadInventoryTo(path);
                    WatcherAsync(token);
                    Logger.Log("ItemRestrictorAdvanced by M22 loaded!", ConsoleColor.Yellow);
                }
                catch (Exception e)
                {
                    Logger.LogException(e, $"EXCEPTION MESSAGE: {e.Message} \n EXCEPTION TargetSite: {e.TargetSite} \n EXCEPTION StackTrace {e.StackTrace}");
                    cts.Cancel();
                    Console.WriteLine();
                }
                //create json files for each player from inventory.dat..
            }
            else
            {
                cts.Cancel();
                Logger.Log("Plugin is turned off in Configuration, unloading...", ConsoleColor.Cyan);
                UnloadPlugin();
            }
        }
        [RocketCommand("inventory", "", "", AllowedCaller.Both)]
        [RocketCommandAlias("inv")]
        public void Execute(IRocketPlayer caller, string[] command)
        {
            foreach (var steamPlayer in Provider.clients)
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                for (byte i = 0; i < 8; i++)
                {
                    for (byte j = 0; j < player.Inventory.getItemCount(i); j++)
                    {
                        ItemJar item = player.Inventory.getItem(i, j);
                        Console.WriteLine($"id: {item.item.id}, x:{item.x}, y:{item.y}  size x: {item.size_x}, size y: {item.size_y}, rot: {item.rot}");
                    }
                }
            }
        }
        static async void WatcherAsync(System.Threading.CancellationToken token)
        {
            //Console.WriteLine("Начало метода FactorialAsync"); // выполняется синхронно
            if (token.IsCancellationRequested)
                return;
            await Task.Run(()=>new Watcher().Run(path, token));                            // выполняется асинхронно
            //Console.WriteLine("Конец метода FactorialAsync");  // выполняется синхронно
        }
        public static void OnServerShutdown()
        {
            cts.Cancel();
            Provider.onServerShutdown -= OnServerShutdown;
        }

        //[RocketCommand("ss", "", "", AllowedCaller.Console)]
        //[RocketCommandAlias("ss")]

        //public void CheckTotalVehicles()
        //{
        //    Dictionary<SteamPlayer, ushort> carOwners = new Dictionary<SteamPlayer, ushort>();
        //    foreach (var steamPlayer in Provider.clients)
        //    {
        //    }
        //    foreach (var carOwner in carOwners)
        //    {
        //        Console.WriteLine($"Owner: {carOwner.Key}, cars: {carOwner.Value}");
        //    }

        //}
        //private ushort VehiclesCounter(SteamPlayer steamPlayer)
        //{
        //    ushort counter = 0;
        //    foreach (var veh in VehicleManager.vehicles)
        //    {
        //        if (veh.isLocked && veh.lockedOwner == steamPlayer.playerID.steamID)
        //            counter++;
        //    }
        //    return counter;
        //}
        internal static void OnInventoryAdded()
        {

        }
        private (byte, byte) GetPageSize(List<MyItem> myItems, byte pageIndex)
        {
            foreach (var item in myItems)
            {
                if (item.Index == pageIndex)
                    return (item.Width, item.Height);
            }

            return (0, 0);
        }
        public bool TryAddItems(string writepath, string readpath)
        {
            Console.WriteLine("point -3");
            Block block = new Block();
            Console.WriteLine("point -2");
            block.writeByte(PlayerInventory.SAVEDATA_VERSION);
            Console.WriteLine("point -1");
            List<MyItem> myItems = new List<MyItem>();
            Console.WriteLine("point 0");
            using (StreamReader streamReader = new StreamReader(readpath))//SDG.Framework.IO.Deserialization
            {
                JsonReader reader = (JsonReader)new JsonTextReader((TextReader)streamReader);
                myItems = new JsonSerializer().Deserialize<List<MyItem>>(reader);
            }
            Console.WriteLine("point 1");
            if (myItems == null)
            {
                Console.WriteLine("Items is null");
                return false;
            }
            else
            {
                Console.WriteLine($"items count: {myItems.Count}");
            }

            //foreach (var myItem in myItems)
            //{
            //    Console.WriteLine("point 1.1");
            //    ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, myItem.ID);
            //    Console.WriteLine("point 1.2");
            //    myItem.Size_x = itemAsset.size_x;
            //    myItem.Size_y = itemAsset.size_y;
            //    myItem.Rot = 0;
            //    Console.WriteLine("point 1.3");
            //    if (myItem.Count > 1)
            //    {

            //        for (byte i = 0; i < myItem.Count - 1; i++)
            //        {
            //            myItems.Add(myItem);
            //        }
            //    }
            //    Console.WriteLine("point 1.4");
            //}
            int len = myItems.Count;
            for (byte i = 0; i < (byte)len; i++)
            {
                Console.WriteLine("point 1.1");
                ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, myItems[i].ID);
                Console.WriteLine("point 1.2");
                myItems[i].Size_x = itemAsset.size_x;
                myItems[i].Size_y = itemAsset.size_y;
                myItems[i].Rot = 0;
                Console.WriteLine("point 1.3");
                if (myItems[i].Count > 1)
                {
                    for (byte j = 0; j < myItems[i].Count-1; j++)
                    {
                        myItems.Add(myItems[i]);
                    }
                }
                Console.WriteLine("point 1.4");
            }
            Console.WriteLine("point 2");
            myItems.Sort(new MyItemComparer());
            
            for (byte i = 0; i < PlayerInventory.PAGES - 1; i++)
            {
                Console.WriteLine("for (byte i = 0; i < PlayerInventory.PAGES - 1; i++)");
                byte width, height, itemsCount;
                Console.WriteLine("byte width, height, itemsCount;");
                (width, height) = GetPageSize(myItems, i);
                Console.WriteLine(" width = myItems[0].Pages[i].width/height");
                Console.WriteLine($"page#: {i}  wid: {width}, hei: {height}");
                Console.WriteLine("page#: {i}  wid: {width}, hei: {height}");
                (List<MyItem> selectedItems, List<MyItem> unSelectedItems) = SelectItems(width, height, myItems);
                Console.WriteLine("(List<MyItem> selectedItems, List<MyItem> unSelectedItems) = SelectItems(width, height, myItems);");
                itemsCount = (byte)selectedItems.Count;
                Console.WriteLine("itemsCount = (byte)selectedItems.Count");
                block.writeByte(width);
                block.writeByte(height);
                block.writeByte(itemsCount);
                Console.WriteLine("block.writeByte(itemsCount);");
                for (byte j = 0; j < itemsCount; j++)
                {
                    ItemJar itemJar = new ItemJar(selectedItems[j].X, selectedItems[j].Y, selectedItems[j].Rot, new Item(selectedItems[j].ID, selectedItems[j].x, selectedItems[j].Quality));
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.x);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.y);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.rot);
                    block.writeUInt16(itemJar == null ? (ushort)0 : itemJar.item.id);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.item.amount);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.item.quality);
                    block.writeByteArray(itemJar == null ? new byte[0] : itemJar.item.state);
                }
                Console.WriteLine("point 5");
            }
            ServerSavedata.writeBlock(writepath, block);
            Console.WriteLine("point 6");
            return true;
        }
        private (List<MyItem>, List<MyItem>) SelectItems(byte width, byte height, List<MyItem> myItems)// for page
        {
            List<MyItem> selectedItems = new List<MyItem>();
            List<MyItem> unSelectedItems = new List<MyItem>();
            bool[,] page = FillPage(width, height);
            foreach (var item in myItems)
            {
                byte x, y;
                if (FindPlace(ref page, width, item.Size_x, item.Size_y, out x, out y))
                {
                    item.X = x;
                    item.Y = y;
                    selectedItems.Add(item);
                }
                else
                {
                    item.Rot = 1;
                    if (FindPlace(ref page, width, item.Size_x, item.Size_y, out x, out y))
                    {
                        item.X = x;
                        item.Y = y;
                        selectedItems.Add(item);
                    }
                    else
                        unSelectedItems.Add(item);
                }

            }

            return (selectedItems, unSelectedItems);
        }
        private bool FindPlace(ref bool[,] page, byte pageWidth, byte reqWidth, byte reqHeight, out byte x, out byte y)//request > 1
        {
            for (byte i = 0; i < page.Length; i++)
            {
                for (byte j = 0; j < pageWidth; j++)
                {
                    if (FindTrues(ref page, pageWidth, i, j, reqWidth, reqHeight, out byte temp_x, out byte temp_y))
                    {
                        x = temp_x;
                        y = temp_y;
                        FillPageCells(ref page, i, j, reqWidth, reqHeight);

                        return true;
                    }
                }
            }
            x = 0;
            y = 0;

            return false;
        }
        private void FillPageCells(ref bool[,] page, byte row, byte startIndex, byte timesSide, byte timesBelow)
        {
            for (byte i = 0; i < timesBelow; i++)
            {
                byte index = startIndex;
                for (byte j = startIndex; j < timesSide; j++, index++)
                {
                    page[row + i, index] = true;
                }
            }
        }
        private bool FindTrues(ref bool[,] page, byte pageWidth, byte row, byte startIndex, byte timesSide, byte timesBelow, out byte temp_x, out byte temp_y)
        {
            if (pageWidth < (startIndex + timesSide) || page.Length < (row + timesBelow))
            {
                temp_x = 0;
                temp_y = 0;
                return false;
            }

            for (byte i = 0; i < timesBelow; i++)
            {
                byte index = startIndex;
                for (byte j = 0; j < timesSide; j++, index++)
                {
                    if (page[row + i, index] == false)
                    {
                        temp_x = 0;
                        temp_y = 0;
                        return false;
                    }
                }
            }

            temp_y = row;
            temp_x = startIndex;
            return true;
        }
        private bool[,] FillPage(byte width, byte height)
        {
            bool[,] page = new bool[width, height];
            for (byte i = 0; i < height; i++)
            {
                for (byte j = 0; j < width; j++)
                {
                    page[i, j] = true;
                }
            }

            return page;
        }
        private List<MyItem> GetPlayerItems(string steamIdstr)//look up a call of GetPlayerItems for "str" for more info
        {
            List<MyItem> myItems = new List<MyItem>();
            Block block = ServerSavedata.readBlock("/Players/" + steamIdstr + "/" + Provider.map + "/Player/Inventory.dat", 0);
            if (block == null)
                System.Console.WriteLine("Player has no items");
            else
                System.Console.WriteLine("Player has items");
            byte num1 = block.readByte();//BUFFER_SIZE
            for (byte index1 = 0; index1 < PlayerInventory.PAGES - 1; ++index1)
            {
                byte width = block.readByte();
                byte height = block.readByte();
                //block.readByte();
                //block.readByte();
                //System.Console.WriteLine($"page: {index1} wid: {width}, hei: {height}");
                byte itemCount = block.readByte();
                //MyItem.Pages.Clear();
                //MyItem.Pages.Add((width, height, itemCount));
                for (byte index = 0; index < itemCount; index++)
                {
                    byte x = block.readByte();
                    byte y = block.readByte();
                    byte rot = 0;
                    if (num1 > 4)
                        rot = block.readByte();
                    else
                        block.readByte();

                    ushort newID = block.readUInt16();
                    byte newAmount = block.readByte();
                    byte newQuality = block.readByte();
                    byte[] newState = block.readByteArray();
                    MyItem myItem = new MyItem(newID, newAmount, newQuality, newState, rot, x, y, index, width, height);
                    if (HasItem(myItem, myItems))
                        continue;
                    else
                        myItems.Add(myItem);
                }
            }
            return myItems;
        }
        public void LoadInventoryTo(string path)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo("../Players").GetDirectories())
            {
                //string path = $@"..\Players\{directory.Name}\{Provider.map}\Player\Inventory.json";
                //string path = $@"Plugins\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}\Inventories\{directory.Name.Split('_')[0]}.json";
                //string newPath = path + $@"\{directory.Name.Split('_')[0]}_{PlayerName(directory.Name.Split('_')[0])}.json";
                string newPath = path + $@"\{directory.Name.Split('_')[0]}.json";
                //if (!File.Exists(newPath))
                //    File.Create(newPath);
                FileInfo file = new FileInfo(newPath);
                if (file.Attributes == FileAttributes.ReadOnly)
                    file.Attributes &= ~FileAttributes.ReadOnly;
                List<MyItem> myItems = GetPlayerItems(directory.Name);
                //new JSONSerializer().serialize<List<MyItem>>(myItems, newPath, false);
                using (StreamWriter streamWriter = new StreamWriter(newPath))//SDG.Framework.IO.Serialization
                {
                    JsonWriter jsonWriter = (JsonWriter)new JsonTextWriterFormatted((TextWriter)streamWriter);
                    new JsonSerializer().Serialize(jsonWriter, (object)myItems);
                    jsonWriter.Flush();
                }
                //DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<MyItem>));
                //using (FileStream fs = new FileStream(newPath, FileMode.OpenOrCreate))
                //{
                //    jsonFormatter.WriteObject(fs, myItems);
                //}

                //using (FileStream fs = new FileStream("people.json", FileMode.OpenOrCreate))
                //{
                //    Person[] newpeople = (Person[])jsonFormatter.ReadObject(fs);

                //    foreach (Person p in newpeople)
                //    {
                //        Console.WriteLine("Имя: {0} --- Возраст: {1}", p.Name, p.Age);
                //    }
                //}

                //try
                //{
                //    string path = $@"..\Players\{directory.Name}\{Provider.map}\Player\Inventory.txt";
                //    string path2 = $@"..\Players\{directory.Name}\{Provider.map}\Inventory.txt";
                //    if (!File.Exists(path))
                //        File.Create(path);
                //    if (directory.Attributes == FileAttributes.ReadOnly)
                //        directory.Attributes &= ~FileAttributes.ReadOnly;
                //    using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                //    {
                //        //string[] playerID = directory.Name.Split('_');
                //        List<Item> playerItems = GetPlayerItems(directory.Name);
                //        foreach (Item item in playerItems)
                //        {
                //            //sw.WriteLine($"ID: {item.id}\n Amount: {item.amount}\n Quality: {item.quality}");
                //            sw.WriteLine($"ID: {item.id}");
                //            sw.WriteLine($"Amount: {item.state[0]}");
                //            sw.WriteLine($"x{item.amount}");
                //            sw.WriteLine($"Quality: {item.quality}");
                //            sw.WriteLine("-------");
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    Logger.LogError($"{e.Message}\n{e.TargetSite}");
                //}
            }
        }
        private bool HasItem(MyItem item, List<MyItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == item.ID && items[i].Quality == item.Quality && items[i].x == item.x)
                {
                    items[i].Count++;
                    return true;
                }
            }
            return false;
        }
    }
}
