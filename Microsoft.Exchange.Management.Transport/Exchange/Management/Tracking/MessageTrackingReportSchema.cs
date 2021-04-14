using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tracking
{
	internal class MessageTrackingReportSchema : MessageTrackingSharedResultSchema
	{
		public static readonly SimpleProviderPropertyDefinition DeliveryCount = new SimpleProviderPropertyDefinition("DeliveryCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UnsuccessfulCount = new SimpleProviderPropertyDefinition("UnsuccessfulCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PendingCount = new SimpleProviderPropertyDefinition("PendingCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TransferredCount = new SimpleProviderPropertyDefinition("TransferredCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RecipientTrackingEvents = new SimpleProviderPropertyDefinition("RecipientTrackingEvents", ExchangeObjectVersion.Exchange2010, typeof(RecipientTrackingEvent[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
