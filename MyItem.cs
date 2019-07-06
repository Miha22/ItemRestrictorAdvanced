
using Newtonsoft.Json;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    [JsonObject(MemberSerialization.OptIn)]
    class MyItem
    {
        [JsonProperty]
        public ushort ID { get; set; }
        [JsonProperty]
        public byte Count { get; set; }
        [JsonProperty]
        public byte x { get; set; }
        [JsonProperty]
        public byte Quality { get; set; }
        public byte[] State { get; set; }
        public byte Rot { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Size_x { get; set; }
        public byte Size_y { get; set; }
        public byte Index { get; set; }
        [JsonProperty]
        public byte Width { get; set; }
        [JsonProperty]
        public byte Height { get; set; }

        public MyItem()
        {
            
        }
        public MyItem(ushort id, byte amount, byte quality, byte[] state, byte rot, byte x, byte y, byte index, byte width, byte height)
        {
            Count = 1;
            ID = id;
            this.x = amount;
            Quality = quality;
            State = state;
            Rot = rot;
            X = x;
            Y = y;
            Index = index;
            Width = width;
            Height = height;
            ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
            Size_x = itemAsset.size_x;
            Size_y = itemAsset.size_y;
        }
        //private bool HasIndex(ref byte[,] Pages, ushort index)
        //{
        //    for (byte i = 0; i < Pages.Length; i++)
        //    {
        //        if (Pages[i, 0] == index)
        //            return true;
        //    }

        //    return false;
        //}
    }
    class MyItemComparer : IComparer<MyItem>
    {
        public int Compare(MyItem item1, MyItem item2)
        {
            if ((item1.Size_x * item1.Size_y) > (item2.Size_x * item2.Size_y))
                return 1;
            else if ((item1.Size_x * item1.Size_y) < (item2.Size_x * item2.Size_y))
                return -1;
            else
                return 0;
        }
    }
}
