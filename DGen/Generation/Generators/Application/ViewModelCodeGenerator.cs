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
                var builder = new ClassBuilder(context.SyntaxGenerator, context.Namespace, $"{viewModel.Name}ViewModel");
                
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
                if (property.Type.SystemType == null && !(property.Type.Type is Aggregate) && !property.IsCollection)
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
                        //TODO: generate viewmodel types
                        Type = new PropertyType { SystemType = property.Type.SystemType ?? $"{property.Type.Name}ViewModel" }
                    });
                }
            }
        }

        public string GetFileName(BaseType type)
        {
            return $"{type.Name}ViewModel.cs";
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
