using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class FederatedOrganizationIdSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition AccountNamespace = new ADPropertyDefinition("AccountNamespace", ExchangeObjectVersion.Exchange2010, typeof(SmtpDomain), "msExchFedAccountNamespace", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchFedIsEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrganizationContact = new ADPropertyDefinition("OrganizationContact", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), "msExchFedOrgAdminContact", ADPropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 320),
			new ValidSmtpAddressConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition OrganizationApprovalContact = new ADPropertyDefinition("OrganizationApprovalContact", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), "msExchFedOrgApprovalContact", ADPropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 320),
			new ValidSmtpAddressConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition DelegationTrustLink = new ADPropertyDefinition("DelegationTrustLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), null, "msExchFedDelegationTrust", null, "msExchFedDelegationTrustSL", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition ClientTrustLink = new ADPropertyDefinition("ClientTrustLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchFedClientTrust", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AcceptedDomainsBackLink = new ADPropertyDefinition("AcceptedDomainsBackLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchFedAcceptedDomainBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultSharingPolicyLink = new ADPropertyDefinition("DefaultSharingPolicyLink", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchSharingDefaultPolicyLink", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
