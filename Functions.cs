using SDG.Unturned;
using System.IO;

namespace ItemRestrictorAdvanced
{
    class Functions
    {
        public static void WriteBlock(string path, Block block)
        {
            int size;
            byte[] bytes = block.getBytes(out size);
            writeBytes(path, bytes, size);
        }
        private static void writeBytes(
          string path,
          byte[] bytes,
          int size)
        {
            //if (useCloud)
            //{
            //    ReadWrite.cloudFileWrite(path, bytes, size);
            //}
            //path = ReadWrite.PATH + path;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream fileStream = new FileStream(path, FileMode.Append))
            {
                fileStream.Write(bytes, 0, size);
                fileStream.SetLength((long)size);
                fileStream.Flush();
                fileStream.Close();
                //fileStream.Dispose();
            }
        }

        public static Block ReadBlock(string path, byte prefix)
        {
            return readBlock(ServerSavedata.directory + "/" + Provider.serverID + path, prefix);
        }
        private static Block readBlock(string path, byte prefix)
        {
            return readBlockRW(path, true, prefix);
        }
        private static Block readBlockRW(string path, bool usePath, byte prefix)
        {
            byte[] contents = readBytes(path, usePath);
            if (contents == null)
                return (Block)null;
            return new Block(prefix, contents);
        }
        private static byte[] readBytes(string path, bool usePath)
        {
            //if (useCloud)
            //    return ReadWrite.cloudFileRead(path);
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
