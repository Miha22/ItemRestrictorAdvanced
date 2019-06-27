using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Commands;
using SDG.Unturned;
using Logger = Rocket.Core.Logging.Logger;

namespace ItemRestrictorAdvanced
{
    class ItemRestrictor : RocketPlugin<PluginConfiguration>
    {
        internal static ItemRestrictor _instance;

        public ItemRestrictor()
        {

        }

        protected override void Load()
        {
            if (Configuration.Instance.Enabled)
            {
                //Start();
                _instance = this;
                Logger.Log("ItemRestrictorAdvanced by M22 loaded!", ConsoleColor.Yellow);
            }
            else
            {
                UnloadPlugin();
            }
        }

        [RocketCommand("inventorycheck", "", "", AllowedCaller.Player)]
        [RocketCommandAlias("inv")]
        public void Execute(IRocketPlayer caller, string[] command)
        {
            
        }

        void Start()
        {
            foreach (DirectoryInfo directory in new DirectoryInfo("../Players").GetDirectories())
            {
                try
                {
                    string path = $@"..\Players\{directory.Name}\{Provider.map}\Player\Inventory.txt";
                    string path2 = $@"..\Players\{directory.Name}\{Provider.map}\Inventory.txt";
                    if (!File.Exists(path))
                        File.Create(path);
                    using (StreamWriter sw = new StreamWriter(path2, false, System.Text.Encoding.Default))
                    {
                        //string[] playerID = directory.Name.Split('_');
                        //Console.WriteLine($"player id: {playerID[0]}, char id: {playerID[1]}");
                        List<Item> playerItems = GetPlayerItems(directory.Name);
                        foreach (Item item in playerItems)
                        {
                            //sw.WriteLine($"ID: {item.id}\n Amount: {item.amount}\n Quality: {item.quality}");
                            sw.WriteLine($"ID: {item.id}");
                            sw.WriteLine($"Amount: {item.amount}");
                            sw.WriteLine($"Quality: {item.quality}");
                            sw.WriteLine("-------");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError($"{e.Message}\n{e.TargetSite}");
                }
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
                    byte[] newState = block.readByteArray();

                    Item item = new Item(newID, newAmount, newQuality, newState);
                    if (items.Contains(item))
                        continue;
                    else
                        items.Add(item);

                    //this.items[(int)index1].loadItem(x, y, rot, new Item(num3, newAmount, newQuality, newState));
                }
            }
            return items;
        }

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
