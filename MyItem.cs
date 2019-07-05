
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
        //public static List<(byte, byte, byte)> Pages { get; set; } = new List<(byte, byte, byte)>();

        public MyItem()
        {
            
        }
        public MyItem(ushort id, byte amount, byte quality, byte[] state, byte rot, byte x, byte y)
        {
            Count = 1;
            ID = id;
            this.x = amount;
            Quality = quality;
            State = state;
            Rot = rot;
            ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
            Size_x = itemAsset.size_x;
            Size_y = itemAsset.size_y;
        }

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
