using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedAccountConfigurationSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition EmailAddressRaw = new SimplePropertyDefinition("EmailAddressRaw", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition EmailAddress = new SimplePropertyDefinition("EmailAddress", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimplePropertyDefinition[]
		{
			AggregatedAccountConfigurationSchema.EmailAddressRaw
		}, null, new GetterDelegate(AggregatedAccountConfiguration.SmtpAddressGetter), new SetterDelegate(AggregatedAccountConfiguration.SmtpAddressSetter));

		public static readonly SimplePropertyDefinition SyncFailureCode = new SimplePropertyDefinition("SyncFailureCode", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncFailureTimestamp = new SimplePropertyDefinition("SyncFailureTimestamp", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncFailureType = new SimplePropertyDefinition("SyncFailureType", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncLastUpdateTimestamp = new SimplePropertyDefinition("SyncLastUpdateTimestamp", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncQueuedTimestamp = new SimplePropertyDefinition("SyncQueuedTimestamp", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncRequestGuidRaw = new SimplePropertyDefinition("SyncRequestGuidRaw", ExchangeObjectVersion.Exchange2012, typeof(byte[]), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncRequestGuid = new SimplePropertyDefinition("SyncRequestGuid", ExchangeObjectVersion.Exchange2012, typeof(Guid), PropertyDefinitionFlags.Calculated, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimplePropertyDefinition[]
		{
			AggregatedAccountConfigurationSchema.SyncRequestGuidRaw
		}, null, new GetterDelegate(AggregatedAccountConfiguration.SyncRequestGuidGetter), new SetterDelegate(AggregatedAccountConfiguration.SyncRequestGuidSetter));

		public static readonly SimplePropertyDefinition SyncStartTimestamp = new SimplePropertyDefinition("SyncStartTimestamp", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncStatus = new SimplePropertyDefinition("SyncStatus", ExchangeObjectVersion.Exchange2012, typeof(RequestStatus), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition SyncSuspendedTimestamp = new SimplePropertyDefinition("SyncSuspendedTimestamp", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
