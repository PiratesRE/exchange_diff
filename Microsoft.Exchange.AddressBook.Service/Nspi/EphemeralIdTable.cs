using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.AddressBook.Nspi
{
	internal class EphemeralIdTable
	{
		internal static bool IsActiveDirectoryEphemeralId(int mid)
		{
			return mid >= 16;
		}

		internal static EphemeralIdTable.NamingContext GetNamingContext(ADRawEntry rawEntry)
		{
			return EphemeralIdTable.GetNamingContext((ADObjectId)rawEntry[ADObjectSchema.Id]);
		}

		internal static EphemeralIdTable.NamingContext GetNamingContext(ADObjectId id)
		{
			if (id.IsDescendantOf(ADSession.GetConfigurationNamingContext(id.GetPartitionId().ForestFQDN)))
			{
				if (!ADSession.IsTenantIdentity(id, id.GetPartitionId().ForestFQDN))
				{
					return EphemeralIdTable.NamingContext.Config;
				}
				return EphemeralIdTable.NamingContext.TenantConfig;
			}
			else
			{
				if (!ADSession.IsTenantIdentity(id, id.GetPartitionId().ForestFQDN))
				{
					return EphemeralIdTable.NamingContext.Domain;
				}
				if (!id.IsDescendantOf(ADSession.GetConfigurationUnitsRoot(id.GetPartitionId().ForestFQDN)))
				{
					return EphemeralIdTable.NamingContext.Domain;
				}
				return EphemeralIdTable.NamingContext.TenantConfig;
			}
		}

		internal int CreateEphemeralId(Guid guid, EphemeralIdTable.NamingContext namingContext)
		{
			int count;
			if (!this.guidToEphemeralId.TryGetValue(guid, out count))
			{
				count = this.ephemeralIdToEntry.Count;
				this.ephemeralIdToEntry.Add(new EphemeralIdTable.Entry(guid, namingContext));
				this.guidToEphemeralId.Add(guid, count);
			}
			return -(count + 16);
		}

		internal bool GetGuid(int id, out Guid guid, out EphemeralIdTable.NamingContext namingContext)
		{
			if (!this.IsAddressBookEphemeralId(id))
			{
				guid = EphemeralIdTable.InvalidGuid;
				namingContext = EphemeralIdTable.NamingContext.Invalid;
				return false;
			}
			id = -id;
			EphemeralIdTable.Entry entry = this.ephemeralIdToEntry[id - 16];
			guid = entry.Guid;
			namingContext = entry.NamingContext;
			return true;
		}

		internal void ConvertIdsToGuids(int[] ids, out Guid[] guids, out EphemeralIdTable.NamingContext[] contexts)
		{
			guids = new Guid[ids.Length];
			contexts = new EphemeralIdTable.NamingContext[ids.Length];
			for (int i = 0; i < ids.Length; i++)
			{
				Guid guid;
				EphemeralIdTable.NamingContext namingContext;
				this.GetGuid(ids[i], out guid, out namingContext);
				guids[i] = guid;
				contexts[i] = namingContext;
			}
		}

		internal bool IsAddressBookEphemeralId(int mid)
		{
			if (mid > -16)
			{
				return false;
			}
			mid = -mid;
			return mid >= 16 && mid < 16 + this.ephemeralIdToEntry.Count;
		}

		internal void AddIdMapping(int mid1, int mid2)
		{
			if (this.midMappingTable == null)
			{
				this.midMappingTable = new Dictionary<int, int>(4);
			}
			this.midMappingTable[mid1] = mid2;
			this.midMappingTable[mid2] = mid1;
		}

		internal bool TryGetMapping(int inputMid, out int outputMid)
		{
			if (this.midMappingTable == null || !this.midMappingTable.TryGetValue(inputMid, out outputMid))
			{
				outputMid = -1;
				return false;
			}
			return true;
		}

		internal const int InvalidId = -1;

		internal const int NoGalId = -2;

		internal const int GlobalAddressList = 0;

		internal const int Beginning = 0;

		internal const int Current = 1;

		internal const int End = 2;

		private const int InitialSize = 20;

		private const int FirstEphemeralId = 16;

		private const int FirstActiveDirectoryEphemeralId = 16;

		private const int FirstAddressBookEphemeralId = -16;

		internal static readonly Guid InvalidGuid = new Guid(uint.MaxValue, ushort.MaxValue, ushort.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private Dictionary<Guid, int> guidToEphemeralId = new Dictionary<Guid, int>(20);

		private List<EphemeralIdTable.Entry> ephemeralIdToEntry = new List<EphemeralIdTable.Entry>(20);

		private Dictionary<int, int> midMappingTable;

		internal enum NamingContext
		{
			Invalid,
			Domain,
			Config,
			TenantDomain,
			TenantConfig
		}

		private class Entry
		{
			internal Guid Guid { get; set; }

			internal EphemeralIdTable.NamingContext NamingContext { get; set; }

			internal Entry(Guid guid, EphemeralIdTable.NamingContext namingContext)
			{
				this.Guid = guid;
				this.NamingContext = namingContext;
			}
		}
	}
}
