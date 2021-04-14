using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class MesoContainerSchema : ADConfigurationObjectSchema
	{
		internal static readonly int DomainPrepVersion = 13237;

		public static readonly ADPropertyDefinition ObjectVersion = new ADPropertyDefinition("ObjectVersion", ExchangeObjectVersion.Exchange2003, typeof(int), "objectVersion", ADPropertyDefinitionFlags.PersistDefaultValue, MesoContainerSchema.DomainPrepVersion, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ShowInAdvancedViewOnly = new ADPropertyDefinition("ShowInAdvancedViewOnly", ExchangeObjectVersion.Exchange2003, typeof(bool), "showInAdvancedViewOnly", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
