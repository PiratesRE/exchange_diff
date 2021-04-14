using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Configuration
	{
		public static event Action<object> ConfigurationChanged;

		public static string AppConfigFileName
		{
			get
			{
				if (Configuration.appConfigFile == null)
				{
					Configuration.appConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
				}
				return Configuration.appConfigFile;
			}
			set
			{
				Configuration.appConfigFile = value;
			}
		}

		public static ICollection<AuxiliaryBlock> DefaultEcDoConnectExAuxOutBlocks
		{
			get
			{
				if (Configuration.defaultEcDoConnectExAuxOutBlocks != null)
				{
					return Configuration.defaultEcDoConnectExAuxOutBlocks;
				}
				return Configuration.defaultEcDoConnectExAuxOutBlocks = Array.AsReadOnly<AuxiliaryBlock>(new AuxiliaryBlock[]
				{
					new MapiEndpointAuxiliaryBlock(MapiEndpointProcessType.RpcClientAccess, Configuration.ServiceConfiguration.ThisServerFqdn),
					new ServerCapabilitiesAuxiliaryBlock(ServerCapabilityFlag.PackedFastTransferUploadBuffers | ServerCapabilityFlag.PackedWriteStreamExtendedUploadBuffers),
					new EndpointCapabilitiesAuxiliaryBlock(EndpointCapabilityFlag.SingleEndpoint)
				});
			}
		}

		public static ConfigurationSchema.EventLogger EventLogger { get; set; }

		public static ProtocolLogConfiguration ProtocolLogConfiguration
		{
			get
			{
				return Configuration.protocolLogConfiguration;
			}
			set
			{
				Configuration.protocolLogConfiguration = value;
			}
		}

		public static ServiceConfiguration ServiceConfiguration { get; set; }

		internal static void InternalFireOnChanged(object newConfiguration)
		{
			Action<object> configurationChanged = Configuration.ConfigurationChanged;
			if (configurationChanged != null)
			{
				configurationChanged(newConfiguration);
			}
		}

		private static ICollection<AuxiliaryBlock> defaultEcDoConnectExAuxOutBlocks;

		private static ProtocolLogConfiguration protocolLogConfiguration = ProtocolLogConfiguration.Default;

		private static string appConfigFile = null;
	}
}
