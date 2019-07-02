using System;
using Rocket.Core.Plugins;
using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using System.Threading.Tasks;
using Rocket.Core.Commands;
using System.Collections.Generic;
using SDG.Unturned;

namespace ItemRestrictorAdvanced
{
    class ItemRestrictor : RocketPlugin<PluginConfiguration>
    {
        static string path = $@"Plugins\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}\Inventories\{SDG.Unturned.Provider.map}";
        internal static ItemRestrictor _instance;
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
                _instance = this;
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory.Attributes == FileAttributes.ReadOnly)
                    directory.Attributes &= ~FileAttributes.ReadOnly;

                try
                {
                    new Functions().LoadInventoryTo(path);
                    WatcherAsync(token);
                    Logger.Log("ItemRestrictorAdvanced by M22 loaded!", ConsoleColor.Yellow);
                }
                catch (Exception e)
                {
                    cts.Cancel();
                    Logger.LogException(e, $"EXCEPTION MESSAGE: {e.Message} \n EXCEPTION TargetSite: {e.TargetSite} \n EXCEPTION StackTrace {e.StackTrace}");
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
    }
}
