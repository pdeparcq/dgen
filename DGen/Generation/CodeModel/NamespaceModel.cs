﻿using Guards;
using System.Collections.Generic;
using System.Linq;

namespace DGen.Generation.CodeModel
{
    public class NamespaceModel
    {
        public NamespaceModel Parent { get; }
        public string Name { get; }
        public string FullName => Parent == null ? Name : $"{Parent.FullName}.{Name}";
        public List<NamespaceModel> Namespaces { get; }
        public IEnumerable<NamespaceModel> AllNamespaces => Namespaces.SelectMany(ns => ns.Namespaces).Concat(Namespaces);
        public List<TypeModel> Types { get; set; }
        public IEnumerable<TypeModel> AllTypes => Namespaces.SelectMany(ns => ns.AllTypes).Concat(Types);
        public IReadOnlyCollection<ClassModel> Classes => Types.OfType<ClassModel>().ToList().AsReadOnly();
        public IReadOnlyCollection<InterfaceModel> Interfaces => Types.OfType<InterfaceModel>().Where(i => i.GetType() == typeof(InterfaceModel)).ToList().AsReadOnly();
        public IReadOnlyCollection<EnumerationModel> Enumerations => Types.OfType<EnumerationModel>().ToList().AsReadOnly();

        public NamespaceModel(NamespaceModel parent, string name)
        {
            Guard.ArgumentNotNullOrEmpty(() => name);

            Parent = parent;
            Name = name;
            Namespaces = new List<NamespaceModel>();
            Types = new List<TypeModel>();
        }

        public NamespaceModel AddNamespace(string name)
        {
            var ns = Namespaces.FirstOrDefault(n => n.Name == name);
            if(ns == null)
            {
                ns = new NamespaceModel(this, name);
                Namespaces.Add(ns);
            }
            return ns;
        }

        public ClassModel AddClass(string name)
        {
            var c = Classes.FirstOrDefault(c => c.Name == name);
            if (c == null)
            {
                c = new ClassModel(this, name);
                Types.Add(c);
            }
            return c;
        }

        public EnumerationModel AddEnumeration(string name)
        {
            var e = Enumerations.FirstOrDefault(e => e.Name == name);
            if (e == null)
            {
                e = new EnumerationModel(this, name);
                Types.Add(e);
            }
            return e;
        }

        public InterfaceModel AddInterface(string name)
        {
            var i = Interfaces.FirstOrDefault(i => i.Name == name);
            if (i == null)
            {
                i = new InterfaceModel(this, name);
                Types.Add(i);
            }
            return i;
        }

        public bool HasParent(NamespaceModel @namespace)
        {
            if (Parent == null)
                return false;

            return Parent == @namespace || Parent.HasParent(@namespace);
        }
    }
}
