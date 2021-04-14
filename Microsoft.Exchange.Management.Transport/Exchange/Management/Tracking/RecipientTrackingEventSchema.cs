using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;

namespace Microsoft.Exchange.Management.Tracking
{
	internal class RecipientTrackingEventSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Date = new SimpleProviderPropertyDefinition("Date", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.UtcNow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientAddress = new SimpleProviderPropertyDefinition("RecipientAddress", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientDisplayName = new SimpleProviderPropertyDefinition("RecipientDisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DeliveryStatus = new SimpleProviderPropertyDefinition("DeliveryStatus", ExchangeObjectVersion.Exchange2010, typeof(_DeliveryStatus), PropertyDefinitionFlags.None, _DeliveryStatus.Pending, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EventTypeValue = new SimpleProviderPropertyDefinition("EventType", ExchangeObjectVersion.Exchange2010, typeof(EventType), PropertyDefinitionFlags.None, EventType.Submit, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EventDescription = new SimpleProviderPropertyDefinition("EventDescription", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EventData = new SimpleProviderPropertyDefinition("EventData", ExchangeObjectVersion.Exchange2010, typeof(string[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Server = new SimpleProviderPropertyDefinition("Server", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
