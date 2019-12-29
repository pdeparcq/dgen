using Guards;
using System.Collections.Generic;
using System.Linq;

namespace DGen.Generation.CodeModel
{
    public class ServiceModel
    {
        public ApplicationModel Application { get; }
        public string Name { get; }
        public NamespaceModel RootNamespace { get; }
        public IEnumerable<NamespaceModel> Layers => RootNamespace.Namespaces;

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
            RootNamespace = new NamespaceModel(null, $"{application.Name}.{Name}");
        }

        public NamespaceModel GetLayer(string layerName)
        {
            return Layers.Single(l => l.Name == layerName);
        }

        public NamespaceModel AddLayer(string layerName)
        {
            return RootNamespace.AddNamespace(layerName);
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
