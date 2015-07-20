// This file is part of SINFONI.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.  If not, see <http://www.gnu.org/licenses/>.
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

            if (elementType.StartsWith("map"))
                result.elementType = MapParser.Instance.ParseMap(elementType);
            else if (elementType.StartsWith("array"))
                result.elementType = ArrayParser.Instance.ParseArray(elementType);
            else
                result.elementType = IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType(elementType.Trim());

            return result;
        }
    }
}
