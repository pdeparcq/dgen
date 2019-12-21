﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DGen.Generation.Helpers;
using DGen.Meta;
using Microsoft.CodeAnalysis.Editing;

namespace DGen.Generation.Generators.Application
{
    public class QueryCodeGenerator : ICodeGenerator
    {
        public string Layer => "Application";

        public DirectoryInfo CreateSubdirectory(DirectoryInfo di)
        {
            return di.CreateSubdirectory("Queries");
        }

        public async Task Generate(string @namespace, Module module, BaseType type, StreamWriter sw, SyntaxGenerator syntaxGenerator)
        {
            if (type is Query query)
            {
                var builder = new ClassBuilder(syntaxGenerator, @namespace, query.Name);
                builder.AddBaseType(query.IsCollection ? $"Query<IEnumerable<{query.Result.Name}>>" : $"Query<{query.Result.Name}>");
                query.GenerateProperties(builder);
                await sw.WriteAsync(builder.ToString());
            }
        }

        public string GetFileName(BaseType type)
        {
            if (type is Query query && query.Result != null)
                return $"{query.Name}.cs";
            return null;
        }

        public string GetFileNameForModule(Module module)
        {
            return null;
        }

        public IEnumerable<BaseType> GetListFromModule(Module module)
        {
            return module.Queries;
        }
    }
}
