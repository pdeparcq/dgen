using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DGen.StarUml
{
    public class AttributeTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AttributeType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            var type = new AttributeType();

            if (reader.TokenType == JsonToken.String)
            {
                type.SystemType = (string)reader.Value;
            }
            else
            {
                var jObject = JObject.Load(reader);
                string id = (string)jObject["$ref"];
                if(id != null)
                    type.ReferenceType = serializer.ReferenceResolver.ResolveReference(serializer, id) as Element;
            }

            return type;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
