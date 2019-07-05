
using Newtonsoft.Json;
using SDG.Unturned;
using System.Collections.Generic;

namespace ItemRestrictorAdvanced
{
    [JsonObject(MemberSerialization.OptIn)]
    class MyItem
    {
        [JsonProperty]
        public ushort ID { get; }
        [JsonProperty]
        public byte Count { get; set; }
        [JsonProperty]
        public byte x { get; }
        [JsonProperty]
        public byte Quality { get; }
        public byte[] State { get; set; }
        public byte Rot { get; }
        public byte X { get;}
        public byte Y { get; }
        public byte Size_x { get; }
        public byte Size_y { get; }
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
}
