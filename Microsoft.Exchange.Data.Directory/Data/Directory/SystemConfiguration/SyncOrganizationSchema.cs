using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SyncOrganizationSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition DisableWindowsLiveID = new ADPropertyDefinition("DisableWindowsLiveID", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchGalsyncDisableLiveIdOnRemove", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FederatedIdentitySourceADAttribute = new ADPropertyDefinition("FederatedIdentitySourceADAttribute", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchGalsyncFederatedTenantSourceAttribute", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new AsciiCharactersOnlyConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition WlidUseSMTPPrimary = new ADPropertyDefinition("WlidUseSMTPPrimary", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchGalsyncWlidUseSmtpPrimary", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PasswordFilePath = new ADPropertyDefinition("PasswordFilePath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchGalsyncPasswordFilePath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ResetPasswordOnNextLogon = new ADPropertyDefinition("ResetPasswordOnNextLogon", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchGalsyncResetPasswordOnNextLogon", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProvisioningDomain = new ADPropertyDefinition("ProvisioningDomain", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomainWithSubdomains), "msExchGalsyncProvisioningDomain", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EnterpriseExchangeVersion = new ADPropertyDefinition("EnterpriseExchangeVersion", ExchangeObjectVersion.Exchange2007, typeof(EnterpriseExchangeVersionEnum), "msExchGalsyncSourceActiveDirectorySchemaVersion", ADPropertyDefinitionFlags.None, EnterpriseExchangeVersionEnum.Exchange2010, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FederatedTenant = OrganizationSchema.IsFederated;
	}
}
