
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
        [JsonProperty]
        public byte[] State { get; }
        [JsonProperty]
        public byte Page { get; }
        [JsonProperty]
        public byte Width { get; }
        [JsonProperty]
        public byte Height { get; }
        public byte Index { get; set; }

        public MyItem()
        {

        }
        public MyItem(ushort id, byte amount, byte quality, byte[] state, byte width, byte height, byte page)
        {
            ID = id;
            x = amount;
            Quality = quality;
            State = state;
            Count = 1;
            Page = page;
        }

    }
}
