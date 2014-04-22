using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KIARA
{
    internal class ArrayParser
    {
        internal static ArrayParser Instance = new ArrayParser();

        internal KtdArray ParseArray(string arrayDefinition)
        {
            KtdArray result = new KtdArray();

            int indexStart = arrayDefinition.IndexOf('<') + 1;
            int indexEnd = arrayDefinition.IndexOf('>');
            string elementType = arrayDefinition.Substring(indexStart, indexEnd - indexStart);
            
            result.elementType = KTD.Instance.GetType(elementType.Trim());
            return result;
        }
    }
}
