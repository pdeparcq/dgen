using System.IO;
using Newtonsoft.Json;

namespace DGen.StarUml
{
    public class StarUmlReader
    {
        private readonly JsonSerializerSettings _settings;

        public StarUmlReader()
        {
            _settings = new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                ReferenceResolverProvider = () => new CustomReferenceResolver()
            };
        }

        public Element Read(string fileName)
        {
            var json = File.ReadAllText(fileName);
            json = json.Replace("_id", "$id");
            return JsonConvert.DeserializeObject<Element>(json, _settings);
        }
    }
}
