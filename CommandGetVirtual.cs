using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class CommandGetVirtual : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "incloud";
        public string Help => "Shows your virtual inventory using UI";
        public string Syntax => "/incloud or /inc";
        public List<string> Aliases => new List<string>() { "inc" };
        public List<string> Permissions => new List<string>() { "rocket.incloud", "rocket.inc"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Block block = Functions.ReadBlock(Plugin.Instance.pathTemp + $"\\{player.CSteamID}\\Heap.dat", 0);
            if (block.block.Length == 0)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, "You do not have items in cloud inventory!");
                return;
            }
            (List<List<MyItem>> myItemsPages, byte pagesCount) = Functions.GetMyItems(block);
            EffectManager.sendUIEffect(8101, 26, player.CSteamID, false);
            for (byte i = 0; i < pagesCount; i++)
                for (byte j = 0; j < myItemsPages[i].Count; j++)
                    EffectManager.sendUIEffectText(26, player.CSteamID, false, $"item{j}", $"{((ItemAsset)Assets.find(EAssetType.ITEM, myItemsPages[i][j].ID)).itemName}\r\nID: {myItemsPages[i][j].ID}\r\nCount: {myItemsPages[j][i].Count}");
            for (byte i = (byte)(myItemsPages[pagesCount - 1].Count); i < 24; i++)
                EffectManager.sendUIEffectText(26, player.CSteamID, false, $"item{i}", $"");
            EffectManager.sendUIEffectText(26, player.CSteamID, false, "playerName", $"Cloud: {player.CharacterName}");
            EffectManager.onEffectButtonClicked += new ManageCloudUI(myItemsPages, pagesCount).OnEffectButtonClick8101;
            player.Player.serversideSetPluginModal(true);  
        }
    }
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.