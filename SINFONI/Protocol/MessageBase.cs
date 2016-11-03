﻿// This file is part of SINFONI.
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
    [Serializable]
    public class MessageBase : IMessage
    {
        public MessageType Type { get; set; }

        public int ID { get; set; }

        public string MethodName { get; set; }

        public List<object> Parameters { get; set; }

        public List<int> Callbacks { get; set; }

        public object Result { get; set; }

        public bool IsException { get; set; }
    }
}
