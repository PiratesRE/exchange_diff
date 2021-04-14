using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationEndpointMessageSchema
	{
		public const string MigrationEndpointMessageClass = "IPM.MS-Exchange.MigrationEndpoint";

		public static readonly StorePropertyDefinition MigrationEndpointType = InternalSchema.MigrationEndpointType;

		public static readonly StorePropertyDefinition MigrationEndpointName = InternalSchema.MigrationEndpointName;

		public static readonly StorePropertyDefinition RemoteHostName = InternalSchema.MigrationEndpointRemoteHostName;

		public static readonly StorePropertyDefinition ExchangeServer = InternalSchema.MigrationEndpointExchangeServer;

		public static readonly StorePropertyDefinition LastModifiedTime = InternalSchema.MigrationSubscriptionSettingsLastModifiedTime;

		public static readonly StorePropertyDefinition MigrationEndpointGuid = InternalSchema.MigrationEndpointGuid;
	}
}
