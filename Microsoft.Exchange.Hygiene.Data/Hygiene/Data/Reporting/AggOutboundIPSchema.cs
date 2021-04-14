using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class AggOutboundIPSchema
	{
		internal static readonly HygienePropertyDefinition TenantIdProperty = ReportingCommonSchema.TenantIdProperty;

		internal static readonly HygienePropertyDefinition IPAddressProperty = ReportingCommonSchema.IPAddressProperty;

		internal static readonly HygienePropertyDefinition FromEmailAddressProperty = ReportingCommonSchema.FromEmailAddressProperty;

		internal static readonly HygienePropertyDefinition SpamMessageCountProperty = ReportingCommonSchema.SpamMessageCountProperty;

		internal static readonly HygienePropertyDefinition TotalMessageCountProperty = ReportingCommonSchema.TotalMessageCountProperty;

		internal static readonly HygienePropertyDefinition SpamRecipientCountProperty = ReportingCommonSchema.SpamRecipientCountProperty;

		internal static readonly HygienePropertyDefinition TotalRecipientCountProperty = ReportingCommonSchema.TotalRecipientCountProperty;

		internal static readonly HygienePropertyDefinition NDRSpamMessageCountProperty = new HygienePropertyDefinition("NDRSpamMessageCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NDRTotalMessageCountProperty = new HygienePropertyDefinition("NDRTotalMessageCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NDRSpamRecipientCountProperty = new HygienePropertyDefinition("NDRSpamRecipientCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NDRTotalRecipientCountProperty = new HygienePropertyDefinition("NDRTotalRecipientCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition UniqueDomainsCountProperty = new HygienePropertyDefinition("UniqueDomainsCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NonProvisionedDomainCountProperty = new HygienePropertyDefinition("NonProvisionedDomainCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition UniqueSendersCountProperty = new HygienePropertyDefinition("UniqueSendersCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ToSameDomainCountProperty = new HygienePropertyDefinition("ToSameDomainCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MaxRecipientCountProperty = new HygienePropertyDefinition("MaxRecipientCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ProvisionedDomainCountProperty = new HygienePropertyDefinition("ProvisionedDomainCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastNMinutesQueryProperty = ReportingCommonSchema.LastNMinutesQueryProperty;

		internal static readonly HygienePropertyDefinition MinimumEmailThresholdQueryProperty = new HygienePropertyDefinition("MinimumEmailThreshold", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = ReportingCommonSchema.PageSizeQueryProperty;

		internal static readonly HygienePropertyDefinition SummaryOnlyQueryProperty = new HygienePropertyDefinition("SummaryOnly", typeof(bool), true, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
