using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ItemRestrictorAdvanced
{
    public class CommandTest : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "closegetinventory";
        public string Help => "Loads some player's inventory to your inventory, after you finished edit it, it loads to that player";
        public string Syntax => "/closegetinventory <player>  /cgi <player>";
        public List<string> Aliases => new List<string>() { "closegetinventory", "cgi" };
        public List<string> Permissions => new List<string>() { "rocket.closegetinventory", "rocket.cgi" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            //int len = (VehicleManager.vehicles.Count - 1);
            //for (int index1 = len; index1 > 0; index1--)
            //{
            //    //Console.WriteLine($"index1: {index1}");
            //    //Console.WriteLine($"VehicleManager.vehicles.Count: {VehicleManager.vehicles.Count}");
            //    //Console.WriteLine($"vehicles Count: {VehicleManager.vehicles.Count}, index1: {index1}");
            //    UnityEngine.Object.Destroy((UnityEngine.Object)VehicleManager.vehicles[index1].gameObject);
            //    //Console.WriteLine($"vehicles Count after destroy: {VehicleManager.vehicles.Count - 1}");
            //    VehicleManager.vehicles.RemoveAt(index1);
            //    //Console.WriteLine($"index: {index1}, veh: {VehicleManager.vehicles[index1].instanceID}");
            //    //Console.WriteLine("remove passed");
            //}
            //UnityEngine.Object.Destroy((UnityEngine.Object)VehicleManager.vehicles[0].gameObject);
            //Console.WriteLine($"vehicles: {VehicleManager.vehicles.Count}");
            foreach (var st in Provider.clients)
            {
                st.player.inventory.onInventoryAdded += OnInventoryChange;
            }
        }
        private void OnInventoryChange(byte page, byte index, ItemJar item)
        {
            Console.WriteLine($"item changed! in player inventory");
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.