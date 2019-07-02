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
        internal static void writeBlock(string writepath, string readpath)
        {
            MyItem[] myItems;
            using (StreamReader streamReader = new StreamReader(readpath))//SDG.Framework.IO.Deserialization
            {
                JsonReader reader = (JsonReader)new JsonTextReader((TextReader)streamReader);
                //JsonSerializer jsonSerializer = new JsonSerializer();
                myItems = new JsonSerializer().Deserialize<MyItem[]>(reader);
            }
            Block block = new Block();
            block.writeByte(PlayerInventory.SAVEDATA_VERSION);
            for (byte index1 = 0; index1 < PlayerInventory.PAGES - 2; ++index1)
            {
                byte num1;
                byte num2;
                byte num3;
                num1 = myItems[index1].Width;
                num2 = myItems[index1].Height;
                num3 = (byte)myItems[index1].items.Count;
                block.writeByte(num1);
                block.writeByte(num2);
                block.writeByte(num3);
                for (byte index2 = 0; index2 < num3; ++index2)
                {
                    ItemJar itemJar = myItems[index1].items[index2];
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.x);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.y);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.rot);
                    block.writeUInt16(itemJar == null ? (ushort)0 : itemJar.item.id);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.item.amount);
                    block.writeByte(itemJar == null ? (byte)0 : itemJar.item.quality);
                    block.writeByteArray(itemJar == null ? new byte[0] : itemJar.item.state);
                }
            }
            //PlayerSavedata.writeBlock(this.channel.owner.playerID, "/Player/Inventory.dat", block);
            ServerSavedata.writeBlock(writepath, block);
        }
        private (List<Item>, List<MyItem>) GetPlayerItems(string str)//look up a call of GetPlayerItems for "str" for more info
        {
            List<Item> items = new List<Item>();
            List<MyItem> myItems = new List<MyItem>();
            Block block = ServerSavedata.readBlock("/Players/" + str + "/" + Provider.map + "/Player/Inventory.dat", 0);
            //PlayerSavedata.readBlock(steamPlayerID, "/Player/Inventory.dat", (byte)0);
            byte num1 = block.readByte();//BUFFER_SIZE
            for (byte index1 = 0; index1 < PlayerInventory.PAGES - 2; ++index1)
            {
                //this.items[(int)index1].loadSize(block.readByte(), block.readByte());

                byte width = block.readByte();
                byte height = block.readByte();
                byte itemCount = block.readByte();
                for (byte index2 = 0; index2 < itemCount; ++index2)
                {
                    byte x = block.readByte();
                    //block.readByte();
                    byte y = block.readByte();
                    //block.readByte();
                    byte rot = 0;
                    if (num1 > 4)
                        rot = block.readByte();
                    //block.readByte();
                    ushort newID = block.readUInt16();
                    byte newAmount = block.readByte();
                    byte newQuality = block.readByte();
                    byte[] newState = block.readByteArray();
                    //block.readByteArray();
                    //byte[] newState = new byte[1] { 1 };

                    Item item = new Item(newID, newAmount, newQuality, newState);
                    myItems.Add(new MyItem(width, height, x, y, rot, newID, newAmount, newQuality, newState));
                    if (HasItem(item, items))
                        continue;
                    else
                        items.Add(item);
                    //this.items[(int)index1].loadItem(x, y, rot, new Item(num3, newAmount, newQuality, newState));
                }
            }
            return (items, myItems);
        }
        internal static (string, string) GetSteamID(string line)
        {
            string[] str = line.Split('\\');
            string steamId = str[str.Length - 1];
            string map = str[str.Length - 2];

            int steamId32;
            if (!int.TryParse(steamId, out steamId32))
                throw new System.InvalidCastException($"Failed to get player SteamID at ItemRestrictorAdvanced.Watcher.GetSteamID(string line), output: {steamId}");

            return (steamId, map);
        }
        internal static bool IsPlayerOnline(string steamID)
        {
            foreach (var steamPlayer in SDG.Unturned.Provider.clients)
            {
                if (steamID == steamPlayer.playerID.ToString())
                    return true;
            }

            return false;
        }
        internal static string PlayerInPlayersFolder(string steamId)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo("../Players").GetDirectories())
            {
                if (directory.Name.Split('\\')[0] == steamId)
                    return directory.Name;
            }
            throw new System.IO.DirectoryNotFoundException($@"Failed to find: {steamId} in ../{Provider.serverName}/Players  folder!");
        }
        internal void LoadInventoryTo(string path)
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
                var(items, myItems) = GetPlayerItems(directory.Name);
                //new JSONSerializer().serialize<List<MyItem>>(myItems, newPath, false);
                using (StreamWriter streamWriter = new StreamWriter(newPath))//SDG.Framework.IO.Serialization
                {
                    JsonWriter jsonWriter = (JsonWriter)new JsonTextWriterFormatted((TextWriter)streamWriter);
                    //JsonSerializer jsonSerializer = new JsonSerializer();
                    new JsonSerializer().Serialize(jsonWriter, (object)items);
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
