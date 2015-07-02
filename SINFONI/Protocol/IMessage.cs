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
    public enum MessageType
    {
        REQUEST,
        RESPONSE,
        EXCEPTION
    }

    public interface IMessage
    {
        MessageType Type { get; set; }
        int ID { get; set; }
        string MethodName { get; set; }
        List<object> Parameters { get; set; }
        List<int> Callbacks { get; set;}
        object Result { get; set; }
        bool IsException { get; set; }
    }
}
