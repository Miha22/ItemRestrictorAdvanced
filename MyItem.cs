using System.Runtime.Serialization;

namespace ItemRestrictorAdvanced
{
    [DataContract]
    class MyItem
    {
        [DataMember]
        ushort _id;
        [DataMember]
        byte _amount;
        [DataMember]
        byte _x;
        [DataMember]
        byte _quality;
        public MyItem()
        {

        }
        public MyItem(ushort id, byte amount, byte x, byte quality)
        {
            _id = id;
            _amount = amount;
            _x = x;
            _quality = quality;
        }
    }
}
