using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class InformationStore : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return InformationStore.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return InformationStore.mostDerivedClass;
			}
		}

		internal Container GetChildContainer(string commonName)
		{
			return base.Session.Read<Container>(base.Id.GetChildId(commonName));
		}

		internal Container GetParentContainer()
		{
			return base.Session.Read<Container>(base.Id.Parent);
		}

		internal int MaxStorageGroups
		{
			get
			{
				return (int)this[InformationStoreSchema.MaxStorageGroups];
			}
			set
			{
				this[InformationStoreSchema.MaxStorageGroups] = value;
			}
		}

		internal int MaxStoresPerGroup
		{
			get
			{
				return (int)this[InformationStoreSchema.MaxStoresPerGroup];
			}
			set
			{
				this[InformationStoreSchema.MaxStoresPerGroup] = value;
			}
		}

		internal int MaxStoresTotal
		{
			get
			{
				return (int)this[InformationStoreSchema.MaxStoresTotal];
			}
			set
			{
				this[InformationStoreSchema.MaxStoresTotal] = value;
			}
		}

		internal int MaxRestoreStorageGroups
		{
			get
			{
				return (int)this[InformationStoreSchema.MaxRestoreStorageGroups];
			}
			set
			{
				this[InformationStoreSchema.MaxRestoreStorageGroups] = value;
			}
		}

		internal int? MinCachePages
		{
			get
			{
				return (int?)this[InformationStoreSchema.MinCachePages];
			}
			set
			{
				this[InformationStoreSchema.MinCachePages] = value;
			}
		}

		internal int? MaxCachePages
		{
			get
			{
				return (int?)this[InformationStoreSchema.MaxCachePages];
			}
			set
			{
				this[InformationStoreSchema.MaxCachePages] = value;
			}
		}

		public bool? EnableOnlineDefragmentation
		{
			get
			{
				return (bool?)this[InformationStoreSchema.EnableOnlineDefragmentation];
			}
		}

		internal int? MaxRpcThreads
		{
			get
			{
				return (int?)this[InformationStoreSchema.MaxRpcThreads];
			}
		}

		internal const int MaxStorageGroupsEnt = 100;

		internal const int MaxStoresPerGroupEnt = 5;

		internal const int MaxStoresTotalEnt = 100;

		internal const int MaxRestoreStorageGroupsEnt = 1;

		internal const int MaxStorageGroupsStd = 5;

		internal const int MaxStoresPerGroupStd = 5;

		internal const int MaxStoresTotalStd = 5;

		internal const int MaxRestoreStorageGroupsStd = 1;

		internal const int MaxStorageGroupsCoEx = 5;

		internal const int MaxStoresPerGroupCoEx = 5;

		internal const int MaxStoresTotalCoEx = 5;

		internal const int MaxRestoreStorageGroupsCoEx = 1;

		private static InformationStoreSchema schema = ObjectSchema.GetInstance<InformationStoreSchema>();

		private static string mostDerivedClass = "msExchInformationStore";
	}
}
