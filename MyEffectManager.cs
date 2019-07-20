using SDG.Unturned;
using Steamworks;

namespace ItemRestrictorAdvanced
{
    class MyEffectManager : EffectManager
    {
        public static void sendUIEffect(ushort id, short key, CSteamID steamID, bool reliable, params string[] arg)
        {
            object[] newArgs = new object[arg.Length + 2];
            newArgs[0] = id;
            newArgs[1] = key;
            for (byte i = 2; i < newArgs.Length; i++)
                newArgs[i] = arg[i-2];
            //send("tellUIEffect4Args", steamID, (ESteamPacket) (!reliable ? 16 : 15), (object) id, (object) key, (object) arg0, (object) arg1, (object) arg2, (object) arg3);
            instance.channel.send("tellUIEffectText", steamID, (ESteamPacket)(!reliable ? 16 : 15), newArgs);
        }
      //  [SteamCall(ESteamCallValidation.ONLY_FROM_SERVER)]
      //  public static void tellUIEffectParamsArgs(
      //CSteamID steamID,
      //ushort id,
      //short key,
      //params string[] args)
      //  {
      //      //if (!channel.checkServer(steamID))
      //      //    return;
      //      EffectManager.createAndFormatUIEffect(id, key, args);
      //  }
    }
}
