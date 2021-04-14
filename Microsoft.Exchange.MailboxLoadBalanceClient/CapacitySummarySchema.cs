using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalanceClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CapacitySummarySchema : ObjectSchema
	{
		public static readonly PropertyDefinition Identity = ADObjectSchema.Name;

		public static readonly PropertyDefinition LogicalSize = new SimpleProviderPropertyDefinition("LogicalSize", ExchangeObjectVersion.Exchange2012, typeof(ByteQuantifiedSize), PropertyDefinitionFlags.TaskPopulated, ByteQuantifiedSize.Zero, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly PropertyDefinition MaximumSize = new SimpleProviderPropertyDefinition("MaximumSize", ExchangeObjectVersion.Exchange2012, typeof(ByteQuantifiedSize), PropertyDefinitionFlags.TaskPopulated, ByteQuantifiedSize.Zero, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly PropertyDefinition PhysicalSize = new SimpleProviderPropertyDefinition("PhysicalSize", ExchangeObjectVersion.Exchange2012, typeof(ByteQuantifiedSize), PropertyDefinitionFlags.TaskPopulated, ByteQuantifiedSize.Zero, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly PropertyDefinition RetrievedTimestamp = new SimpleProviderPropertyDefinition("RetrievedTimestamp", ExchangeObjectVersion.Exchange2012, typeof(DateTime), PropertyDefinitionFlags.TaskPopulated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly PropertyDefinition TotalMailboxCount = new SimpleProviderPropertyDefinition("TotalMailboxCount", ExchangeObjectVersion.Exchange2012, typeof(long), PropertyDefinitionFlags.TaskPopulated, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
