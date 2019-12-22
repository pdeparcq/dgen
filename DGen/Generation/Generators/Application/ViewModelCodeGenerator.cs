using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis;

namespace DGen.Generation.Generators.Application
{
    public class ViewModelCodeGenerator : ICodeGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("ViewModels");
        }

        public SyntaxNode Generate(CodeGenerationContext context)
        {
            if(context.Type is ViewModel viewModel)
            {
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, viewModel.Name);
                
                if(viewModel.Target != null)
                {
                    GenerateProperties("", viewModel.Target.Properties, viewModel);
                }

                viewModel.GenerateProperties(builder);

                return builder.Build();
            }
            return null;
        }


        private void GenerateProperties(string parent, IEnumerable<Property> properties, ViewModel viewModel)
        {
            foreach (var property in properties)
            {
                if (property.IsCollection)
                {
                    if (viewModel.IsCompact && property.Type.Type is Entity)
                    {
                        viewModel.Properties.Add(new Property
                        {
                            Name = $"{parent}NumberOf{property.Name}",
                            Type = new PropertyType { SystemType = "int" }
                        });
                    }
                    else
                    {
                        viewModel.Properties.Add(new Property
                        {
                            Name = $"{parent}{property.Name}",
                            IsCollection = true,
                            //TODO: create real viewmodel for property type
                            Type = new PropertyType { SystemType = $"{property.Type.Name}Overview" }
                        });
                    }
                }
                else
                {
                    GenerateProperty(parent, property, viewModel);
                }
            }
        }

        private void GenerateProperty(string parent, Property property, ViewModel viewModel)
        {
            if (property.Type.SystemType == null && !(property.Type.Type is Aggregate))
            {
                // If only one property with systemtype, use system type without appending the name
                if (property.Type.Type.Properties.Count == 1 
                    && !property.Type.Type.Properties.First().IsCollection 
                    && property.Type.Type.Properties.First().Type.SystemType != null)
                {
                    viewModel.Properties.Add(new Property
                    {
                        Name = $"{parent}{property.Name}",
                        Type = property.Type.Type.Properties.First().Type
                    });
                }
                else
                {
                    GenerateProperties($"{parent}{property.Name}", property.Type.Type.Properties, viewModel);
                }
            }
            else
            {
                viewModel.Properties.Add(new Property
                {
                    Name = $"{parent}{property.Name}",
                    IsCollection = property.IsCollection,
                    Type = property.Type
                });
            }
        }  

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}.cs";
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public IEnumerable<BaseType> GetTypesFromModule(Module module)
        {
            return module.ViewModels;
        }
    }
}
