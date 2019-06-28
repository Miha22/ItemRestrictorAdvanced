
namespace ItemRestrictorAdvanced
{
    class MyItem
    {
        ushort _id;
        byte _amount;
        byte _x;
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
        
        public ushort ID
        {
            get
            {
                return _id;
            }
        }
        public byte Amount
        {
            get
            {
                return _amount;
            }
        }
        public byte x
        {
            get
            {
                return _x;
            }
        }
        public byte Quality
        {
            get
            {
                return _quality;
            }
        }

    }
}
