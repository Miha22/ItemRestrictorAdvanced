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
    class Functions
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
        public static void OnInventoryAdded()
        {

        }
        public static void writeBlock(string path)
        {

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
                List<Item> playerItems = GetPlayerItems(directory.Name);
                List<MyItem> myItems = new List<MyItem>();
                foreach (var item in playerItems)
                {
                    myItems.Add(new MyItem(item.id, item.state[0], item.amount, item.quality));
                }
                //new JSONSerializer().serialize<List<MyItem>>(myItems, newPath, false);
                using (StreamWriter streamWriter = new StreamWriter(newPath))
                {
                    JsonWriter jsonWriter = (JsonWriter)new JsonTextWriterFormatted((TextWriter)streamWriter);
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonWriter, (object)myItems);
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

        private List<Item> GetPlayerItems(string str)//string steamID, string charID
        {
            List<Item> items = new List<Item>();
            Block block = ServerSavedata.readBlock("/Players/" + str + "/" + Provider.map + "/Player/Inventory.dat", 0);
            //PlayerSavedata.readBlock(steamPlayerID, "/Player/Inventory.dat", (byte)0);
            byte num1 = block.readByte();
            for (byte index1 = 0; (int)index1 < (int)PlayerInventory.PAGES - 2; ++index1)
            {
                //this.items[(int)index1].loadSize(block.readByte(), block.readByte());
                block.readByte();
                block.readByte();
                byte itemCount = block.readByte();
                for (byte index2 = 0; (int)index2 < (int)itemCount; ++index2)
                {
                    //byte x = block.readByte();
                    block.readByte();
                    //byte y = block.readByte();
                    block.readByte();
                    //byte rot = 0;
                    //if (num1 > (byte)4)
                    //    rot = block.readByte();
                    block.readByte();
                    ushort newID = block.readUInt16();
                    byte newAmount = block.readByte();
                    byte newQuality = block.readByte();
                    //byte[] newState = block.readByteArray();
                    block.readByteArray();
                    byte[] newState = new byte[1] { 1 };

                    Item item = new Item(newID, newAmount, newQuality, newState);
                    if (HasItem(item, items))
                        continue;
                    else
                        items.Add(item);
                    //this.items[(int)index1].loadItem(x, y, rot, new Item(num3, newAmount, newQuality, newState));
                }
            }
            return items;
        }
        private bool HasItem(Item item, List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == item.id && items[i].quality == item.quality && items[i].amount == item.amount)
                {
                    items[i].state[0]++;
                    return true;
                }
            }
            return false;
        }
        public static bool IsPlayersGroup(IRocketPlayer caller, Group group)
        {
            string[] groups = R.Permissions.GetGroups(caller, true).Select(g => g.Id).ToArray();
            for (ushort i = 0; i < groups.Length; i++)
            {
                if (group.GroupID.ToLower() == groups[i].ToLower())
                    return true;
            }

            return false;
        }
        
    }
}
