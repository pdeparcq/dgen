using System;
using DGen.Meta.MetaModel.Types;

namespace DGen.Generation.CodeModel
{
    public static class SystemTypes
    {
        private static NamespaceModel SystemNamespace = new NamespaceModel(null, "System");
        private static NamespaceModel SystemGenericCollectionsNamespace;
        private static NamespaceModel SystemLinqNamespace;
        private static NamespaceModel SystemThreadingTasksNamespace;
        private static NamespaceModel EntitityFrameworkCoreNamespace = new NamespaceModel(null, "Microsoft").AddNamespace("EntityFrameworkCore");
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
            SystemLinqNamespace = SystemNamespace.AddNamespace("Linq");
            SystemThreadingTasksNamespace = SystemNamespace.AddNamespace("Threading").AddNamespace("Tasks");
            KledexDomainNamespace = KledexNamespace.AddNamespace("Domain");
            KledexQueryNamespace = KledexNamespace.AddNamespace("Queries");
            KledexCommandNamespace = KledexNamespace.AddNamespace("Commands");
            Guid = Parse("Guid");
        }

        public static TypeModel DbSet(TypeModel type)
        {
            return new ClassModel(EntitityFrameworkCoreNamespace, "DbSet").WithGenericTypes(type);
        }

        public static ClassModel List(TypeModel type)
        {
            return new ClassModel(SystemGenericCollectionsNamespace, "List").WithGenericTypes(type);
        }

        public static ClassModel Enumerable(TypeModel type)
        {
            return new ClassModel(SystemGenericCollectionsNamespace, "IEnumerable").WithGenericTypes(type);
        }

        public static ClassModel Task(TypeModel type = null)
        {
            var @class = new ClassModel(SystemThreadingTasksNamespace, "Task");

            if (type != null)
                @class = @class.WithGenericTypes(type);

            return @class;
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

        public static InterfaceModel DomainEvent()
        {
            return new InterfaceModel(KledexDomainNamespace, "IDomainEvent");
        }

        public static ClassModel DomainEvent(TypeModel type)
        {
            var @class = new ClassModel(KledexDomainNamespace, "DomainEvent");
            @class.AddProperty(DomainEventAggregateRootIdentifierName, Parse("Guid"));
            return @class;
        }

        public static InterfaceModel Repository(TypeModel aggregate)
        {
            return new InterfaceModel(KledexDomainNamespace, "IRepository").WithGenericTypes(aggregate);
        }

        public static InterfaceModel Queryable(TypeModel entity)
        {
            return new InterfaceModel(SystemLinqNamespace, "IQueryable").WithGenericTypes(entity);
        }

        public static ClassModel Query(TypeModel result)
        {
            return new ClassModel(KledexQueryNamespace, "IQuery").WithGenericTypes(result);
        }

        public static InterfaceModel QueryHandler(TypeModel query, TypeModel result)
        {
            return new InterfaceModel(KledexQueryNamespace, "IQueryHandler").WithGenericTypes(query, result);
        }

        internal static ClassModel Command()
        {
            return new ClassModel(KledexCommandNamespace, "Command");
        }

        internal static ClassModel CommandResponse()
        {
            return new ClassModel(KledexCommandNamespace, "CommandResponse");
        }

        public static InterfaceModel CommandHandler(TypeModel command)
        {
            return new InterfaceModel(KledexCommandNamespace, "ICommandHandler").WithGenericTypes(command);
        }

    }
}
