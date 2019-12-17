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
        public ElementReference ReferenceType { get; set; }
    }

    public class ElementReference
    {
        public Element Element { get; set; }
    }

    public class Element
    {
        [JsonProperty(PropertyName = "_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "_parent", IsReference = true)]
        public ElementReference Parent { get; set; }

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
        public ElementReference Reference { get; set; }

        [JsonProperty(PropertyName = "source", IsReference = true)]
        public ElementReference Source { get; set; }

        [JsonProperty(PropertyName = "target", IsReference = true)]
        public ElementReference Target { get; set; }

        [JsonProperty(PropertyName = "end1")]
        public Element AssociationEndFrom { get; set; }

        [JsonProperty(PropertyName = "end2")]
        public Element AssociationEndTo { get; set; }

        [JsonProperty(PropertyName = "multiplicity")]
        public string Multiplicity { get; set; }


        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }

    }
}
