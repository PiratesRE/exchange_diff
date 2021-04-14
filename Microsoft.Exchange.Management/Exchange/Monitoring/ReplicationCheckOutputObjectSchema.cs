using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	internal class ReplicationCheckOutputObjectSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition Server = new SimpleProviderPropertyDefinition("Server", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition CheckIdProperty = new SimpleProviderPropertyDefinition("CheckId", ExchangeObjectVersion.Exchange2010, typeof(CheckId), PropertyDefinitionFlags.None, CheckId.Undefined, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Check = new SimpleProviderPropertyDefinition("Check", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition IdentityProperty = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition DbFailureEventId = new SimpleProviderPropertyDefinition("DbFailureEventId", ExchangeObjectVersion.Exchange2010, typeof(uint?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Result = new SimpleProviderPropertyDefinition("Result", ExchangeObjectVersion.Exchange2010, typeof(ReplicationCheckResult), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Error = new SimpleProviderPropertyDefinition("Error", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
