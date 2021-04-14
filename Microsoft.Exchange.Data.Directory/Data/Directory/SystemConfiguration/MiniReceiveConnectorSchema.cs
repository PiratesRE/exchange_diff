using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniReceiveConnectorSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Server = ReceiveConnectorSchema.Server;

		public static readonly ADPropertyDefinition SecurityFlags = ReceiveConnectorSchema.SecurityFlags;

		public static readonly ADPropertyDefinition Bindings = ReceiveConnectorSchema.Bindings;

		public static readonly ADPropertyDefinition AdvertiseClientSettings = ReceiveConnectorSchema.AdvertiseClientSettings;

		public static readonly ADPropertyDefinition Fqdn = ReceiveConnectorSchema.Fqdn;

		public static readonly ADPropertyDefinition ServiceDiscoveryFqdn = ReceiveConnectorSchema.ServiceDiscoveryFqdn;
	}
}
