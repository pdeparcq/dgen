using System.Collections.Generic;
using System.Linq;
using DGen.Generation.CodeModel;
using DGen.Generation.Generators.Application;
using DGen.Generation.Generators.Domain;
using DGen.Generation.Generators.Infrastructure;
using DGen.Meta;

namespace DGen.Generation.Generators
{
    public class CodeModelGenerator : ITypeModelRegistry
    {
        private readonly List<ICodeModelGenerator> _generators;
        private Dictionary<string, Dictionary<BaseType, TypeModel>> _types;

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
            _types = new Dictionary<string, Dictionary<BaseType, TypeModel>>();

            var application = new ApplicationModel(model.Name);

            foreach (var service in model.Services)
            {
                var serviceModel = application.AddService(service.Name);

                foreach (var layer in _generators.GroupBy(g => g.Layer))
                {
                    _types[layer.Key] = new Dictionary<BaseType, TypeModel>();

                    var layerModel = serviceModel.AddLayer(layer.Key);

                    PrepareModule(service, layerModel, layer.ToList());
                    GenerateModule(service, layerModel, layer.ToList());
                }
            }

            return application;
        }

        public void Register(string layer, BaseType type, TypeModel model)
        {
            _types[layer][type] = model;
        }

        public TypeModel Resolve(string layer, BaseType type)
        {
            if(_types.ContainsKey(layer) && _types[layer].ContainsKey(type))
            {
                return _types[layer][type];
            }
            return null;
        }


        private void PrepareModule(Module module, NamespaceModel @namespace, IEnumerable<ICodeModelGenerator> generators)
        {

            foreach (var generator in generators)
            {
                foreach (var type in generator.GetTypes(module))
                {
                    var model = generator.PrepareType(type, @namespace);
                    if(model != null)
                    {
                        Register(generator.Layer, type, model);
                    }                        
                }
            }

            module.Modules.ForEach(m =>
            {
                PrepareModule(m, @namespace.AddNamespace(m.Name), generators);
            });
        }

        private void GenerateModule(Module module, NamespaceModel model, IEnumerable<ICodeModelGenerator> generators)
        {

            foreach (var generator in generators)
            {
                generator.GenerateModule(module, model, this);
                
                foreach(var type in generator.GetTypes(module))
                {
                    var resolved = Resolve(generator.Layer, type);
                    if(resolved != null)
                    {
                        generator.GenerateType(type, resolved, this);
                    }  
                }
            }

            module.Modules.ForEach(m =>
            {
                GenerateModule(m, model.AddNamespace(m.Name), generators);
            });
        }
    }
}
