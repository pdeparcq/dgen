using Guards;
using System.Collections.Generic;
using System.Linq;

namespace DGen.Generation.CodeModel
{
    public class ServiceModel
    {
        public ApplicationModel Application { get; }
        public string Name { get; }
        public List<NamespaceModel> Layers { get; }

        public ServiceModel(ApplicationModel application, string name)
        {
            Guard.ArgumentNotNull(() => application);
            Guard.ArgumentNotNullOrEmpty(() => name);

            Application = application;
            Name = name;
            Layers = new List<NamespaceModel>();
        }

        public NamespaceModel GetLayer(string layerName)
        {
            return Layers.Single(l => l.Name == $"{Application.Name}.{Name}.{layerName}");
        }

        public NamespaceModel AddLayer(string layerName)
        {
            var layer = new NamespaceModel(null, $"{Application.Name}.{Name}.{layerName}");
            Layers.Add(layer);
            return layer;
        }
    }
}
