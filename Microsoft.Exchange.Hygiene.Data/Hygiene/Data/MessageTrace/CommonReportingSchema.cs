using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class CommonReportingSchema
	{
		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		internal static readonly HygienePropertyDefinition DataSourceProperty = new HygienePropertyDefinition("DataSource", typeof(string), "EXO", ADPropertyDefinitionFlags.Mandatory);

		internal static readonly HygienePropertyDefinition TrafficTypeProperty = new HygienePropertyDefinition("TrafficType", typeof(string), string.Empty, ADPropertyDefinitionFlags.Mandatory);

		internal static readonly HygienePropertyDefinition DateKeyProperty = new HygienePropertyDefinition("DateKey", typeof(int), 0, ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition HourKeyProperty = new HygienePropertyDefinition("HourKey", typeof(short), 0, ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TenantDomainProperty = new HygienePropertyDefinition("TenantDomain", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageCountProperty = new HygienePropertyDefinition("MessageCount", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RecipientCountProperty = new HygienePropertyDefinition("RecipientCount", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainHashKeyProp = new HygienePropertyDefinition("DomainHashKey", typeof(byte[]));

		internal static readonly HygienePropertyDefinition StartDateKeyProperty = new HygienePropertyDefinition("StartDateKey", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition StartHourKeyProperty = new HygienePropertyDefinition("StartHourKey", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndDateKeyProperty = new HygienePropertyDefinition("EndDateKey", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EndHourKeyProperty = new HygienePropertyDefinition("EndHourKey", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
