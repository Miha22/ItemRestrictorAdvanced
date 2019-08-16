using Rocket.API;
using System.Collections.Generic;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging.Logger;

namespace ItemRestrictorAdvanced
{
    class CommandGetBox : IRocketCommand
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
            if (command.Length > 1)
            {
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, (IRocketCommand)this);
            }
        }
    }
    //public class RefreshOnD
    //{
    //    private CSteamID _steamID;
    //    public RefreshOnD(CSteamID steamID)
    //    {
    //        this._steamID = steamID;
    //    }
    //    public void Execute(UnturnedPlayer connectedPlayer)
    //    {
    //        EffectManager.askEffectClearByID(8100, _steamID);
    //        EffectManager.sendUIEffect(8100, 22, _steamID, false);
    //        for (byte i = 0; i < Provider.clients.Count; i++)
    //            EffectManager.sendUIEffectText(22, _steamID, false, $"text{i}", $"{Provider.clients[i].playerID.characterName}");

    //        U.Events.OnPlayerDisconnected -= Execute;
    //    }
    //}
}
//Effect ID is the id parameter, key is an optional instance identifier for modifying instances of an effect, 
//and child name is the unity name of a GameObject with a Text component.