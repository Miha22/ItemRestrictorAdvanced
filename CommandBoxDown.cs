using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using System.IO;

namespace ItemRestrictorAdvanced
{
    public class CommandBoxDown : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "dropbox";
        public string Help => "drops your vitrual box into a real box";
        public string Syntax => "/dropbox box_<index> or /dropbox <name of your box>";
        public List<string> Aliases => new List<string>() { "db" };
        public List<string> Permissions => new List<string>() { "rocket.dropbox", "rocket.db" };
        //string path = Plugin.Instance.pathTemp;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer player = (UnturnedPlayer)caller;
            try
            {
                string path = $@"{Plugin.Instance.pathTemp}\{player.CSteamID.ToString()}\{command[0]}.dat";
                FileStream fs = new FileStream(path, FileMode.Open);
                fs.Close();
                //fs.Dispose();
            }
            catch(System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"{command[0]} does not exist in your virtual inventory!", Color.red);
                return;
            }
            GiveBox(player, player.CSteamID.ToString(), command[0]);
        }

        private static void GiveBox(UnturnedPlayer player, string steamID, string boxName)
        {
            Block block = Functions.ReadBlock(Plugin.Instance.pathTemp + $@"\{steamID}\{boxName}.dat", 0);
            ushort id = block.readByte();
            //Vector3 point = block.readSingleVector3();
            block.readSingleVector3();
            float x = (player.Player.look.aim.forward.x - player.Position.x < 5) ? player.Player.look.aim.forward.x : player.Position.x + 4;
            float y = (player.Player.look.aim.forward.y - player.Position.y < 5) ? player.Player.look.aim.forward.y : player.Position.y + 4;
            float z = (player.Player.look.aim.forward.z - player.Position.z < 5) ? player.Player.look.aim.forward.z : player.Position.z + 4;
            Vector3 point = new Vector3(x, y, z);
            byte angle_x = block.readByte();
            byte angle_y = block.readByte();
            byte angle_z = block.readByte();
            ulong owner = block.readUInt64();
            ulong group = block.readUInt64();
            byte[] state = block.readByteArray();
            Asset asset1 = Assets.find(EAssetType.ITEM, id);
            ItemBarricadeAsset asset2 = asset1 as ItemBarricadeAsset;
            Barricade barricade = new Barricade(id, 100, state, asset2);
            //byte x = block.readByte();//player region
            //byte y = block.readByte();//player region
            //idea is to spawn barricade in player region
            //BarricadeRegion region = BarricadeManager.regions[(int)x, (int)y];
            //Transform hit = BarricadeTool.getBarricade(region.parent, 100, owner, group, point, Quaternion.Euler((float)((int)angle_x * 2), (float)((int)angle_y * 2), (float)((int)angle_z * 2)), id, state, asset2);
            BarricadeManager.dropBarricade(barricade, null, point, angle_x, angle_y, angle_z, owner, group);
            //BarricadeManager.instance.channel.send("tellBarricade", ESteamCall.OTHERS, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object)x, (object)y, (object)ushort.MaxValue, (object)id, (object)state, (object)point, (object)angle_x, (object)angle_y, (object)angle_z, (object)owner, (object)group, (object)1);
            //BarricadeManager.instance.channel.send("tellBarricade", ESteamCall.OTHERS, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object)x, (object)y, (object)ushort.MaxValue, (object)368, (object)new byte[0], (object)point2, (object)angle_x, (object)angle_y, (object)angle_z, (object)owner, (object)group, (object)2);
            //block.writeUInt16(barricadeData.barricade.id);
            //block.writeByte(x);
            //block.writeByte(y);
            //block.writeSingleVector3(point);
            //block.writeByte(barricadeData.angle_x);
            //block.writeByte(barricadeData.angle_y);
            //block.writeByte(barricadeData.angle_z);
            //block.writeUInt64(barricadeData.owner);
            //block.writeUInt64(barricadeData.group);
            //block.writeUInt64(instanceID);
            //block.writeByteArray(barricadeData.barricade.state);


            //byte[] state = new byte[16];
            //for (byte i = 0; i < 16; i++)//spell
            //    state[i] = block.readByte();
            //byte[] numArray = block.readByteArray();
            //byte[] newState = new byte[state.Length + numArray.Length];
            //for (ushort i = 0; i < state.Length; i++)//spell
            //    newState[i] = state[i];
            //for (ushort i = (ushort)state.Length; i < newState.Length; i++)//state
            //    newState[i] = numArray[i - state.Length];

            //for (byte i = 0; i < numArray.Length; i++)
            //{
            //    System.Console.WriteLine($"state{i}: {numArray[i]}");
            //}
            //Item barricade = new Item(barricadeID, 1, 100, numArray);

            //Item item = new Item(328, false);
            //Item item2 = new Item(368, 1, 100, new byte[0]);
            //Barricade barricade = new Barricade(barricadeID, barricadeHE, numArray, new ItemBarricadeAsset())
            //player.GiveItem(barricade);
            //player.Inventory.tryAddItemAuto(barricade, false, false, false, false);
            //player.Player.inventory.forceAddItemAuto(barricade, false, true, false);
            //player.Inventory.forceAddItemAuto(barricade, false, true, false);
            //player.Inventory.forceAddItemAuto(item, false, true, false);
            //player.Inventory.forceAddItemAuto(item2, false, true, false);
            System.Console.WriteLine("box was spawned!");
        }

        //BarricadeManager.manager.channel.send("tellBarricade", ESteamCall.OTHERS, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object) x, (object) y, (object) ushort.MaxValue, (object) barricade.id, (object) barricade.state, (object) barricadeData.point, (object) barricadeData.angle_x, (object) barricadeData.angle_y, (object) barricadeData.angle_z, (object) barricadeData.owner, (object) barricadeData.group, (object) instanceID);
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.