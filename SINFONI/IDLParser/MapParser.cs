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

namespace SINFONI
{
    internal class MapParser
    {
        IDLParser idlParser;

        public MapParser(IDLParser idlParser)
        {
            this.idlParser = idlParser;
        }

        internal SinTDMap ParseMap(string mapDefinition)
        {
            int openingBracket = mapDefinition.IndexOf('<');
            int comma = mapDefinition.IndexOf(',');
            int closingBracket = mapDefinition.LastIndexOf('>');

            string keyType = mapDefinition.Substring(openingBracket + 1, comma - openingBracket - 1);
            string valueType = mapDefinition.Substring(comma + 1, closingBracket - comma - 1);

            SinTDMap result = new SinTDMap(getKeyOrValueType(keyType), getKeyOrValueType(valueType));
            return result;
        }

        internal SinTDType getKeyOrValueType(string typeName)
        {
            typeName = typeName.Trim();
            if (typeName.StartsWith("array"))
                return idlParser.ArrayParser.ParseArray(typeName);
            else if (typeName.StartsWith("map"))
                return ParseMap(typeName);
            else
                return idlParser.CurrentlyParsedSinTD.GetSinTDType(typeName);
        }
    }
}
