using System;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class ComponentDataStorageBase
	{
		internal bool IsEmpty
		{
			get
			{
				return this.slots == null;
			}
		}

		public object this[int slotNumber]
		{
			get
			{
				if (this.slots == null)
				{
					return null;
				}
				return this.slots[slotNumber];
			}
			set
			{
				if (this.slots == null && value == null)
				{
					return;
				}
				if (this.slots == null)
				{
					Interlocked.CompareExchange<object[]>(ref this.slots, new object[this.SlotCount], null);
				}
				this.slots[slotNumber] = value;
			}
		}

		public object CompareExchange(int slotNumber, object comparand, object value)
		{
			if (this.slots == null)
			{
				Interlocked.CompareExchange<object[]>(ref this.slots, new object[this.SlotCount], null);
			}
			return Interlocked.CompareExchange(ref this.slots[slotNumber], value, comparand);
		}

		internal void CleanupDataSlots(Context context)
		{
			if (this.slots == null)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.slots.Length; i++)
			{
				IComponentData componentData = this.slots[i] as IComponentData;
				if (componentData != null && !componentData.DoCleanup(context))
				{
					flag = false;
				}
				else
				{
					this.slots[i] = null;
				}
			}
			if (flag)
			{
				this.slots = null;
			}
		}

		internal abstract int SlotCount { get; }

		private object[] slots;
	}
}
