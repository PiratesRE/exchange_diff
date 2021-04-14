using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniTopologyServerSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Fqdn = ServerSchema.Fqdn;

		public static readonly ADPropertyDefinition VersionNumber = ServerSchema.VersionNumber;

		public static readonly ADPropertyDefinition MajorVersion = ServerSchema.MajorVersion;

		public static readonly ADPropertyDefinition AdminDisplayVersion = ServerSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition IsE14OrLater = ServerSchema.IsE14OrLater;

		public static readonly ADPropertyDefinition IsExchange2007OrLater = ServerSchema.IsExchange2007OrLater;

		public static readonly ADPropertyDefinition IsE15OrLater = ServerSchema.IsE15OrLater;

		public static readonly ADPropertyDefinition ServerSite = ServerSchema.ServerSite;

		public static readonly ADPropertyDefinition ExchangeLegacyDN = ServerSchema.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition CurrentServerRole = ServerSchema.CurrentServerRole;

		public static readonly ADPropertyDefinition IsClientAccessServer = ServerSchema.IsClientAccessServer;

		public static readonly ADPropertyDefinition IsCafeServer = ServerSchema.IsCafeServer;

		public static readonly ADPropertyDefinition IsHubTransportServer = ServerSchema.IsHubTransportServer;

		public static readonly ADPropertyDefinition IsEdgeServer = ServerSchema.IsEdgeServer;

		public static readonly ADPropertyDefinition IsFrontendTransportServer = ServerSchema.IsFrontendTransportServer;

		public static readonly ADPropertyDefinition IsMailboxServer = ServerSchema.IsMailboxServer;

		public static readonly ADPropertyDefinition DatabaseAvailabilityGroup = ServerSchema.DatabaseAvailabilityGroup;

		public static readonly ADPropertyDefinition ComponentStates = ServerSchema.ComponentStates;

		public static readonly ADPropertyDefinition HomeRoutingGroup = ServerSchema.HomeRoutingGroup;

		public static readonly ADPropertyDefinition ExternalPostmasterAddress = ServerSchema.ExternalPostmasterAddress;

		public static readonly ADPropertyDefinition IsOutOfService = ActiveDirectoryServerSchema.IsOutOfService;
	}
}
