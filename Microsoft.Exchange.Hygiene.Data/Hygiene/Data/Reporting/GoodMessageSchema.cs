using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class GoodMessageSchema
	{
		internal static readonly HygienePropertyDefinition GoodMessageExistsProperty = new HygienePropertyDefinition("GoodMessageExists", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastNMinutesQueryProperty = ReportingCommonSchema.LastNMinutesQueryProperty;

		internal static readonly HygienePropertyDefinition MinimumGoodMessageCountQueryProperty = ReportingCommonSchema.MinimumGoodMessageCountQueryProperty;

		internal static readonly HygienePropertyDefinition StartingIPAddressQueryProperty = ReportingCommonSchema.StartingIPAddressQueryProperty;

		internal static readonly HygienePropertyDefinition EndIPAddressQueryProperty = ReportingCommonSchema.EndIPAddressQueryProperty;
	}
}
