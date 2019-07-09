
using Newtonsoft.Json;
using SDG.Framework.IO.Serialization;
using System;
using System.Collections;
using System.IO;

namespace ItemRestrictorAdvanced
{
    public class MyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize(reader, objectType);
            }
            return new byte[] { byte.Parse(reader.Value.ToString()) };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable array = (IEnumerable)value;
            writer.WriteStartObject();
            //writer.WritePropertyName("Page");
            //writer.WriteValue("Add");
            writer.WritePropertyName("Attachments");
            writer.WriteStartArray();
            foreach (object item in array)
            {
                serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
