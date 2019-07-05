using System.IO;
using System.Linq;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Core;
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
        //private static byte ItemsCountOnPage(byte page, List<MyItem> myItems)
        //{
        //    byte counter = 0;
        //    foreach (var item in myItems)
        //    {
        //        if (item.Page == page)
        //            counter++;
        //    }

        //    return counter;
        //}

        //internal static void writeBlock(string writepath, string readpath)
        //{
        //    //MyItem[] myItems;
        //    List<MyItem> myItems;
        //    using (StreamReader streamReader = new StreamReader(readpath))//SDG.Framework.IO.Deserialization
        //    {
        //        JsonReader reader = (JsonReader)new JsonTextReader((TextReader)streamReader);
        //        //JsonSerializer jsonSerializer = new JsonSerializer();
        //        myItems = new JsonSerializer().Deserialize<List<MyItem>>(reader);
        //    }
        //    Block block = new Block();
        //    block.writeByte(PlayerInventory.SAVEDATA_VERSION);
        //    for (byte index1 = 0; index1 < PlayerInventory.PAGES - 2; ++index1)
        //    {
        //        byte num1;
        //        byte num2;
        //        byte num3;
        //        num1 = myItems[index1].Width;
        //        num2 = myItems[index1].Height;
        //        //num3 = (byte)myItems[index1].items.Count; //items on page
        //        num3 = ItemsCountOnPage(index1, myItems);
        //        block.writeByte(num1);
        //        block.writeByte(num2);
        //        block.writeByte(num3);
        //        for (byte index2 = 0; index2 < num3; ++index2)
        //        {
        //            //ItemJar itemJar = myItems[index1].items[index2];
        //            ItemJar itemJar = new ItemJar();
        //            block.writeByte(itemJar == null ? (byte)0 : itemJar.x);
        //            block.writeByte(itemJar == null ? (byte)0 : itemJar.y);
        //            block.writeByte(itemJar == null ? (byte)0 : itemJar.rot);
        //            block.writeUInt16(itemJar == null ? (ushort)0 : itemJar.item.id);
        //            block.writeByte(itemJar == null ? (byte)0 : itemJar.item.amount);
        //            block.writeByte(itemJar == null ? (byte)0 : itemJar.item.quality);
        //            block.writeByteArray(itemJar == null ? new byte[0] : itemJar.item.state);
        //        }
        //    }
        //    //PlayerSavedata.writeBlock(this.channel.owner.playerID, "/Player/Inventory.dat", block);
        //    ServerSavedata.writeBlock(writepath, block);
        //}
        private void GetSize(string path, out byte width, out byte height)
        {
            Block block = ServerSavedata.readBlock(path, 0);
            block.readByte();
            width = block.readByte();
            height = block.readByte();
        }
        public bool TryAddItems(string writepath, string readpath, out List<MyItem> notAddedItems)
        {
            Block block = new Block();
            block.writeByte(PlayerInventory.SAVEDATA_VERSION);
            List<MyItem> myItems;
            using (StreamReader streamReader = new StreamReader(readpath))//SDG.Framework.IO.Deserialization
            {
                JsonReader reader = (JsonReader)new JsonTextReader((TextReader)streamReader);
                myItems = new JsonSerializer().Deserialize<List<MyItem>>(reader);
            }
            for (byte i = 0; i < PlayerInventory.PAGES - 1; i++)
            {
                byte width, height, itemsCount;
                GetSize(writepath, out width, out height);
                List<MyItem> selectedItems = SelectItems(width, height, myItems);
            }
        }
        private List<MyItem> OrderByLarge(List<MyItem> myItems)
        {
            List<MyItem> selectedItems = new List<MyItem>();
            while(myItems.Count != 0)
            {
                MyItem largest = myItems[0];
                for (byte i = 1; i < myItems.Count; i++)
                {
                    if ((largest.Size_x * largest.Size_y) <= (myItems[i].Size_x * myItems[i].Size_y))
                    {
                        largest = myItems[i];
                        myItems.RemoveAt(i);
                    }   
                }
                selectedItems.Add(largest);
            }

            return selectedItems;
        }
        private List<MyItem> SelectItems(byte width, byte height, List<MyItem> myItems)
        {
            List<MyItem> selectedItems = new List<MyItem>();
            bool[,] page = FillPage(width, height);


            return selectedItems;
        }
        private bool[,] FillPage(byte width, byte height)
        {
            bool[,] page = new bool[width, height];
            for (byte i = 0; i < width; i++)
            {
                for (byte j = 0; j < height; j++)
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
        private (string, string) GetSteamID(string line)
        {
            string[] str = line.Split('\\');
            string steamId = str[str.Length - 1];
            string map = str[str.Length - 2];

            if (!int.TryParse(steamId, out int steamId32))
                throw new System.InvalidCastException($"Failed to get player SteamID at ItemRestrictorAdvanced.Watcher.GetSteamID(string line), output: {steamId}");

            return (steamId, map);
        }
        private bool IsPlayerOnline(string steamID)
        {
            foreach (var steamPlayer in SDG.Unturned.Provider.clients)
            {
                if (steamID == steamPlayer.playerID.ToString())
                    return true;
            }

            return false;
        }
        private string PlayerInPlayersFolder(string steamId)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo("../Players").GetDirectories())
            {
                if (directory.Name.Split('\\')[0] == steamId)
                    return directory.Name;
            }
            throw new System.IO.DirectoryNotFoundException($@"Failed to find: {steamId} in ../{Provider.serverName}/Players  folder!");
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
