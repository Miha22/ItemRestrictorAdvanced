
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
        public byte[] State { get; }

        public MyItem()
        {

        }
        public MyItem(ushort id, byte amount, byte quality, byte[] state)
        {
            ID = id;
            x = amount;
            Quality = quality;
            State = state;
            Count = 1;
        }
    }
}
