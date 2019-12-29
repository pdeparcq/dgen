using Guards;
using System.Collections.Generic;

namespace DGen.Generation.CodeModel
{
    public class MethodParameter
    {
        public string Name { get; }
        public TypeModel Type { get; }

        public MethodParameter(string name, TypeModel type)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);
            Guard.ArgumentNotNull(() => type);

            Name = name;
            Type = type;
        }
    }

    public class MethodModel
    {
        public string Name { get; }
        public TypeModel ReturnType { get; private set; }
        public List<MethodParameter> Parameters { get; }
        public List<ClassModel> Attributes { get; }

        public MethodModel(string name)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);

            Name = name;
            Parameters = new List<MethodParameter>();
            Attributes = new List<ClassModel>();
        }

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

        public MethodModel WithAttributes(params ClassModel[] attributes)
        {
            Attributes.AddRange(attributes);

            return this;
        }
    }
}
