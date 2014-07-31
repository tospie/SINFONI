// This file is part of FiVES.
//
// FiVES is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// FiVES is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with FiVES.  If not, see <http://www.gnu.org/licenses/>.
using KIARA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleClient
{
    /// <summary>
    /// General tool functions used in the ServerSync plugin.
    /// </summary>
    static class ServerSyncTools
    {
        /// <summary>
        /// Converts a file name to the URI that point to the file as if it was located in the same directory as the
        /// current assembly.
        /// </summary>
        /// <param name="configFilename"></param>
        /// <returns></returns>
        public static string ConvertFileNameToURI(string configFilename)
        {
            string assemblyPath = typeof(SimpleClient).Assembly.Location;
            var configFullPath = Path.Combine(Path.GetDirectoryName(assemblyPath), configFilename);
            return "file://" + configFullPath;
        }
    }
}
