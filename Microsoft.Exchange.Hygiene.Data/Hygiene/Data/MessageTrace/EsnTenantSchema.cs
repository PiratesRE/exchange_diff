using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class EsnTenantSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonMessageTraceSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition RecipientCountProperty = new HygienePropertyDefinition("RecipientCount", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageCountProperty = new HygienePropertyDefinition("MessageCount", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition UpperBoundaryQueryProperty = new HygienePropertyDefinition("UpperBoundaryDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
