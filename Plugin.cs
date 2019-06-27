using Rocket.Core.Plugins;
using System;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.API;
using Rocket.Core.Commands;
using SDG.Unturned;

namespace ItemRestrictor
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
                _instance = this;
                Logger.Log("ItemRestrictor loaded!", ConsoleColor.Cyan);
            }
            else
            {
                UnloadPlugin();
                Logger.Log("Configuration.Instance.Enabled == false");
                Logger.Log("ItemRestrictor Unloaded!", ConsoleColor.Cyan);
            }
        }

        [RocketCommand("inventorycheck", "", "", AllowedCaller.Player)]
        [RocketCommandAlias("inv")]
        public void Execute(IRocketPlayer caller, string[] command)
        {
            foreach (var steamPlayer in Provider.clients)
            {
                
            }
        }

        private Item GetPlayerItems(SteamPlayer steamPlayer)
        {
            Block block = PlayerSavedata.readBlock(steamPlayer.playerID, "/Player/Inventory.dat", (byte)0);
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
                    if ((ItemAsset)Assets.find(EAssetType.ITEM, newID) != null)
                    {
                        //this.items[(int)index1].loadItem(x, y, rot, new Item(num3, newAmount, newQuality, newState));
                        return new Item(newID, newAmount, newQuality, newState);
                    }
                    else
                        Logger.Log("Item not found!");
                }
            }

            return null;
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

        //public static bool IsPlayersGroup(IRocketPlayer caller, Group group)
        //{
        //    string[] groups = R.Permissions.GetGroups(caller, true).Select(g => g.Id).ToArray();
        //    for (ushort i = 0; i < groups.Length; i++)
        //    {
        //        if (group.GroupID.ToLower() == groups[i].ToLower())
        //            return true;
        //    }

        //    return false;
        //}
    }
}
