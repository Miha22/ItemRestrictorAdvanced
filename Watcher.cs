using System;
using System.IO;
using System.Security.Permissions;

namespace ItemRestrictorAdvanced
{
    public class Watcher
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run(string watchPath)
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
                while (Console.ReadLine() != "--w") ;
            }
        }

        // Define the event handlers.
        //private static void OnChanged(object source, FileSystemEventArgs e) =>
         //Specify what is done when a file is changed, created, or deleted.
        //Console.WriteLine($"File name: {e.Name}, obj: {source.ToString()}");
        // write block


        //private static void OnRenamed(object source, RenamedEventArgs e) =>
        // Specify what is done when a file is renamed.
        //Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");

        //File: E:\Program Files (x86)\steam\steamapps\common\Unturned_Server\Servers\server1\Rocket\Plugins\ItemRestrictorAdvanced\Inventories\76561198112559333.json Changed
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var(playerSteamID, map) = Functions.GetSteamID(e.Name);
            if (Functions.IsPlayerOnline(playerSteamID))
            {
               //load inventory
            }
            else
            {
                Functions.writeBlock($@"\Players\{Functions.PlayerInPlayersFolder(playerSteamID)}\{map}\Player\Inventory.dat", e.FullPath);
            }
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
