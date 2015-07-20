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
using System.IO;
using System.Linq;
using System.Reflection;

namespace SINFONI
{
    #region Testing
    public interface IProtocolRegistry
    {
        void RegisterProtocol(IProtocol protocol);
        IProtocol GetProtocol(string protocol);
        bool IsRegistered(string protocol);
    }
    #endregion

    /// <summary>
    /// Protocol registry. Allows to register new protocol types and their implementations.
    /// </summary>
    public class ProtocolRegistry : IProtocolRegistry
    {
        /// <summary>
        /// Default instance of the protocol registry.
        /// </summary>
        public readonly static ProtocolRegistry Instance = new ProtocolRegistry();

        /// <summary>
        /// Registers a protocol implementation. Adds it to the list of available protocols and makes it accessible
        /// under the name that is specified in the IProtocol implementaion
        /// </summary>
        /// <param name="protocol">Protocol implementation</param>
        public void RegisterProtocol(IProtocol protocol)
        {
            if (protocol.Name == null)
                throw new Error(ErrorCode.INVALID_VALUE, "Protocol name must not be null.");

            if (IsRegistered(protocol.Name))
                throw new Error(ErrorCode.INVALID_VALUE, "Protocol " + protocol.Name + " is already registered.");

            registeredProtocols[protocol.Name] = protocol;
        }

        /// <summary>
        /// Returns the protocol with the provided <paramref name="protocolName"/>. If protocol is not registered,
        /// an exception is thrown.
        /// </summary>
        /// <returns>The protocol factory.</returns>
        /// <param name="protocolName">Connection name.</param>
        public IProtocol GetProtocol(string protocolName) {
            if (IsRegistered(protocolName))
                return registeredProtocols[protocolName];
            throw new Error(ErrorCode.GENERIC_ERROR, "Protocol " + protocolName + " is not registered.");
        }

        /// <summary>
        /// Returns whether a given <paramref name="protocol"/> is registered.
        /// </summary>
        /// <returns><c>true</c>, if <paramref name="protocol"/> is registered, <c>false</c> otherwise.</returns>
        /// <param name="protocol">Protocol name.</param>
        public bool IsRegistered(string protocol) {
            return registeredProtocols.ContainsKey(protocol);
        }

        public void LoadProtocolsFrom(string protocolDir, string[] pluginWhiteList)
        {
            string[] files = Directory.GetFiles(protocolDir, "*.dll");

            foreach (string filename in files)
            {
                string cleanFilename = Path.GetFileName(filename);
                if (pluginWhiteList != null)
                {
                    if (pluginWhiteList.Any(whiteListEntry => cleanFilename.Equals(whiteListEntry + ".dll")))
                        LoadProtocol(filename);
                }
                else
                {
                    LoadProtocol(filename);
                }
            }
        }

        void LoadProtocol(string filename)
        {
            try {
                Assembly assembly = Assembly.LoadFrom(filename);
                List<Type> types = new List<Type>(assembly.GetTypes());
                Type interfaceType = typeof(IProtocol);
                Type connectionFactoryType = types.Find(t => interfaceType.IsAssignableFrom(t));
                if (connectionFactoryType == null || connectionFactoryType.Equals(interfaceType)) {
                    return;
                }

                // Instantiate and register protocol factory.
                IProtocol protocol;
                try {
                    protocol = (IProtocol)Activator.CreateInstance(connectionFactoryType);
                } catch (Exception ex) {
                    return;
                }
                RegisterProtocol(protocol);
            } catch (BadImageFormatException e) {
                return;
            } catch (Exception e) {
                logger.Warn("Failed to load file " + filename + " as a protocol", e);
                return;
            }
        }

        Dictionary<string, IProtocol> registeredProtocols = new Dictionary<string, IProtocol>();

    }
}

