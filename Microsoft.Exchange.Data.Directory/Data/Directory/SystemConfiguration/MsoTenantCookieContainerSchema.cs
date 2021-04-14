using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class MsoTenantCookieContainerSchema : OrganizationSchema
	{
		public static readonly ADPropertyDefinition MsoForwardSyncRecipientCookie = MsoMainStreamCookieContainerSchema.MsoForwardSyncRecipientCookie;

		public static readonly ADPropertyDefinition MsoForwardSyncNonRecipientCookie = MsoMainStreamCookieContainerSchema.MsoForwardSyncNonRecipientCookie;

		public static readonly ADPropertyDefinition ExternalDirectoryOrganizationId = ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId;

		public static readonly ADPropertyDefinition DirSyncServiceInstance = ExchangeConfigurationUnitSchema.DirSyncServiceInstance;
	}
}
