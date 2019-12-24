using System.Collections.Generic;

namespace DGen.Generation.CodeModel
{
    public struct Literal
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }

    public class EnumerationModel : TypeModel
    {
        public List<Literal> Literals { get; }

        public EnumerationModel(NamespaceModel @namespace, string name)
            : base(@namespace, name)
        {
            Literals = new List<Literal>();
        }

        public Literal AddLiteral(string name)
        {
            var literal = new Literal
            {
                Name = name,
                Value = Literals.Count
            };
            Literals.Add(literal);
            return literal;
        }
    }
}
