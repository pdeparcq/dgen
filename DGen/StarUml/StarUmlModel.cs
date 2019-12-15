using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DGen.StarUml
{
    public enum ElementType
    {
        Project,
        UMLModel,
        UMLPackage,
        UMLClass,
        UMLAssociation,
        UMLAssociationEnd,
        UMLDependency,
        UMLEnumeration,
        UMLEnumerationLiteral,
        UMLClassDiagram,
        UMLAttribute
    }

    public class AttributeType
    {
        public string SystemType { get; set; }
        public Element ReferenceType { get; set; }
    }

    public class Element
    {
        [JsonProperty(PropertyName = "_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public string FullName => Parent == null ? Name : $"{Parent.Name}.{Name}";

        [JsonProperty(PropertyName = "_parent", IsReference = true)]
        public Element Parent { get; set; }

        [JsonProperty(PropertyName = "ownedElements")]
        public List<Element> OwnedElements { get; set; }

        [JsonProperty(PropertyName = "stereotype")]
        public string Stereotype { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public List<Element> Attributes { get; set; }

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(AttributeTypeConverter))]
        public AttributeType AttributeType { get; set; }

        [JsonProperty(PropertyName = "literals")]
        public List<Element> Literals { get; set; }

        [JsonProperty(PropertyName = "reference", IsReference = true)]
        public Element Reference { get; set; }

        [JsonProperty(PropertyName = "source", IsReference = true)]
        public Element Source { get; set; }

        [JsonProperty(PropertyName = "target", IsReference = true)]
        public Element Target { get; set; }

        [JsonProperty(PropertyName = "end1")]
        public Element AssociationEndFrom { get; set; }

        [JsonProperty(PropertyName = "end2")]
        public Element AssociationEndTo { get; set; }

        [JsonProperty(PropertyName = "multiplicity")]
        public string Multiplicity { get; set; }


        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            System.Console.Error.WriteLine(errorContext.Error.Message);
            errorContext.Handled = true;
        }

    }
}
