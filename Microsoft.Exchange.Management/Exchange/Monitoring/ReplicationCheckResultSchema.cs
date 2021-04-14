using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	internal class ReplicationCheckResultSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition Value = new SimpleProviderPropertyDefinition("Value", ExchangeObjectVersion.Exchange2010, typeof(ReplicationCheckResultEnum), PropertyDefinitionFlags.None, ReplicationCheckResultEnum.Undefined, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
