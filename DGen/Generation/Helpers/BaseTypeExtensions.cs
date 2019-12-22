using DGen.Meta;

namespace DGen.Generation.Helpers
{
    public static class BaseTypeExtensions
    {
        public static void GenerateProperties(this BaseType t, ClassBuilder builder, bool readOnly = false)
        {
            t.Properties?.ForEach(p =>
            {
                if (p.IsCollection)
                    builder.AddNamespaceImportDeclaration("System.Collections.Generic");

                if (p.Type.Type is Aggregate aggregate && aggregate.UniqueIdentifier != null)
                {
                    GenerateProperty(new Property
                    {
                        IsCollection = p.IsCollection,
                        Name = p.IsCollection ? p.Name : $"{p.Name}{aggregate.UniqueIdentifier.Name}",
                        Type = aggregate.UniqueIdentifier.Type
                    }, builder, readOnly);
                }
                else
                {
                    GenerateProperty(p, builder, readOnly);
                }
            });
        }

        private static void GenerateProperty(Property p, ClassBuilder builder, bool readOnly)
        {
            builder.AddAutoProperty(p.Name, p.IsCollection ? $"List<{p.Type.Name}>" : p.Type.Name, readOnly);
        }
    }
}
