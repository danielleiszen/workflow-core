using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowCore.Persistence.EntityFramework
{
    public class GrpcTimestampJsonConverter : JsonConverter<Timestamp>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override Timestamp ReadJson(JsonReader reader, System.Type objectType, Timestamp existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var converter = new ExpandoObjectConverter();

            if (existingValue == null)
            {
                string dateTime = reader.Value.ToString();

                if (string.IsNullOrEmpty(dateTime) != true)
                {
                    var parsedDate = DateTime.SpecifyKind(DateTime.Parse(dateTime), DateTimeKind.Utc);

                    existingValue = parsedDate.ToTimestamp();
                }                
            }

            if (existingValue != null)
            {
                object? o = converter.ReadJson(reader, objectType, existingValue, serializer);

                if (o != null)
                {
                    // Convert it back to json text.
                    string text = JsonConvert.SerializeObject(o);

                    return JsonParser.Default.Parse<Timestamp>(text);
                }
            }

            return new Timestamp();
        }        

        public override void WriteJson(JsonWriter writer, Timestamp value, JsonSerializer serializer)
        {
            var json = JsonFormatter.Default.Format(value);

            var jObject = JObject.Parse(json);

            jObject.AddFirst(new JProperty("$type", typeof(Timestamp).AssemblyQualifiedName));

            jObject.WriteTo(writer);
        }
    }
}
