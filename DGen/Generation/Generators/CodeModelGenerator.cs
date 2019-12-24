using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Generators.Application;
using DGen.Generation.Generators.Domain;
using DGen.Generation.Generators.Infrastructure;
using DGen.Meta;

namespace DGen.Generation.Generators
{
    public class CodeModelGenerator
    {
        private readonly List<ICodeModelGenerator> _generators;

        public CodeModelGenerator()
        {
            _generators = new List<ICodeModelGenerator>
            {
                // Domain
                new AggregateCodeGenerator(),
                new DomainEventCodeGenerator(),
                new Domain.EntityCodeGenerator(),
                new EnumerationCodeGenerator(),
                new ValueCodeGenerator(),
                // Infrastructure
                new DbContextCodeGenerator(),
                // Application
                new ViewModelCodeGenerator(),
                new QueryCodeGenerator(),
                new QueryHandlerCodeGenerator()
            };
        }

        public ApplicationModel Generate(MetaModel model)
        {
            var application = new ApplicationModel(model.Name);

            foreach (var service in model.Services)
            {
                var serviceModel = application.AddService(service.Name);

                foreach (var layer in _generators.GroupBy(g => g.Layer))
                {
                    var layerModel = serviceModel.AddLayer(layer.Key);

                    GenerateModule(service, layerModel, layer.ToList());
                }
            }

            return application;
        }

        private void GenerateModule(Module module, NamespaceModel model, IEnumerable<ICodeModelGenerator> generators)
        {

            foreach (var generator in generators)
            {
                generator.Visit(module, model);
                
                foreach(var type in generator.GetTypes(module))
                {
                    generator.Visit(type, model);
                }
            }

            module.Modules.ForEach(m =>
            {
                GenerateModule(m, model.AddNamespace(m.Name), generators);
            });
        }
    }
}
