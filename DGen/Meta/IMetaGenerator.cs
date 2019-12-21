﻿using System.Collections.Generic;
using DGen.StarUml;

namespace DGen.Meta
{
    public interface IMetaGenerator
    {
        System.Type GeneratedType { get; }
        void GenerateTypes(Element parent, Module module, ITypeRegistry registry);
        void Generate(BaseType type, Element e, ITypeRegistry registry); 
    }
}