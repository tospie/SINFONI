﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class MapParser
    {
        public KtdMap ParseMap(string mapDefinition)
        {
            int openingBracket = mapDefinition.IndexOf('<');
            int comma = mapDefinition.IndexOf(',');
            int closingBracket = mapDefinition.IndexOf('>');

            string keyType = mapDefinition.Substring(openingBracket + 1, comma - openingBracket - 1);
            string valueType = mapDefinition.Substring(comma + 1, closingBracket - comma - 1);

            KtdMap result = new KtdMap();
            result.keyType = KTD.Instance.GetType(keyType);
            result.elementType = KTD.Instance.GetType(valueType);

            return result;
        }
    }
}
