using System.Xml.Serialization;

namespace ItemRestrictorAdvanced
{
    public class Vehicle
    {
        [XmlAttribute]
        public ushort Id { get; set; }
        [XmlAttribute("Limit")]
        public ushort Limit { get; set; }
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
