using System.Xml.Serialization;

namespace ItemRestrictor
{
    public class ItemBan
    {
        [XmlAttribute]
        public ushort Id { get; set; }
        [XmlAttribute("Limit")]
        public ushort Limit { get; set; }

        public ItemBan()
        {

        }
        public ItemBan(ushort id, ushort limit)
        {
            this.Id = id;
            this.Limit = limit;
        }
    }
}
