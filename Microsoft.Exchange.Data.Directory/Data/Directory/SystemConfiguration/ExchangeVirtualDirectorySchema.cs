using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExchangeVirtualDirectorySchema : ADVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition MetabasePath = new ADPropertyDefinition("MetabasePath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMetabasePath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExtendedProtectionTokenChecking = new ADPropertyDefinition("ExtendedProtectionTokenChecking", ExchangeObjectVersion.Exchange2010, typeof(ExtendedProtectionTokenCheckingMode), null, ADPropertyDefinitionFlags.TaskPopulated, ExtendedProtectionTokenCheckingMode.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExtendedProtectionFlags = new ADPropertyDefinition("ExtendedProtectionFlags", ExchangeObjectVersion.Exchange2010, typeof(ExtendedProtectionFlag), null, ADPropertyDefinitionFlags.TaskPopulated, ExtendedProtectionFlag.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExtendedProtectionSPNList = new ADPropertyDefinition("ExtendedProtectionSPNList", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
