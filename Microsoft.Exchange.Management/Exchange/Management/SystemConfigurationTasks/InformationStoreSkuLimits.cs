using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal class InformationStoreSkuLimits
	{
		private InformationStoreSkuLimits(int maxStorageGroups, int maxStoresPerGroup, int maxStoresTotal, int maxRestoreStorageGroups)
		{
			this.maxStorageGroups = maxStorageGroups;
			this.maxStoresPerGroup = maxStoresPerGroup;
			this.maxStoresTotal = maxStoresTotal;
			this.maxRestoreStorageGroups = maxRestoreStorageGroups;
		}

		public InformationStoreSkuLimits(InformationStore informationStore) : this(informationStore.MaxStorageGroups, informationStore.MaxStoresPerGroup, informationStore.MaxStoresTotal, informationStore.MaxRestoreStorageGroups)
		{
		}

		public void UpdateInformationStore(InformationStore informationStore)
		{
			informationStore.MaxStorageGroups = this.maxStorageGroups;
			informationStore.MaxStoresPerGroup = this.maxStoresPerGroup;
			informationStore.MaxStoresTotal = this.maxStoresTotal;
			informationStore.MaxRestoreStorageGroups = this.maxRestoreStorageGroups;
		}

		private readonly int maxStorageGroups;

		private readonly int maxStoresPerGroup;

		private readonly int maxStoresTotal;

		private readonly int maxRestoreStorageGroups;

		public static readonly InformationStoreSkuLimits Enterprise = new InformationStoreSkuLimits(100, 5, 100, 1);

		public static readonly InformationStoreSkuLimits Standard = new InformationStoreSkuLimits(5, 5, 5, 1);

		public static readonly InformationStoreSkuLimits Coexistence = new InformationStoreSkuLimits(5, 5, 5, 1);
	}
}
