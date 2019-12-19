using DGen.Meta;

namespace DGen.Generation.Domain
{
    public static class BaseTypeExtensions
    {
        public static void GenerateProperties(this BaseType t, ClassBuilder builder)
        {
            t.Properties?.ForEach(p =>
            {
                if (p.IsCollection)
                    builder.AddNamespaceImportDeclaration("System.Collections.Generic");
                builder.AddAutoProperty(p.Name, p.IsCollection ? $"List<{p.Type.Name}>" : p.Type.Name);
            });
        }
    }
}
