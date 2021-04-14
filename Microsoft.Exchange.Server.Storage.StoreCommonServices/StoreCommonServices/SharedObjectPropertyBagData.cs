using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class SharedObjectPropertyBagData : DisposableBase, IStateObject
	{
		internal SharedObjectPropertyBagData(Context context, Mailbox mailbox, SharedObjectPropertyBagDataCache cache, ExchangeId propertyBagId, DataRow dataRow)
		{
			this.cache = cache;
			this.propertyBagId = propertyBagId;
			this.dataRow = dataRow;
			if (dataRow != null && dataRow.IsNew)
			{
				this.isOriginallyNew = true;
				this.OnDirty(context, mailbox);
			}
		}

		internal SharedObjectPropertyBagDataCache Cache
		{
			get
			{
				return this.cache;
			}
		}

		internal ExchangeId PropertyBagId
		{
			get
			{
				return this.propertyBagId;
			}
		}

		internal DataRow DataRow
		{
			get
			{
				return this.dataRow;
			}
			set
			{
				this.dataRow = value;
			}
		}

		internal Dictionary<ushort, KeyValuePair<StorePropTag, object>> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		internal bool PropertiesDirty
		{
			get
			{
				return this.propertiesDirty;
			}
			set
			{
				this.propertiesDirty = value;
			}
		}

		internal bool IsInUse
		{
			get
			{
				return this.usageCount > 0;
			}
		}

		internal bool IsActiveInTheCache
		{
			get
			{
				return this.isActiveInTheCache;
			}
			set
			{
				this.isActiveInTheCache = value;
			}
		}

		internal Mailbox ModifierMailbox
		{
			get
			{
				return this.modifierMailbox;
			}
		}

		internal static int AllocateComponentDataSlot()
		{
			return SharedObjectPropertyBagData.ComponentDataStorage.AllocateSlot();
		}

		internal void ResetModifierMailbox()
		{
			Interlocked.Exchange<Mailbox>(ref this.modifierMailbox, null);
		}

		public virtual void OnBeforeCommit(Context context)
		{
		}

		public virtual void OnCommit(Context context)
		{
			if (!base.IsDisposed)
			{
				this.isOriginallyNew = false;
				if (this.modifierMailbox != null)
				{
					this.ResetModifierMailbox();
				}
			}
		}

		public virtual void OnAbort(Context context)
		{
			if (!base.IsDisposed)
			{
				if (ExTraceGlobals.FolderTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.FolderTracer.TraceDebug<ExchangeId>(0L, "Discarding shared object property bag {0} cache on Abort", this.PropertyBagId);
				}
				if (this.isOriginallyNew)
				{
					if (this.dataRow != null)
					{
						this.dataRow.Dispose();
						this.dataRow = null;
					}
					this.ResetModifierMailbox();
					return;
				}
				this.DiscardCache(context);
			}
		}

		public void OnDirty(Context context, Mailbox modifierMailbox)
		{
			if (!context.IsStateObjectRegistered(this))
			{
				context.RegisterStateObject(this);
			}
			Mailbox mailbox = Interlocked.Exchange<Mailbox>(ref this.modifierMailbox, modifierMailbox);
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(mailbox == null || mailbox == modifierMailbox, "Concurrent modifier?");
		}

		internal void DiscardCache(Context context)
		{
			if (this.DataRow != null && !this.DataRow.IsDead)
			{
				this.DataRow.DiscardCache(context, true);
			}
			this.Properties = null;
			this.PropertiesDirty = false;
			this.componentData.CleanupDataSlots(context);
			this.ResetModifierMailbox();
		}

		internal void IncrementUsage()
		{
			this.usageCount++;
		}

		internal void DecrementUsage()
		{
			this.usageCount--;
		}

		internal object GetComponentData(int slotNumber)
		{
			return this.componentData[slotNumber];
		}

		internal void SetComponentData(int slotNumber, object value)
		{
			this.componentData[slotNumber] = value;
		}

		public object CompareExchangeComponentData(int slotNumber, object comparand, object value)
		{
			return this.componentData.CompareExchange(slotNumber, comparand, value);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.dataRow != null)
			{
				this.dataRow.Dispose();
				this.dataRow = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SharedObjectPropertyBagData>(this);
		}

		private SharedObjectPropertyBagDataCache cache;

		private ExchangeId propertyBagId;

		private Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties;

		private bool propertiesDirty;

		private DataRow dataRow;

		private int usageCount;

		private bool isActiveInTheCache;

		private bool isOriginallyNew;

		private SharedObjectPropertyBagData.ComponentDataStorage componentData = new SharedObjectPropertyBagData.ComponentDataStorage();

		private Mailbox modifierMailbox;

		private class ComponentDataStorage : ComponentDataStorageBase
		{
			internal static int AllocateSlot()
			{
				return Interlocked.Increment(ref SharedObjectPropertyBagData.ComponentDataStorage.nextAvailableSlot) - 1;
			}

			internal override int SlotCount
			{
				get
				{
					return SharedObjectPropertyBagData.ComponentDataStorage.nextAvailableSlot;
				}
			}

			private static int nextAvailableSlot;
		}
	}
}
