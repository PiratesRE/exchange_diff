using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncConfigSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<SyncOrganizationSchema>();
		}

		public static readonly ADPropertyDefinition FederatedTenant = SyncOrganizationSchema.FederatedTenant;

		public static readonly ADPropertyDefinition DisableWindowsLiveID = SyncOrganizationSchema.DisableWindowsLiveID;

		public static readonly ADPropertyDefinition FederatedIdentitySourceADAttribute = SyncOrganizationSchema.FederatedIdentitySourceADAttribute;

		public static readonly ADPropertyDefinition WlidUseSMTPPrimary = SyncOrganizationSchema.WlidUseSMTPPrimary;

		public static readonly ADPropertyDefinition PasswordFilePath = SyncOrganizationSchema.PasswordFilePath;

		public static readonly ADPropertyDefinition ResetPasswordOnNextLogon = SyncOrganizationSchema.ResetPasswordOnNextLogon;

		public static readonly ADPropertyDefinition ProvisioningDomain = SyncOrganizationSchema.ProvisioningDomain;

		public static readonly ADPropertyDefinition EnterpriseExchangeVersion = SyncOrganizationSchema.EnterpriseExchangeVersion;
	}
}
