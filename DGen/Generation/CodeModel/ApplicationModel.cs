using Guards;
using System.Collections.Generic;

namespace DGen.Generation.CodeModel
{
    public class ApplicationModel
    {
        public string Name { get; }
        public List<ServiceModel> Services { get; }

        public ApplicationModel(string name)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);

            Name = name;
            Services = new List<ServiceModel>();
        }

        public ServiceModel AddService(string name)
        {
            var service = new ServiceModel(this, name);
            Services.Add(service);
            return service;
        }
    }
}
