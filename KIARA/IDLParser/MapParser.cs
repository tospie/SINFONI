﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI
{
    internal class MapParser
    {
        internal static MapParser Instance = new MapParser();

        internal KtdMap ParseMap(string mapDefinition)
        {
            int openingBracket = mapDefinition.IndexOf('<');
            int comma = mapDefinition.IndexOf(',');
            int closingBracket = mapDefinition.LastIndexOf('>');

            string keyType = mapDefinition.Substring(openingBracket + 1, comma - openingBracket - 1);
            string valueType = mapDefinition.Substring(comma + 1, closingBracket - comma - 1);

            KtdMap result = new KtdMap();
            result.keyType = IDLParser.Instance.CurrentlyParsedKTD.GetKtdType(keyType.Trim());
            result.elementType = IDLParser.Instance.CurrentlyParsedKTD.GetKtdType(valueType.Trim());

            return result;
        }
    }
}
