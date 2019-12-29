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

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                return Layers.SelectMany(l => l.AllTypes).OfType<ClassModel>().SelectMany(c => c.Usings).Distinct();
            }
        }

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

        public IEnumerable<ServiceModel> Dependencies
        {
            get
            {
                var usings = Usings;
                foreach(var service in Application.Services.Except(new[] { this }))
                {
                    var namespaces = service.Layers.SelectMany(l => l.AllNamespaces);
                    if (usings.Any(u => namespaces.Contains(u)))
                    {
                        yield return service;
                    }
                }
            }
        }
    }
}
