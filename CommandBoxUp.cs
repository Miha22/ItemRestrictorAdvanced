using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using System.IO;
using Steamworks;

namespace ItemRestrictorAdvanced
{
    public class CommandBoxUp : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sendbox";
        public string Help => "watch on special loot box and execute /sendbox or /sendbox <prefered box name>";
        public string Syntax => "/sendbox or /sendbox <name of your box>";
        public List<string> Aliases => new List<string>() { "sb" };
        public List<string> Permissions => new List<string>() { "rocket.sendbox", "rocket.sb" };
        //string path = Plugin.Instance.pathTemp;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length > 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, 4, RayMasks.BARRICADE_INTERACT))
            {
                if (BarricadeManager.tryGetInfo(hit.transform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion r))
                {
                    BarricadeData bdata = r.barricades[index];
                    if (bdata.barricade.id != 3280 || bdata.owner != player.CSteamID.m_SteamID)
                    {
                        Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Error occured: this barricade is not a virtual inventory box or box is not yours.", Color.red);
                        Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Owner steamID: {bdata.owner}\r\nYour steamID: {player.CSteamID.ToString()}");
                        return;
                    }

                    //System.Console.WriteLine("point 0");
                    BarricadeManager.damage(hit.transform, ushort.MaxValue, 1, false);
                    //List<RegionCoordinate> regionCoordinates = new List<RegionCoordinate>();
                    List<InteractableItem> interactableItems = new List<InteractableItem>();
                    Vector3 center = hit.transform.position;
                    float sqrRadius = 2;
                    BarricadeManager.tryGetRegion(hit.transform, out byte _x, out byte _y, out ushort _plant, out BarricadeRegion _region);
                    RegionCoordinate regionCoordinate = new RegionCoordinate(_x, _y);
                    if (ItemManager.regions[(int)regionCoordinate.x, (int)regionCoordinate.y] != null)
                    {
                        System.Console.WriteLine("point 0");
                        System.Console.WriteLine($"ItemManager.regions[_x, _y].drops.Count: {ItemManager.regions[_x, _y].drops.Count}");
                        for (int index2 = 0; index2 < ItemManager.regions[_x, _y].drops.Count; ++index2)
                        {
                            System.Console.WriteLine("point 1");
                            ItemDrop drop = ItemManager.regions[(int)regionCoordinate.x, (int)regionCoordinate.y].drops[index2];
                            if ((double)(drop.model.position - center).sqrMagnitude < (double)sqrRadius)
                                interactableItems.Add(drop.interactableItem);
                        }
                        System.Console.WriteLine("point 2");
                    }
                    else
                        System.Console.WriteLine("Item manager region is null");

                    //ItemManager.getItemsInRadius(bdata.point, (float)4, regionCoordinates, interactableItems);
                    //System.Console.WriteLine($"regionCoordinates: {regionCoordinates.Count}");
                    System.Console.WriteLine($"interactableItems: {interactableItems.Count}");
                    ItemRegion region = ItemManager.regions[(int)x, (int)y];
                    for (ushort ind = 0; (int)ind < region.drops.Count; ++ind)
                    {
                        foreach (var item in interactableItems)
                        {
                            if ((int)region.drops[(int)ind].instanceID == (int)item.GetInstanceID())
                            {
                                if (ItemManager.onItemDropRemoved != null)
                                    ItemManager.onItemDropRemoved(region.drops[(int)ind].model, region.drops[(int)ind].interactableItem);
                                Object.Destroy((Object)region.drops[(int)ind].model.gameObject);
                                region.drops.RemoveAt((int)ind);
                                //break;
                            }
                        }
                    }
                    //r.drops.Clear();
                    //Object.Destroy(hit.transform.gameObject);
                    //BarricadeManager.salvageBarricade(hit.transform);
                    //System.Console.WriteLine("point 1");
                    //BarricadeManager.instance.channel.send("tellTakeBarricade", ESteamCall.ALL, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object)x, (object)y, (object)plant, (object)index);
                    //BarricadeManager.instance.channel.send("tellTakeBarricade", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object)x, (object)y, (object)plant, (object)index);
                    
                    //BarricadeManager.clearPlants();
                    //BarricadeManager.instance.channel.send("askSalvageBarricade", ESteamCall.SERVER, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, (object)x, (object)y, (object)plant, (object)index);
                    //System.Console.WriteLine("point 2");
                    StateToBlock(bdata.barricade, player.CSteamID, (command.Length == 0) ? SetBoxName(Plugin.Instance.pathTemp + $@"\{player.CSteamID}") : command[0]);
                    System.Console.WriteLine("point 3");
                    //r.barricades[index].barricade.state = new byte[0];
                    //BarricadeManager.damage(hit.transform, ushort.MaxValue, 1, false);
                    //Object.Destroy(hit.transform.gameObject);
                }
            }
        }

        private static void StateToBlock(Barricade barricade, CSteamID steamID, string boxName)
        {
            Block block = new Block();
            block.writeUInt16(barricade.id);
            block.writeUInt16(barricade.health);
            Plugin.Instance.WriteSpell(block);
            block.writeByteArray(barricade.state);
            Functions.WriteBlock(Plugin.Instance.pathTemp + "\\" + steamID.ToString() + "\\" + boxName, block, false);
        }

        private string SetBoxName(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
                directory.Create();
            DirectoryInfo[] directories = directory.GetDirectories();

            //Directory.CreateDirectory(path + $@"\box_{directories.Length - 1}");
            return $"box_{directories.Length}.dat";
        }

        //private void UploadItems(List<MyItem> items, string playerSteamID)
        //{
        //    string path = Plugin.Instance.pathTemp + $"\\{playerSteamID}";
        //    List<List<MyItem>> boxes = CreateBoxes(items);
        //    foreach (List<MyItem> box in boxes)
        //    {
        //        Block block = new Block();
        //        block.writeUInt16(3280);
        //        block.writeUInt16(600);
        //        Plugin.Instance.WriteSpell(block);
        //        block.writeUInt16((ushort)box.Count);
        //        foreach (var myItem in box)
        //            block.writeByteArray(myItem.State);
        //        Functions.WriteBlock(path + $"\\{SetBoxName(path)}", block, false);
        //    }
        //}

        //private List<List<MyItem>> CreateBoxes(List<MyItem> myItems)
        //{
        //    List<List<MyItem>> boxes = new List<List<MyItem>>();
        //    do
        //    {
        //        List<MyItem> selectedItems = new List<MyItem>();
        //        bool[,] page = Plugin.Instance.FillPage(10, 6);
        //        foreach (var item in myItems)
        //        {
        //            if ((item.Size_x > 10 && item.Size_y > 6) || (item.Size_x > 6 && item.Size_y > 10))
        //                continue;
        //            if (Plugin.Instance.FindPlace(ref page, 10, 6, item.Size_x, item.Size_y, out byte x, out byte y))
        //            {
        //                item.X = x;
        //                item.Y = y;
        //                selectedItems.Add(item);
        //                myItems.Remove(item);
        //            }
        //            else
        //            {
        //                if (Plugin.Instance.FindPlace(ref page, 6, 10, item.Size_y, item.Size_x, out byte newX, out byte newY))
        //                {
        //                    item.X = newX;
        //                    item.Y = newY;
        //                    item.Rot = 1;
        //                    selectedItems.Add(item);
        //                    myItems.Remove(item);
        //                }
        //            }
        //        }
        //        boxes.Add(selectedItems);
        //    } while (myItems.Count != 0);

        //    return boxes;
        //}
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.