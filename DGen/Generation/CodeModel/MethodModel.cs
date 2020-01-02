using System;
using Guards;
using System.Collections.Generic;
using System.Linq;
using DGen.Generation.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGen.Generation.CodeModel
{
    public class MethodModel
    {
        public ClassModel Class { get; }
        public string Name { get; }
        public TypeModel ReturnType { get; private set; }
        public List<MethodParameter> Parameters { get; }
        public List<ClassModel> Attributes { get; }
        public List<StatementSyntax> Body { get; private set; }

        public IEnumerable<NamespaceModel> Usings
        {
            get
            {
                var usings = Attributes.Select(t => t.Namespace)
                    .Concat(Parameters.Select(p => p.Type.Namespace))
                    .ToList();

                if (ReturnType != null)
                    usings.Add(ReturnType.Namespace);

                return usings;
            }
        }

        public MethodModel(ClassModel @class, string name)
        {
            Guard.ArgumentNotNull(() => @class);
            Guard.ArgumentNotNullOrEmpty(() => name);

            Class = @class;
            Name = name;
            Parameters = new List<MethodParameter>();
            Attributes = new List<ClassModel>();
            Body = new List<StatementSyntax>();
        }

        /*
         * Assign properties from parameters with same name
         */

        public MethodModel WithReturnType(TypeModel returnType)
        {
            ReturnType = returnType;

            return this;
        }

        public MethodModel WithParameters(params MethodParameter[] parameters)
        {
            Parameters.AddRange(parameters);

            return this;
        }

        /*
         * Generate method parameters from class properties
         */
        public MethodModel WithPropertyParameters()
        {
            return WithParameters(Class.Properties.Select(p => new MethodParameter(p.Name.ToCamelCase(), p.Type))
                .ToArray());
        }

        public bool HasParameter(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return GetParameter(name, comparisonType) != null;
        }

        public MethodParameter GetParameter(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return Parameters.SingleOrDefault(p => p.Name.Equals(name, comparisonType));
        }

        public MethodModel WithAttributes(params ClassModel[] attributes)
        {
            Attributes.AddRange(attributes);

            return this;
        }

        public MethodModel WithBody(Action<BodyBuilder> build)
        {
            var builder = new BodyBuilder(this);
            build(builder);
            Body = builder.Statements;

            return this;
        }
    }
}
