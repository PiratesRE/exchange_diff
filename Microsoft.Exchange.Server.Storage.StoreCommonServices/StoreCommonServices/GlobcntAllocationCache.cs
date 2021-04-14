using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class GlobcntAllocationCache : IComponentData
	{
		public GlobcntAllocationCache(ulong nextUnallocated, ulong maxReserved)
		{
			this.next = nextUnallocated;
			this.max = maxReserved;
		}

		public uint CountAvailable
		{
			get
			{
				return (uint)(this.max - this.next);
			}
		}

		public ulong Max
		{
			get
			{
				return this.max;
			}
		}

		public void SetMax(ulong maxReserved)
		{
			this.max = maxReserved;
		}

		public ulong Allocate(uint numAllocate)
		{
			ulong result = this.next;
			this.next += (ulong)numAllocate;
			return result;
		}

		bool IComponentData.DoCleanup(Context context)
		{
			return false;
		}

		private ulong next;

		private ulong max;
	}
}
