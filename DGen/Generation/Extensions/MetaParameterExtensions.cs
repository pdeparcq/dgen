using DGen.Generation.CodeModel;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.Extensions
{
    public static class MetaParameterExtensions
    {
        public static string GetDomainName(this MetaParameter parameter)
        {

            if (parameter.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                if (!parameter.IsCollection)
                    return $"{parameter.Name}{aggregate.UniqueIdentifier.Name}";
            }
            return parameter.Name;
        }

        public static TypeModel GetDomainType(this MetaParameter parameter, ITypeModelRegistry registry)
        {
            TypeModel parameterType;

            if (parameter.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
            {
                parameterType = aggregate.UniqueIdentifier.Type.Resolve(registry);
            }
            else
            {
                parameterType = parameter.Type.Resolve(registry);
            }

            if (parameter.IsCollection)
                return SystemTypes.List(parameterType);

            return parameterType;
        }
    }
}
