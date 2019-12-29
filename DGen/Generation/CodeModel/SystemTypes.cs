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
            SystemGenericCollectionsNamespace = SystemNamespace.AddNamespace("Collections").AddNamespace("Generic");
            KledexDomainNamespace = KledexNamespace.AddNamespace("Domain");
            KledexQueryNamespace = KledexNamespace.AddNamespace("Queries");
        }

        public static ClassModel GenericList(TypeModel type)
        {
            return new ClassModel(SystemGenericCollectionsNamespace, "List").WithGenericTypes(type);
        }

        public static ClassModel Parse(string name)
        {
            return new ClassModel(SystemNamespace, name);
        }

        public static ClassModel AggregateRoot(TypeModel type)
        {
            return new ClassModel(KledexDomainNamespace, "AggregateRoot");
        }

        public static ClassModel DomainEvent(TypeModel type)
        {
            return new ClassModel(KledexDomainNamespace, "DomainEvent");
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
