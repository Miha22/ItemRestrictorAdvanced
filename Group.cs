using System.Collections.Generic;
using System.Xml.Serialization;

namespace ItemRestrictorAdvanced
{
    public class Group
    {
        [XmlAttribute]
        public string GroupID { get; set; }
        [XmlAttribute("PlayerTotalVehicles")]
        public ushort PlayerTotalVehicles { get; set; }
        [XmlAttribute ("LimitItems")]
        public bool LimitItems = true;
        [XmlAttribute("LimitVehicles")]
        public bool LimitVehicles = true;
        [XmlArrayItem(ElementName = "ID")]
        public List<ushort> BlackListItems;
        [XmlArrayItem(ElementName = "ID")]
        public List<ushort> BlackListVehicles;
        public List<ItemBan> ItemLimits;
        public List<Vehicle> VehicleLimits;

        public Group()
        {

        }
    }
}
