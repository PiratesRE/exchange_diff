using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	internal class RecipientTemplateProvisioningPolicySchema : TemplateProvisioningPolicySchema
	{
		public static readonly ADPropertyDefinition DefaultMaxSendSize = ProvisioningHelper.FromProvisionedADProperty(MailEnabledRecipientSchema.MaxSendSize, "DefaultMaxSendSize", "msExchRecipientMaxSendSize");

		public static readonly ADPropertyDefinition DefaultMaxReceiveSize = ProvisioningHelper.FromProvisionedADProperty(MailEnabledRecipientSchema.MaxReceiveSize, "DefaultMaxReceiveSize", "msExchRecipientMaxReceiveSize");

		public static readonly ADPropertyDefinition DefaultProhibitSendQuota = ProvisioningHelper.FromProvisionedADProperty(MailboxSchema.ProhibitSendQuota, "DefaultProhibitSendQuota", "msExchRecipientProhibitSendQuota");

		public static readonly ADPropertyDefinition DefaultProhibitSendReceiveQuota = ProvisioningHelper.FromProvisionedADProperty(MailboxSchema.ProhibitSendReceiveQuota, "DefaultProhibitSendReceiveQuota", "msExchRecipientProhibitSendReceiveQuota");

		public static readonly ADPropertyDefinition DefaultIssueWarningQuota = ProvisioningHelper.FromProvisionedADProperty(MailboxSchema.IssueWarningQuota, "DefaultIssueWarningQuota", "msExchRecipientIssueWarningQuota");

		public static readonly ADPropertyDefinition DefaultRulesQuota = ProvisioningHelper.FromProvisionedADProperty(MailboxSchema.RulesQuota, "DefaultRulesQuota", "msExchRecipientRulesQuota");

		public static readonly ADPropertyDefinition DefaultDistributionListOU = ProvisioningHelper.FromProvisionedADProperty(ADRecipientSchema.DefaultDistributionListOU, "DefaultDistributionListOU", "msExchDistributionListOU");
	}
}
