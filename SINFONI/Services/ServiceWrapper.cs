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
using SINFONI;

namespace SINFONI
{
    public delegate void Connected(Connection connection);

    public class ServiceWrapper : Service
    {
        /// <summary>
        /// Occurs when connection is established. New handlers are also invoked, even if connection have been
        /// established before they were added.
        /// </summary>
        private event Connected InternalOnConnected;
        public event Connected OnConnected {
            add {
                if (connection == null)
                    InternalOnConnected += value;
                else
                    value(connection);
            }
            remove {
                if (connection == null)
                    InternalOnConnected -= value;
            }
        }

        internal void HandleConnected(Connection aConnection)
        {
            connection = aConnection;
            
            RegisterMethods(connection);

            if (InternalOnConnected != null)
                InternalOnConnected(aConnection);
        }

        internal ServiceWrapper(Context context) : base(context) {}

        private Connection connection = null;
    }
}

