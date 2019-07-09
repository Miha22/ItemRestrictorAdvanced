using SDG.Unturned;
using System.IO;

namespace ItemRestrictorAdvanced
{
    class Functions
    {
        public static void WriteBlock(string path, Block block)
        {
            writeBlock(ServerSavedata.directory + "/" + Provider.serverID + path, false, block);
        }
        private static void writeBlock(string path, bool useCloud, Block block)
        {
            writeBlockRW(path, useCloud, true, block);
        }
        private static void writeBlockRW(string path, bool useCloud, bool usePath, Block block)
        {
            int size;
            byte[] bytes = block.getBytes(out size);
            writeBytes(path, useCloud, usePath, bytes, size);
        }
        private static void writeBytes(
          string path,
          bool useCloud,
          bool usePath,
          byte[] bytes,
          int size)
        {
            if (useCloud)
            {
                ReadWrite.cloudFileWrite(path, bytes, size);
            }
            else
            {
                if (usePath)
                    path = ReadWrite.PATH + path;
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, size);
                    fileStream.SetLength((long)size);
                    fileStream.Flush();
                    fileStream.Close();
                    //fileStream.Dispose();
                }
            }
        }

        public static Block ReadBlock(string path, byte prefix)
        {
            return readBlock(ServerSavedata.directory + "/" + Provider.serverID + path, false, prefix);
        }
        private static Block readBlock(string path, bool useCloud, byte prefix)
        {
            return readBlockRW(path, useCloud, true, prefix);
        }
        private static Block readBlockRW(string path, bool useCloud, bool usePath, byte prefix)
        {
            byte[] contents = readBytes(path, useCloud, usePath);
            if (contents == null)
                return (Block)null;
            return new Block((int)prefix, contents);
        }
        private static byte[] readBytes(string path, bool useCloud, bool usePath)
        {
            if (useCloud)
                return ReadWrite.cloudFileRead(path);
            if (usePath)
                path = ReadWrite.PATH + path;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (!File.Exists(path))
            {
                System.Console.WriteLine((object)("Failed to find file at: " + path));
                return (byte[])null;
            }
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] buffer = new byte[fileStream.Length];
            if (fileStream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                System.Console.WriteLine("Failed to read the correct file size.");
                return (byte[])null;
            }
            fileStream.Close();
            fileStream.Dispose();
            return buffer;
        }
    }
}
