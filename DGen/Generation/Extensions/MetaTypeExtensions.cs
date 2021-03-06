﻿using DGen.Generation.CodeModel;
using DGen.Meta;
using DGen.Meta.MetaModel;

namespace DGen.Generation.Extensions
{
    public static class MetaTypeExtensions
    {
        public static TypeModel Resolve(this MetaType type, ITypeModelRegistry registry, string layer = "Domain")
        {
            if(type.SystemType != null)
            {
                return SystemTypes.Parse(type.SystemType);
            }
            else
            {
                return registry.Resolve(layer, type.Type) ?? SystemTypes.Parse(type.Name);
            }
        }
    }
}
