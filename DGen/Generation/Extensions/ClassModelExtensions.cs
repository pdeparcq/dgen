﻿using DGen.Generation.CodeModel;
using DGen.Meta;
using System.Linq;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Extensions
{
    public static class ClassModelExtensions
    {
        public static void AddDomainProperty(this ClassModel @class, Property property, ITypeModelRegistry registry)
        {
            @class.AddProperty(property.GetDomainName(), property.GetDomainType(registry))
                .WithDescription(property.Description)
                .MakeReadOnly();
        }

        public static void AddViewModelProperty(this ClassModel @class, Property property, ITypeModelRegistry registry)
        {
            TypeModel propertyType;

            if (property.Type.SystemType != null)
            {
                propertyType = SystemTypes.Parse(property.Type.SystemType);
            }
            else if (!(property.Type.Type is Enumeration))
            {
                var viewModel = registry.GetAllBaseTypes<ViewModel>("Application").SingleOrDefault(vm => vm.IsCompact && vm.Target == property.Type.Type);
                propertyType = registry.Resolve("Application", viewModel);
            }
            else
            {
                propertyType = property.Type.Resolve(registry);
            }

            if (property.IsCollection)
                propertyType = SystemTypes.List(propertyType);

            @class.AddProperty(property.Name, propertyType)
                .WithDescription(property.Description);
        }
    }
}
