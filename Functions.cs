using System.IO;
using System.Collections.Generic;
using SDG.Unturned;
using SDG.Framework.IO.Serialization;
using Newtonsoft.Json;

namespace ItemRestrictorAdvanced
{
    internal class Functions
    {
        //private string PlayerName(string playerID)
        //{
        //    foreach (var steamplayer in Provider.clients)
        //    {
        //        if (playerID == steamplayer.playerID.ToString())
        //            return steamplayer.playerID.playerName;
        //    }

        //    return "";
        //}
        internal static void OnInventoryAdded()
        {

        }
        private void GetSize(string path, out byte width, out byte height)
        {
            Block block = ServerSavedata.readBlock(path, 0);
            block.readByte();
            width = block.readByte();
            height = block.readByte();
        }
        public bool TryAddItems(string writepath, string readpath)
        {
            Block block = new Block();
            block.writeByte(PlayerInventory.SAVEDATA_VERSION);
            List<MyItem> myItems;
            using (StreamReader streamReader = new StreamReader(readpath))//SDG.Framework.IO.Deserialization
            {
                JsonReader reader = (JsonReader)new JsonTextReader((TextReader)streamReader);
                myItems = new JsonSerializer().Deserialize<List<MyItem>>(reader);
            }
            if (myItems == null)
                return false;
            foreach (var myitem in myItems)
            {
                ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, myitem.ID);
                myitem.Size_x = itemAsset.size_x;
                myitem.Size_y = itemAsset.size_y;
                myitem.Rot = 0;
            }
            myItems.Sort(new MyItemComparer());
            for (byte i = 0; i < PlayerInventory.PAGES - 1; i++)
            {
                byte width, height, itemsCount;
                GetSize(writepath, out width, out height);
                (List<MyItem> selectedItems, List<MyItem> unSelectedItems) = SelectItems(width, height, myItems);
                itemsCount = (byte)selectedItems.Count;
                block.writeByte(width);
                block.writeByte(height);
                block.writeByte(itemsCount);
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
            }
            ServerSavedata.writeBlock(writepath, block);

            return true;
        }
        private (List<MyItem>, List<MyItem>) SelectItems(byte width, byte height, List<MyItem> myItems)// for page
        {
            List<MyItem> selectedItems = new List<MyItem>();
            List<MyItem> unSelectedItems = new List<MyItem>();
            List<Area> areas = new List<Area>();
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
        private bool FindPlace(ref bool [,] page, byte pageWidth, byte reqWidth, byte reqHeight, out byte x, out byte y)//request > 1
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
            if(block == null)
                System.Console.WriteLine("Player has no items");
            else
                System.Console.WriteLine("Player has items");
            byte num1 = block.readByte();//BUFFER_SIZE
            for (byte index1 = 0; index1 < PlayerInventory.PAGES - 2; ++index1)
            {
                //byte width = block.readByte();
                //byte height = block.readByte();
                block.readByte();
                block.readByte();
                //System.Console.WriteLine($"page: {index1} wid: {width}, hei: {height}");
                byte itemCount = block.readByte();
                //MyItem.Pages.Clear();
                //MyItem.Pages.Add((width, height, itemCount));
                for (byte index2 = 0; index2 < itemCount; ++index2)
                {
                    byte x = block.readByte();
                    byte y = block.readByte();
                    byte rot = 0;
                    if (num1 > 4)
                        rot = block.readByte();
                    ushort newID = block.readUInt16();
                    byte newAmount = block.readByte();
                    byte newQuality = block.readByte();
                    byte[] newState = block.readByteArray();
                    MyItem myItem = new MyItem(newID, newAmount, newQuality, newState, rot, x, y);
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
                if(file.Attributes == FileAttributes.ReadOnly)
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
