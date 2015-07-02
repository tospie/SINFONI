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
    public class ServiceFunctionDescription
    {
        public string Name { get; internal set; }
        public SinTDType ReturnType { get; internal set; }

        public Dictionary<string, SinTDType> Parameters
        {
            get { return parameters; }
            internal set { parameters = value; }
        }

        internal bool CanBeCalledWithParameters(object[] callParameters)
        {
            if (callParameters.Length != parameters.Count)
                return false;

            for (int i = 0; i < parameters.Values.Count; i++ )
            {
                if (!parameters.Values.ElementAt(i).CanBeAssignedFromType(callParameters[i].GetType()))
                    return false;
            }
            return true;
        }

        internal bool CanBeCalledWithReturnType(Type callReturnType)
        {
            return ReturnType.CanBeAssignedFromType(callReturnType);
        }

        internal ServiceFunctionDescription(string name, SinTDType returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        private Dictionary<string, SinTDType> parameters =
            new Dictionary<string,SinTDType>();
    }
}
