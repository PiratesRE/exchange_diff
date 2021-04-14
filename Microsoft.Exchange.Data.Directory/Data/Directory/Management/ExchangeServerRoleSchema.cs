using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ExchangeServerRoleSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ServerSchema>();
		}

		public static readonly ADPropertyDefinition IsHubTransportServer = ServerSchema.IsHubTransportServer;

		public static readonly ADPropertyDefinition IsClientAccessServer = ServerSchema.IsClientAccessServer;

		public static readonly ADPropertyDefinition IsExchange2007OrLater = ServerSchema.IsExchange2007OrLater;

		public static readonly ADPropertyDefinition IsEdgeServer = ServerSchema.IsEdgeServer;

		public static readonly ADPropertyDefinition IsMailboxServer = ServerSchema.IsMailboxServer;

		public static readonly ADPropertyDefinition IsProvisionedServer = ServerSchema.IsProvisionedServer;

		public static readonly ADPropertyDefinition IsUnifiedMessagingServer = ServerSchema.IsUnifiedMessagingServer;

		public static readonly ADPropertyDefinition IsCafeServer = ServerSchema.IsCafeServer;

		public static readonly ADPropertyDefinition IsFrontendTransportServer = ServerSchema.IsFrontendTransportServer;
	}
}
