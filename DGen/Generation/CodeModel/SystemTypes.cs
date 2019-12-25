namespace DGen.Generation.CodeModel
{
    public static class SystemTypes
    {
        private static NamespaceModel SystemNamespace = new NamespaceModel(null, "System");
        private static NamespaceModel SystemGenericCollectionsNamespace;

        static SystemTypes()
        {
            SystemGenericCollectionsNamespace = SystemNamespace.AddNamespace("Generic").AddNamespace("Collections");
        }

        public static TypeModel GenericList(TypeModel type)
        {
            return new ClassModel(SystemGenericCollectionsNamespace, "List").WithGenericTypes(type);
        }

        public static TypeModel Parse(string name)
        {
            return new ClassModel(SystemNamespace, name);
        }
    }
}
