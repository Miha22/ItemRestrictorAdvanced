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
            ushort id = block.readUInt16();
            //Vector3 point = block.readSingleVector3();
            block.readSingleVector3();
            float x = (player.Player.look.aim.forward.x - player.Position.x < 5) ? player.Player.look.aim.forward.x : player.Position.x + 4;
            float y = (player.Player.look.aim.forward.y - player.Position.y < 5) ? player.Player.look.aim.forward.y : player.Position.y + 4;
            float z = (player.Player.look.aim.forward.z - player.Position.z < 5) ? player.Player.look.aim.forward.z : player.Position.z + 4;
            Vector3 point = new Vector3(x, y, z);
            float angle_x = block.readByte();
            float angle_y = block.readByte();
            float angle_z = block.readByte();
            ulong owner = block.readUInt64();
            ulong group = block.readUInt64();
            byte[] state = block.readByteArray();
            Asset asset1 = Assets.find(EAssetType.ITEM, id);
            ItemBarricadeAsset asset2 = asset1 as ItemBarricadeAsset;
            Barricade barricade = new Barricade(id, 100, state, asset2);

            //Transform hit = BarricadeTool.getBarricade(region.parent, 100, owner, group, point, Quaternion.Euler((float)((int)angle_x * 2), (float)((int)angle_y * 2), (float)((int)angle_z * 2)), id, state, asset2);

            BarricadeManager.dropBarricade(barricade, null, point, angle_x, angle_y, angle_z, owner, group);
            //block.writeUInt16(bdata.barricade.id);
            //block.writeSingleVector3(bdata.point);
            //block.writeByte(bdata.angle_x);
            //block.writeByte(bdata.angle_y);
            //block.writeByte(bdata.angle_z);
            //block.writeUInt64(bdata.owner);
            //block.writeUInt64(bdata.group);
            //block.writeByteArray(bdata.barricade.state);

            System.Console.WriteLine("box was spawned!");
        }

        //BarricadeManager.manager.channel.send("tellBarricade", ESteamCall.OTHERS, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object) x, (object) y, (object) ushort.MaxValue, (object) barricade.id, (object) barricade.state, (object) barricadeData.point, (object) barricadeData.angle_x, (object) barricadeData.angle_y, (object) barricadeData.angle_z, (object) barricadeData.owner, (object) barricadeData.group, (object) instanceID);
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.