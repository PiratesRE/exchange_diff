using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADPowerShellCommonVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition VirtualDirectoryType = new ADPropertyDefinition("VirtualDirectoryType", ExchangeObjectVersion.Exchange2010, typeof(PowerShellVirtualDirectoryType), "msExchVirtualDirectoryFlags", ADPropertyDefinitionFlags.None, PowerShellVirtualDirectoryType.PowerShell, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
