namespace DGen.Generation.CodeModel
{
    public static class SystemTypes
    {
        private static NamespaceModel SystemNamespace = new NamespaceModel(null, "System");
        private static NamespaceModel SystemGenericCollectionsNamespace;
        private static NamespaceModel KledexNamespace = new NamespaceModel(null, "Kledex");
        private static NamespaceModel KledexDomainNamespace;
        private static NamespaceModel KledexQueryNamespace;

        static SystemTypes()
        {
            SystemGenericCollectionsNamespace = SystemNamespace.AddNamespace("Generic").AddNamespace("Collections");
            KledexDomainNamespace = KledexNamespace.AddNamespace("Domain");
            KledexQueryNamespace = KledexNamespace.AddNamespace("Queries");
        }

        public static TypeModel GenericList(TypeModel type)
        {
            return new ClassModel(SystemGenericCollectionsNamespace, "List").WithGenericTypes(type);
        }

        public static TypeModel Parse(string name)
        {
            return new ClassModel(SystemNamespace, name);
        }

        public static ClassModel AggregateRoot(TypeModel type)
        {
            return new ClassModel(KledexDomainNamespace, "AggregateRoot");
        }

        public static ClassModel Query(TypeModel result)
        {
            return new ClassModel(KledexQueryNamespace, "IQuery").WithGenericTypes(result);
        }

        public static ClassModel QueryHandler(TypeModel query, TypeModel result)
        {
            return new ClassModel(KledexQueryNamespace, "IQueryHandler").WithGenericTypes(query, result);
        }
    }
}
