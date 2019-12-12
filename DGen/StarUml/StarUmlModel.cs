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

    public class Element
    {
        [JsonProperty(PropertyName = "_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "_parent", IsReference = true)]
        public Element Parent { get; set; }

        [JsonProperty(PropertyName = "ownedElements")]
        public List<Element> OwnedElements { get; set; }

        [JsonProperty(PropertyName = "stereotype")]
        public string Stereotype { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public List<Element> Attributes { get; set; }

        public string AttributeSimpleType { get; set; }

        [JsonProperty(PropertyName = "type", IsReference = true)]
        public Element AttributeElementType { get; set; }

        [JsonProperty(PropertyName = "literals")]
        public List<Element> Literals { get; set; }

        [JsonProperty(PropertyName = "reference", IsReference = true)]
        public Element Reference { get; set; }

        [JsonProperty(PropertyName = "end1")]
        public Element AssociationEndFrom { get; set; }

        [JsonProperty(PropertyName = "end2")]
        public Element AssociationEndTo { get; set; }

        [JsonProperty(PropertyName = "multiplicity")]
        public string Multiplicity { get; set; }


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (AttributeElementType == null)
            {
                AttributeSimpleType = "string";
            }
        }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }

    }
}
