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
using Newtonsoft.Json.Linq;

namespace SINFONI
{
    /// <summary>
    /// Various utils to be used by protocol implementations.
    /// </summary>
    public static class ProtocolUtils
    {
        /// <summary>
        /// Attempts to retrieve a setting with <paramref name="name"/> from the server <paramref name="config"/>. The
        /// value of the setting is then casted into <typeparamref name="T">. If the setting is not present in config
        /// the <paramref name="defValue"/> is returned.
        /// </summary>
        /// <returns>The protocol setting value.</returns>
        /// <param name="config">The server config.</param>
        /// <param name="name">The setting name.</param>
        /// <param name="defValue">The default setting value.</param>
        /// <typeparam name="T">The type to which the setting value should be casted.</typeparam>
        public static object retrieveProtocolSetting<T>(ServiceDescription config, string name, object defValue) {
            object value = config.protocol.name;
            return value != null ? value : defValue;
        }
    }
}

