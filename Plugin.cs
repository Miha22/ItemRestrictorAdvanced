using System;
using Rocket.Core.Plugins;
using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using System.Threading.Tasks;

namespace ItemRestrictorAdvanced
{
    class ItemRestrictor : RocketPlugin<PluginConfiguration>
    {
        static string path = $@"Plugins\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}\Inventories\{SDG.Unturned.Provider.map}";
        internal static ItemRestrictor _instance;

        public ItemRestrictor()
        {
            
        }

        protected override void Load()
        {   
            if (Configuration.Instance.Enabled)
            {
                _instance = this;
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory.Attributes == FileAttributes.ReadOnly)
                    directory.Attributes &= ~FileAttributes.ReadOnly;

                try
                {
                    new Functions().LoadInventoryTo(path);
                    WatcherAsync();
                    Logger.Log("ItemRestrictorAdvanced by M22 loaded!", ConsoleColor.Yellow);
                }
                catch (Exception e)
                {
                    Logger.LogException(e, $"EXCEPTION MESSAGE: {e.Message} \n EXCEPTION TargetSite: {e.TargetSite} \n EXCEPTION StackTrace {e.StackTrace}");
                }
                //create json files for each player from inventory.dat..
            }
            else
            {
                Logger.Log("Plugin is turned off in Configuration, unloading...", ConsoleColor.Cyan);
                UnloadPlugin();
            }
        }
        static async void WatcherAsync()
        {
            //Console.WriteLine("Начало метода FactorialAsync"); // выполняется синхронно
            await Task.Run(()=>new Watcher().Run(path));                            // выполняется асинхронно
            //Console.WriteLine("Конец метода FactorialAsync");  // выполняется синхронно
        }

        //[RocketCommand("inventorycheck", "", "", AllowedCaller.Player)]
        //[RocketCommandAlias("inv")]
        //public void Execute(IRocketPlayer caller, string[] command)
        //{

        //}

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
