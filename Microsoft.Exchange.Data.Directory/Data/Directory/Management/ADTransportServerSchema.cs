using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ADTransportServerSchema : TransportServerSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ActiveDirectoryServerSchema>();
		}

		public static readonly ADPropertyDefinition MaxConcurrentMailboxSubmissions = ActiveDirectoryServerSchema.MaxConcurrentMailboxSubmissions;

		public static readonly ADPropertyDefinition UseDowngradedExchangeServerAuth = ActiveDirectoryServerSchema.UseDowngradedExchangeServerAuth;

		public static readonly ADPropertyDefinition TransportSyncAccountsSuccessivePoisonItemThreshold = ActiveDirectoryServerSchema.TransportSyncAccountsSuccessivePoisonItemThreshold;

		public static readonly ADPropertyDefinition TransportSyncHubHealthLogEnabled = ActiveDirectoryServerSchema.TransportSyncHubHealthLogEnabled;

		public static readonly ADPropertyDefinition TransportSyncHubHealthLogFilePath = ActiveDirectoryServerSchema.TransportSyncHubHealthLogFilePath;

		public static readonly ADPropertyDefinition TransportSyncHubHealthLogMaxAge = ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxAge;

		public static readonly ADPropertyDefinition TransportSyncHubHealthLogMaxDirectorySize = ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxDirectorySize;

		public static readonly ADPropertyDefinition TransportSyncHubHealthLogMaxFileSize = ActiveDirectoryServerSchema.TransportSyncHubHealthLogMaxFileSize;
	}
}
