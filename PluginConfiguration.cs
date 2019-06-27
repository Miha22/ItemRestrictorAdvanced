#region Configuration
using Rocket.API;
using System.Collections.Generic;

namespace ItemRestrictor
{
    public class PluginConfiguration : IRocketPluginConfiguration
    {
        public bool Enabled;
        public bool ItemLimits;
        public bool VehicleLimits;
        public bool PlayerTotalVehicles;
        public List<Group> Groups;
        public void LoadDefaults()
        {
            Groups = new List<Group>
            {
                new Group()
                {
                    GroupID = "default",
                    BlackListItems = new List<ushort>() { 0 },
                    BlackListVehicles = new List<ushort>() { 0 },
                    ItemLimits = new List<ItemBan>()
                    {
                        new ItemBan(1059, 9),
                        new ItemBan(1092, 8),
                        new ItemBan(1071, 7),
                        new ItemBan(1065, 6)
                    },
                    VehicleLimits = new List<Vehicle>()
                    {
                        new Vehicle(0, 0)
                    },
                    PlayerTotalVehicles = 3
                },
                new Group()
                {
                    GroupID = "vip",
                    BlackListItems = new List<ushort>() { 1394, 519, 1241 },
                    BlackListVehicles = new List<ushort>() { 120, 121, 137 },
                    ItemLimits = new List<ItemBan>()
                    {
                        new ItemBan(132, 6),
                        new ItemBan(133, 10),
                    },
                    VehicleLimits = new List<Vehicle>()
                    {
                        new Vehicle(57, 3),
                        new Vehicle(53, 3),
                        new Vehicle(119, 3),
                        new Vehicle(148, 3),
                        new Vehicle(172, 3),
                    },
                    PlayerTotalVehicles = 5
                },
                new Group()
                {
                    GroupID = "moderator",
                    BlackListItems = new List<ushort>() { 519, 1241, 519 },
                    BlackListVehicles = new List<ushort>() { 137, 120, 137 },
                    ItemLimits = new List<ItemBan>()
                    {
                        new ItemBan(121, 8),
                        new ItemBan(122, 18),
                    },
                    VehicleLimits = new List<Vehicle>()
                    {
                        new Vehicle(121, 3),
                        new Vehicle(122, 4)
                    },
                    PlayerTotalVehicles = 7
                }
            };
            Enabled = true;
            ItemLimits = false;
            VehicleLimits = false;
            PlayerTotalVehicles = false;
        }
    }
}
#endregion Configuration
