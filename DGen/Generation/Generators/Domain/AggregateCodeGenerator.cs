using System.Collections.Generic;
using DGen.Generation.CodeModel;
using DGen.Meta;

namespace DGen.Generation.Generators.Domain
{
    public class AggregateCodeGenerator : ICodeModelGenerator
    {
        public string Layer => "Domain";

        public IEnumerable<BaseType> GetTypes(Module module)
        {
            return module.Aggregates;
        }

        public void Visit(Module module, NamespaceModel @namespace)
        {
            
        }

        public void Visit(BaseType type, NamespaceModel @namespace)
        {
            if (type is Aggregate aggregate)
            {
                var @class = @namespace.AddClass($"{aggregate.Name}");

                foreach(var p in aggregate.Properties)
                {
                    AddProperty(@class, p);
                }
            }
        }

        private static void AddProperty(ClassModel @class, Property property)
        {

            var propertyName = property.Name;
            TypeModel propertyType;

            if (property.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                if(!property.IsCollection)
                    propertyName = $"{propertyName}{aggregate.UniqueIdentifier.Name}";
                propertyType = SystemTypes.Parse(aggregate.UniqueIdentifier.Type.Name);
            }
            else
            {
                propertyType = SystemTypes.Parse(property.Type.Name);
            }
            
            if (property.IsCollection)
                propertyType = SystemTypes.GenericList(propertyType);

            @class.AddProperty(propertyName, propertyType).MakeReadOnly();
        }
    }
}
