using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class AggInboundIPSchema
	{
		internal static readonly HygienePropertyDefinition IPAddressProperty = ReportingCommonSchema.IPAddressProperty;

		internal static readonly HygienePropertyDefinition MinimumSpamPercentageQueryProperty = new HygienePropertyDefinition("SpamPercentageThreshold", typeof(double));

		internal static readonly HygienePropertyDefinition MinimumSpamCountQueryProperty = new HygienePropertyDefinition("SpamCountThreshold", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartingIPAddressQueryProperty = ReportingCommonSchema.StartingIPAddressQueryProperty;

		internal static readonly HygienePropertyDefinition EndIPAddressQueryProperty = ReportingCommonSchema.EndIPAddressQueryProperty;

		internal static readonly HygienePropertyDefinition AggregationDateProperty = ReportingCommonSchema.AggregationDateProperty;

		internal static readonly HygienePropertyDefinition SpamPercentageProperty = ReportingCommonSchema.SpamPercentageProperty;

		internal static readonly HygienePropertyDefinition SpamMessageCountProperty = ReportingCommonSchema.SpamMessageCountProperty;

		internal static readonly HygienePropertyDefinition LastNMinutesQueryProperty = ReportingCommonSchema.LastNMinutesQueryProperty;

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = ReportingCommonSchema.PageSizeQueryProperty;
	}
}
