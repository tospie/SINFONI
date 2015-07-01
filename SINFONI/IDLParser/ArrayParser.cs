using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SINFONI
{
    internal class ArrayParser
    {
        internal static ArrayParser Instance = new ArrayParser();

        internal SinTDArray ParseArray(string arrayDefinition)
        {
            SinTDArray result = new SinTDArray();

            int indexStart = arrayDefinition.IndexOf('<') + 1;
            int indexEnd = arrayDefinition.LastIndexOf ('>');
            string elementType = arrayDefinition.Substring(indexStart, indexEnd - indexStart);

            if (elementType.Contains("map"))
                result.elementType = MapParser.Instance.ParseMap(elementType);
            else if (elementType.Contains("array"))
                result.elementType = ArrayParser.Instance.ParseArray(elementType);
            else
                result.elementType = IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType(elementType.Trim());

            return result;
        }
    }
}
