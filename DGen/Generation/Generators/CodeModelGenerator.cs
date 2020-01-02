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
        private readonly List<ILayerCodeGenerator> _generators;
        private Dictionary<string, Dictionary<BaseType, List<TypeModel>>> _types;

        public CodeModelGenerator()
        {
            _generators = new List<ILayerCodeGenerator>
            {
                // Domain
                new DomainEventCodeGenerator(),
                new AggregateCodeGenerator(),
                new Domain.EntityCodeGenerator(),
                new EnumerationCodeGenerator(),
                new ValueCodeGenerator(),
                // Infrastructure
                new DbContextCodeGenerator(),
                // Application
                new ViewModelCodeGenerator(),
                new CompactViewModelCodeGenerator(),
                new QueryCodeGenerator(),
                new QueryHandlerCodeGenerator()
            };
        }

        public ApplicationModel Generate(MetaModel model)
        {
            var application = new ApplicationModel(model.Name);
            PrepareServices(model, application);
            GenerateServices(model, application);
            return application;
        }

        private void PrepareServices(MetaModel model, ApplicationModel application)
        {
            _types = new Dictionary<string, Dictionary<BaseType, List<TypeModel>>>();
            foreach (var service in model.Services)
            {
                var serviceModel = application.AddService(service.Name);

                foreach (var layer in _generators.GroupBy(g => g.Layer))
                {
                    if(!_types.ContainsKey(layer.Key))
                        _types[layer.Key] = new Dictionary<BaseType, List<TypeModel>>();
                    PrepareModule(service, serviceModel.AddLayer(layer.Key), layer.ToList());
                }
            }
        }

        private void GenerateServices(MetaModel model, ApplicationModel application)
        {
            foreach (var service in model.Services)
            {
                var serviceModel = application.GetService(service.Name);

                foreach (var layer in _generators.GroupBy(g => g.Layer))
                {
                    GenerateModule(service, serviceModel.GetLayer(layer.Key), layer.ToList());
                }
            }
        }

        private void PrepareModule(Module module, NamespaceModel @namespace, IEnumerable<ILayerCodeGenerator> generators)
        {

            foreach (var generator in generators)
            {
                foreach (var type in generator.GetTypes(module))
                {
                    var name = generator.GetTypeName(type);
                    if(name != null)
                    {
                        var model = generator.PrepareType(@namespace, type);
                        Register(generator.Layer, type, model.WithDescription(type.Description));
                    }
                }
            }

            module.Modules?.ForEach(m =>
            {
                PrepareModule(m, @namespace.AddNamespace(m.Name), generators);
            });
        }

        private void GenerateModule(Module module, NamespaceModel model, IEnumerable<ILayerCodeGenerator> generators)
        {

            foreach (var generator in generators)
            {
                generator.GenerateModule(module, model, this);
                
                foreach(var type in generator.GetTypes(module))
                {
                    var resolved = Resolve(generator.Layer, type, generator.GetTypeName(type));
                    if(resolved != null)
                    {
                        generator.GenerateType(type, resolved, this);
                    }  
                }
            }

            module.Modules?.ForEach(m =>
            {
                GenerateModule(m, model.AddNamespace(m.Name), generators);
            });
        }

        public void Register(string layer, BaseType type, TypeModel model)
        {
            if (!_types[layer].ContainsKey(type))
            {
                _types[layer][type] = new List<TypeModel>();
            }
            _types[layer][type].Add(model);
        }

        public TypeModel Resolve(string layer, BaseType type, string name = null)
        {
            if (_types.ContainsKey(layer) && _types[layer].ContainsKey(type))
            {
                var types = _types[layer][type];
                if (name != null)
                    return types.FirstOrDefault(t => t.Name == name);
                else
                    return types.FirstOrDefault();
            }
            return null;
        }

        public IQueryable<T> GetAllBaseTypes<T>(string layer) where T : BaseType
        {
            return _types[layer].Keys.OfType<T>().AsQueryable();
        }
    }
}
