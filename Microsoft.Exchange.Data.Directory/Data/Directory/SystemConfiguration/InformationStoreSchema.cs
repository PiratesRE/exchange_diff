using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class InformationStoreSchema : ADLegacyVersionableObjectSchema
	{
		internal static readonly ADPropertyDefinition MaxStorageGroups = new ADPropertyDefinition("MaxStorageGroups", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchMaxStorageGroups", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 5, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MaxStoresPerGroup = new ADPropertyDefinition("MaxStoresPerGroup", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchMaxStoresPerGroup", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 5, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MaxStoresTotal = new ADPropertyDefinition("MaxStoresTotal", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMaxStoresTotal", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 5, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MaxRestoreStorageGroups = new ADPropertyDefinition("MaxRestoreStorageGroups", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchMaxRestoreStorageGroups", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MinCachePages = new ADPropertyDefinition("MinCachePages", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchESEParamCacheSizeMin", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MaxCachePages = new ADPropertyDefinition("MaxCachePages", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchESEParamCacheSizeMax", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EnableOnlineDefragmentation = new ADPropertyDefinition("EnableOnlineDefragmentation", ExchangeObjectVersion.Exchange2010, typeof(bool?), "msExchESEParamEnableOnlineDefrag", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MaxRpcThreads = new ADPropertyDefinition("MaxRpcThreads", ExchangeObjectVersion.Exchange2003, typeof(int?), "msExchMaxThreads", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
