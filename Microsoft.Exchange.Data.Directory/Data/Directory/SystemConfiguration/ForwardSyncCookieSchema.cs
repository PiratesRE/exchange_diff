using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ForwardSyncCookieSchema : ForwardSyncCookieHeaderSchema
	{
		internal static readonly ADPropertyDefinition Version = new ADPropertyDefinition("Version", ExchangeObjectVersion.Exchange2010, typeof(int), "VersionNumber", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition SyncPropertySetVersion = new ADPropertyDefinition("SyncPropertySetVersion", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchMSOForwardSyncCookiePropertySetVersion", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsUpgradingSyncPropertySet = ADObject.BitfieldProperty("IsSyncPropertySetUpgrading", 4, SharedPropertyDefinitions.ProvisioningFlags);

		internal static readonly ADPropertyDefinition Data = new ADPropertyDefinition("Data", ExchangeObjectVersion.Exchange2010, typeof(byte[]), "msExchSyncCookie", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 262144)
		}, null, null);

		internal static readonly ADPropertyDefinition FilteredContextIds = new ADPropertyDefinition("FilteredContextIds", ExchangeObjectVersion.Exchange2010, typeof(string), "description", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, null, null);
	}
}
