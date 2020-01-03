using System;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.CodeModel
{
    public static class SystemTypes
    {
        private static NamespaceModel SystemNamespace = new NamespaceModel(null, "System");
        private static NamespaceModel SystemGenericCollectionsNamespace;
        private static NamespaceModel NewtonsoftJson = new NamespaceModel(null, "Newtonsoft").AddNamespace("Json");
        private static NamespaceModel KledexNamespace = new NamespaceModel(null, "Kledex");
        private static NamespaceModel KledexDomainNamespace;
        private static NamespaceModel KledexQueryNamespace;
        private static NamespaceModel KledexCommandNamespace;
        public static readonly ClassModel Guid;

        public static readonly string AggregateRootIdentifierName = "Id";
        public static readonly string DomainEventPublishMethodName = "AddAndApplyEvent";
        public static readonly string DomainEventApplyMethodName = "Apply";
        public static readonly string DomainEventAggregateRootIdentifierName = "AggregateRootId";

        static SystemTypes()
        {
            SystemGenericCollectionsNamespace = SystemNamespace.AddNamespace("Collections").AddNamespace("Generic");
            KledexDomainNamespace = KledexNamespace.AddNamespace("Domain");
            KledexQueryNamespace = KledexNamespace.AddNamespace("Queries");
            KledexCommandNamespace = KledexNamespace.AddNamespace("Commands");
            Guid = Parse("Guid");
        }

        public static ClassModel GenericList(TypeModel type)
        {
            return new ClassModel(SystemGenericCollectionsNamespace, "List").WithGenericTypes(type);
        }

        public static ClassModel Parse(string name)
        {
            return new ClassModel(SystemNamespace, name);
        }

        public static ClassModel NotImplementedException()
        {
            return new ClassModel(SystemNamespace, "NotImplementedException");
        }

        public static ClassModel JsonConstructorAttribute()
        {
            return new ClassModel(NewtonsoftJson, "JsonConstructor");
        }

        public static ClassModel AggregateRoot(TypeModel type)
        {
            var @class = new ClassModel(KledexDomainNamespace, "AggregateRoot");
            @class.AddMethod(DomainEventPublishMethodName);
            @class.AddProperty(AggregateRootIdentifierName, Parse("Guid"));
            return @class;
        }

        public static ClassModel DomainEvent(TypeModel type)
        {
            var @class = new ClassModel(KledexDomainNamespace, "DomainEvent");
            @class.AddProperty(DomainEventAggregateRootIdentifierName, Parse("Guid"));
            return @class;
        }

        public static ClassModel Query(TypeModel result)
        {
            return new ClassModel(KledexQueryNamespace, "IQuery").WithGenericTypes(result);
        }

        public static ClassModel QueryHandler(TypeModel query, TypeModel result)
        {
            return new ClassModel(KledexQueryNamespace, "IQueryHandler").WithGenericTypes(query, result);
        }

        internal static ClassModel Command()
        {
            return new ClassModel(KledexCommandNamespace, "Command");
        }

        public static ClassModel CommandHandler(TypeModel command)
        {
            return new ClassModel(KledexCommandNamespace, "ICommandHandler").WithGenericTypes(command);
        }

    }
}
