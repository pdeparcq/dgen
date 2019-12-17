using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace DGen.StarUml
{
    public class CustomReferenceResolver : IReferenceResolver
    {

        private readonly Dictionary<string, ElementReference> _references;

        public CustomReferenceResolver()
        {
            _references = new Dictionary<string, ElementReference>();
        }

        public object ResolveReference(object context, string reference)
        {
            if (!_references.ContainsKey(reference))
            {
                _references[reference] = new ElementReference();
            }
            return _references[reference];
        }

        public string GetReference(object context, object value)
        {
            return string.Empty;
        }

        public bool IsReferenced(object context, object value)
        {
            return true;
        }

        public void AddReference(object context, string reference, object value)
        {
            if(ResolveReference(context, reference) is ElementReference resolved)
                resolved.Element = value as Element;
        }
    }
}
