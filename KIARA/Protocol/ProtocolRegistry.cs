using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
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
        /// Registers a connection <paramref name="factory"/> for the <paramref name="protocolName"/>.
        /// </summary>
        /// <param name="protocolName">Protocol name.</param>
        /// <param name="factory">Connection factory.</param>
        public void RegisterProtocol(IProtocol protocol)
        {
            if (protocol.Name == null)
                throw new Error(ErrorCode.INVALID_VALUE, "Protocol name must not be null.");

            if (IsRegistered(protocol.Name))
                throw new Error(ErrorCode.INVALID_VALUE, "Protocol " + protocol.Name + " is already registered.");

            registeredProtocols[protocol.Name] = protocol;
        }

        /// <summary>
        /// Returns connection factory for <paramref name="protocolName"/>. If protocol is not registered, an exception is
        /// thrown.
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
                // Load an assembly.
                Assembly assembly = Assembly.LoadFrom(filename);

                // Find connection factory (class implementing IConnectionFactory).
                List<Type> types = new List<Type>(assembly.GetTypes());
                Type interfaceType = typeof(IProtocol);
                Type connectionFactoryType = types.Find(t => interfaceType.IsAssignableFrom(t));
                if (connectionFactoryType == null || connectionFactoryType.Equals(interfaceType)) {
                    logger.Info("Assembly in file " + filename +
                                " doesn't contain any class implementing IConnectionFactory.");
                    return;
                }

                // Instantiate and register protocol factory.
                IProtocol protocol;
                try {
                    protocol = (IProtocol)Activator.CreateInstance(connectionFactoryType);
                } catch (Exception ex) {
                    logger.Warn("Exception occured during construction of protocol factory for " + filename + ".", ex);
                    return;
                }
                RegisterProtocol(protocol);
                logger.Info("Registered protocol {0}", protocol.Name);
            } catch (BadImageFormatException e) {
                logger.Info(filename + " is not a valid assembly and thus cannot be loaded as a protocol.", e);
                return;
            } catch (Exception e) {
                logger.Warn("Failed to load file " + filename + " as a protocol", e);
                return;
            }
        }

        Dictionary<string, IProtocol> registeredProtocols = new Dictionary<string, IProtocol>();

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}

