using SDG.Unturned;
using System.IO;

namespace ItemRestrictorAdvanced
{
    class Functions
    {
        public static void WriteBlock(string path, Block block, bool isUnited)
        {
            int size = 0;
            byte[] bytes = isUnited ? block.block : block.getBytes(out size);
            writeBytes(path, bytes, isUnited ? bytes.Length : size);
        }

        private static void writeBytes(
          string path,
          byte[] bytes,
          int size)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fileStream.Write(bytes, 0, size);
                fileStream.SetLength(size);
            }
        }

        public static Block ReadBlock(string path, byte step)
        {
            byte[] contents = readBytes(path);
            if (contents == null)
                return null;
            return new Block(step, contents);
        }

        private static byte[] readBytes(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[fileStream.Length];
                if (fileStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    Rocket.Core.Logging.Logger.LogError($"Error: Failed to read the correct file size in {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.Functions.readBytes(string, FileMode, FileAcess, FileShare), returning null"); ;
                    return null;
                }
                return buffer;
            }
        }

        private static Block UniteBlocks(Block blockAdd, string path)
        {
            byte[] buffer = blockAdd.getBytes(out int size);
            blockAdd.block = new byte[size];
            for (ushort i = 0; i < size; i++)
                blockAdd.block[i] = buffer[i];

            Block blockExist = ReadBlock(path, 0);
            ushort blockCount = 0;
            if (blockExist.block.Length != 0)
                blockCount = (ushort)(blockExist.readByte() + (256 * blockExist.readByte()));

            byte[] contents = new byte[(ushort)(2 + blockExist.block.Length + blockAdd.block.Length)];
            byte multiplier = (byte)System.Math.Floor((blockCount + 1) / 256.0);

            contents[0] = (byte)(++blockCount);
            contents[1] = multiplier;
            for (ushort i = 2; i < blockExist.block.Length; i++)
                contents[i] = blockExist.block[i];

            if (blockExist.block.Length != 0)
                for (ushort i = 0; i < blockAdd.block.Length; i++)
                    contents[i + blockExist.block.Length] = blockAdd.block[i];
            else
                for (ushort i = 0; i < blockAdd.block.Length; i++)
                    contents[i + 2] = blockAdd.block[i];

            return new Block(0, contents);
        }

        public static void WriteItem(Item item, string pathHeap)
        {
            Block block = new Block();
            block.writeUInt16(item.id);
            block.writeUInt16(item.amount);
            block.writeByte(item.quality);
            block.writeUInt16((ushort)item.state.Length);
            foreach (byte bite in item.state)
                block.writeByte(bite);

            WriteBlock(pathHeap, UniteBlocks(block, pathHeap), true);
        }
    }
}
