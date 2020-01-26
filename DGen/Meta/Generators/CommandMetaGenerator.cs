using System.Collections.Generic;
using System.Linq;
using DGen.Generation.Extensions;
using DGen.Meta.MetaModel;
using DGen.Meta.MetaModel.Types;
using DGen.StarUml;

namespace DGen.Meta.Generators
{
    public class CommandMetaGenerator : MetaGeneratorBase<Command>
    {
        public override string StereoType => "command";

        public override void Generate(Command command, Element element, ITypeRegistry registry)
        {
            base.Generate(command, element, registry);

            var service = GetDependency<Service>(element, registry);

            if(service != null)
            {
                command.Service = service.Value.Type;
                command.ServiceMethod = service.Value.Element.Name;

                var method = command.Service.Methods.SingleOrDefault(m => m.Name == command.ServiceMethod);

                // Try to create service method for corresponding aggregate method
                if(method == null && command.Service.AggregateRepository != null)
                {
                    method = command.Service.AggregateRepository.Methods.FirstOrDefault(m => m.Name == command.ServiceMethod);

                    if (method != null)
                    {
                        var serviceMethod = new MetaMethod(method.Name);

                        // Return aggregate
                        serviceMethod.AddParameter(new MetaParameter
                        {
                            IsReturn = true,
                            Type = new MetaType { Type = command.Service.AggregateRepository }
                        });

                        // Don't pass aggregate when the aggregate needs to be constructed...
                        if (!method.IsConstructor)
                        {
                            serviceMethod.AddParameter(new MetaParameter
                            {
                                Name = command.Service.AggregateRepository.Name.ToCamelCase(),
                                Type = new MetaType { Type = command.Service.AggregateRepository }
                            });
                        }        

                        foreach(var p in method.Parameters)
                        {
                            serviceMethod.AddParameter(p);
                        }

                        command.Service.Methods.Add(serviceMethod);
                    }
                }
            }
        }

        protected override bool ShouldGenerateProperty(BaseType resolved, string stereoType)
        {
            return base.ShouldGenerateProperty(resolved, stereoType) && !(resolved is InputModel);
        }
    }
}
