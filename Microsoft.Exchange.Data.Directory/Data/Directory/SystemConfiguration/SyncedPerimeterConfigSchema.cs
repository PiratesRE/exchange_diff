using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SyncedPerimeterConfigSchema : PerimeterConfigSchema
	{
		public static readonly ADPropertyDefinition SyncErrors = new ADPropertyDefinition("SyncErrors", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncCookies", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
