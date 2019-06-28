using System;
using Rocket.Core.Plugins;
using Rocket.API;
using Rocket.Core.Commands;
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
                new Functions().Start();
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

    }
}
