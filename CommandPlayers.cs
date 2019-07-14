using Rocket.API;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class CommandPlayers : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "Players";
        public string Help => "help";
        public string Syntax => "syntax";
        public List<string> Aliases => new List<string>() { "pl", "pls" };
        public List<string> Permissions => new List<string>() { "rocket.players" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            //for (byte i = 0; i < 8; i++)
            //{
            //    for (byte j = 0; j < ((UnturnedPlayer)caller).Inventory.getItemCount(i); j++)
            //    {
            //        Console.WriteLine($"Sorted items: {((UnturnedPlayer)caller).Inventory.getItem(i, j).item.id}, size x: {((UnturnedPlayer)caller).Inventory.getItem(i, j).size_x}, size y: {((UnturnedPlayer)caller).Inventory.getItem(i, j).size_y}, rot: {((UnturnedPlayer)caller).Inventory.getItem(i, j).rot}, x: {((UnturnedPlayer)caller).Inventory.getItem(i, j).x}, y: {((UnturnedPlayer)caller).Inventory.getItem(i, j).y}");
            //    }
            //}
            //foreach (var steamPlayer in Provider.clients)
            //{
            //    Console.WriteLine("----------------------------");
            //    Console.WriteLine($"character name: {steamPlayer.playerID.characterName}");
            //    Console.WriteLine($"nickname name: {steamPlayer.playerID.nickName}");
            //    Console.WriteLine($"playerName name: {steamPlayer.playerID.playerName}");
            //    Console.WriteLine($"steamID: {steamPlayer.playerID.steamID.ToString()}");
            //    Console.WriteLine("----------------------------");
            //}
            EffectManager.sendUIEffect(1480, 1234, false);
        }
    }
}
