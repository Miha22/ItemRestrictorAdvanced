using System.Xml.Serialization;

namespace ItemRestrictor
{
    public class Vehicle
    {
        [XmlAttribute]
        public ushort Id { get; set; }
        [XmlAttribute("Limit")]
        public ushort Limit { get; set; }
        public static 

        public Vehicle()
        {

        }
        public Vehicle(ushort id, ushort limit)
        {
            this.Id = id;
            this.Limit = limit;
        }
    }
}
