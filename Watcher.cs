using Logger = Rocket.Core.Logging.Logger;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    public class Watcher
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run(string watchPath, CancellationToken token)
        {
            //string[] args = Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            //if (args.Length != 2)
            //{
            //    // Display the proper way to call the program.
            //    Console.WriteLine("Usage: Watcher.exe (directory)");
            //    return;
            //}

            // Create a new FileSystemWatcher and set its properties.
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = watchPath;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastWrite;//|
                                                               //| NotifyFilters.LastAccess
                                                               //| NotifyFilters.FileName
                                                               //| NotifyFilters.DirectoryName;

                // Only watch text files.
                watcher.Filter = "*.json";

                // Add event handlers.
                watcher.Changed += OnChanged;
                //watcher.Created += OnChanged;
                //watcher.Deleted += OnChanged;
                //watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the user to quit the program.
                //Console.WriteLine("Type: '-watcher' or '--w' to disable watcher.");
                while (!token.IsCancellationRequested) ;
            }
            //System.Diagnostics.Process.Start($@"E:\Users\M22\Documents\GitHub\tx.txt");
            //"Thread has been stopped!"
        }

        // Define the event handlers.
        //  Specify what is done when a file is changed, created, or deleted.
        //private static void OnChanged(object source, FileSystemEventArgs e) =>
        //    Console.WriteLine($"File name: {e.Name}, obj: {e.FullPath}");
        // write block


        //private static void OnRenamed(object source, RenamedEventArgs e) =>
        // Specify what is done when a file is renamed.
        //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");

        //File: E:\Program Files (x86)\steam\steamapps\common\Unturned_Server\Servers\server1\Rocket\Plugins\ItemRestrictorAdvanced\Inventories\76561198112559333.json Changed
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            System.Console.WriteLine("OnChange executed!");
            var (playerSteamID, map) = GetSteamID(e.Name);
            System.Console.WriteLine($"playerSteamID: {playerSteamID}, map: {map}");
            if (IsPlayerOnline(playerSteamID))
            {
                System.Console.WriteLine("Player is online!");
            }
            else
            {
                string pathWrite = $@"\Players\{PlayerInPlayersFolder(playerSteamID)}\{map}\Player\Inventory.dat";
                string pathPages = $@"{e.FullPath.Substring(0, e.FullPath.Length - 23)}\Data_DoNotTouch\Pages_{playerSteamID}.json";
                //string pathCached = $@"\Players\{PlayerInPlayersFolder(playerSteamID)}\{map}\Player\InventoryCACHED.dat";
                //System.Console.WriteLine("Try add Items is going to be executed!");
                //FileInfo fileInfo = new FileInfo(pathWrite);
                //if (fileInfo.Exists)
                //{
                //    fileInfo.CopyTo(pathCached).Delete();
                //    fileInfo = null;
                //} Inventory.json
                //if (ItemRestrictor.Instance.TryAddItems(path, e.FullPath))
                //    new FileInfo(pathCached).Delete();
                //else
                //    new FileInfo(pathCached).MoveTo(path);\
                (bool flag, List<MyItem> unSelected) = ItemRestrictor.Instance.TryAddItems(pathWrite, e.FullPath, pathPages);
                //System.Console.WriteLine($"Exists? {File.Exists(pathPages)}");
                //System.Console.WriteLine(pathPages);
                System.Console.WriteLine("Try add Items executed! success");
            }
        }
        private (string, string) GetSteamID(string line)
        {
            string[] str = line.Split('\\');
            string steamId = str[str.Length - 1].Split('.')[0];
            string map = str[str.Length - 2];
            byte steamId32;

            if (!byte.TryParse(steamId.Substring(0, 2), out steamId32))
            {
                Logger.LogException(new System.InvalidCastException(), $"Failed to get player SteamID at ItemRestrictorAdvanced.Watcher.GetSteamID(string line), output: {steamId}");
                return (null, null);
            }
            if (!byte.TryParse(steamId.Substring(steamId.Length-1-2, 2), out steamId32))
            {
                Logger.LogException(new System.InvalidCastException(), $"Failed to get player SteamID at ItemRestrictorAdvanced.Watcher.GetSteamID(string line), output: {steamId}");
                return (null, null);
            }

            return (steamId, map);
        }
        private bool IsPlayerOnline(string steamID)
        {

            if (SDG.Unturned.Provider.clients.Count == 0)
                return false;
            foreach (var steamPlayer in SDG.Unturned.Provider.clients)
            {
                if (steamID == steamPlayer.playerID.ToString())
                    return true;
            }

            return false;
        }
        private string PlayerInPlayersFolder(string steamId)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo("../Players").GetDirectories())
            {
                if (directory.Name.Split('_')[0] == steamId)
                    return directory.Name;
            }
            Logger.LogException(new System.IO.DirectoryNotFoundException(), $@"Failed to find: {steamId} in ../{SDG.Unturned.Provider.serverName}/Players  folder!");
            return null;
        }
        //private static string GetSteamID(string line)
        //{
        //    int index = 0;
        //    for (int i = line.Length-5, j = 0; j <= 17; j++, --i)
        //    {
        //        index = i;
        //    }
        //    Console.WriteLine($"Steam id: {line.Substring(index, 16)}");
        //    string steamId = line.Substring(index, 17);
        //    int steamId32;
        //    if (!Int32.TryParse(steamId, out steamId32))
        //        throw new InvalidCastException($"Unsuccessfull try to get playerSteamID at ItemRestrictorAdvanced.Watcher.GetSteamID(string line), output: {steamId}");

        //    return steamId;
        //}
    }

}
