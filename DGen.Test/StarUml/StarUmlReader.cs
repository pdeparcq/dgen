using Newtonsoft.Json;
using System.IO;

namespace DGen.Test.StarUml
{
    public class StarUmlReader
    {
        private readonly JsonSerializerSettings _settings;

        public StarUmlReader()
        {
            _settings = new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
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
