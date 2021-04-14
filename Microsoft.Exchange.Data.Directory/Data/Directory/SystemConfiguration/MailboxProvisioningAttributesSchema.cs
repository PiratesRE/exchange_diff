using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MailboxProvisioningAttributesSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition RegionPropertyDefinition = new SimpleProviderPropertyDefinition("Region", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LocationPropertyDefinition = new SimpleProviderPropertyDefinition("Location", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DagName = new SimpleProviderPropertyDefinition("DagName", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ServerName = new SimpleProviderPropertyDefinition("ServerName", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DatabaseName = new SimpleProviderPropertyDefinition("DatabaseName", ExchangeObjectVersion.Exchange2003, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
