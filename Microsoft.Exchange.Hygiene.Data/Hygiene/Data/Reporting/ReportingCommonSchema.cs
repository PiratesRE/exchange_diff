using System;
using System.Net;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class ReportingCommonSchema
	{
		internal static readonly HygienePropertyDefinition TenantIdProperty = new HygienePropertyDefinition("TenantId", typeof(Guid));

		internal static readonly HygienePropertyDefinition IPAddressProperty = new HygienePropertyDefinition("IPAddress", typeof(IPAddress));

		internal static readonly HygienePropertyDefinition FromEmailAddressProperty = new HygienePropertyDefinition("FromEmailAddress", typeof(string));

		internal static readonly HygienePropertyDefinition AggregationDateProperty = new HygienePropertyDefinition("AggregationDate", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SpamPercentageProperty = new HygienePropertyDefinition("SpamPercentage", typeof(double));

		internal static readonly HygienePropertyDefinition SpamMessageCountProperty = new HygienePropertyDefinition("SpamMessageCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TotalMessageCountProperty = new HygienePropertyDefinition("TotalMessageCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition SpamRecipientCountProperty = new HygienePropertyDefinition("SpamRecipientCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TotalRecipientCountProperty = new HygienePropertyDefinition("TotalRecipientCount", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastNMinutesQueryProperty = new HygienePropertyDefinition("LastNMinutes", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartingIPAddressQueryProperty = new HygienePropertyDefinition("StartIPAddress", typeof(IPAddress));

		internal static readonly HygienePropertyDefinition EndIPAddressQueryProperty = new HygienePropertyDefinition("EndIPAddress", typeof(IPAddress));

		internal static readonly HygienePropertyDefinition PageSizeQueryProperty = new HygienePropertyDefinition("PageSize", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MinimumGoodMessageCountQueryProperty = new HygienePropertyDefinition("GoodMessageCountThreshold", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid?));

		internal static readonly HygienePropertyDefinition OverriddenOnlyProperty = new HygienePropertyDefinition("OverriddenOnly", typeof(bool?));

		internal static readonly HygienePropertyDefinition ThrottledOnlyProperty = new HygienePropertyDefinition("ThrottledOnly", typeof(bool?));

		internal static readonly HygienePropertyDefinition DataCountProperty = new HygienePropertyDefinition("DataCount", typeof(int?));
	}
}
