using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class SharedObjectPropertyBag : ObjectPropertyBag
	{
		protected SharedObjectPropertyBag(Context context, Table table, Mailbox mailbox, bool newBag, bool writeThrough, SharedObjectPropertyBagDataCache propertyBagDataCache, ExchangeId propertyBagId, params ColumnValue[] initialValues) : base(context, false)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (mailbox != null)
				{
					mailbox.IsValid();
					this.mailbox = mailbox;
				}
				else
				{
					this.mailbox = (Mailbox)this;
				}
				this.objectPropertyBagData = propertyBagDataCache.LoadPropertyBagData(context, this.mailbox, propertyBagId, newBag, table, writeThrough, initialValues);
				base.LoadData(context);
				this.Mailbox.AddPropertyBag(propertyBagId, this);
				if (this.DataRow != null)
				{
					this.OnAccess(context);
					if (!base.IsSaved)
					{
						this.OnDirty(context);
					}
				}
				disposeGuard.Success();
			}
		}

		internal override DataRow DataRow
		{
			get
			{
				if (this.objectPropertyBagData != null)
				{
					return this.objectPropertyBagData.DataRow;
				}
				return null;
			}
			set
			{
				this.objectPropertyBagData.DataRow = value;
			}
		}

		protected override Dictionary<ushort, KeyValuePair<StorePropTag, object>> Properties
		{
			get
			{
				if (this.objectPropertyBagData != null)
				{
					return this.objectPropertyBagData.Properties;
				}
				return null;
			}
		}

		protected override bool PropertiesDirty
		{
			get
			{
				return this.objectPropertyBagData != null && this.objectPropertyBagData.PropertiesDirty;
			}
			set
			{
				this.objectPropertyBagData.PropertiesDirty = value;
			}
		}

		public Mailbox Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return base.IsDirty && this.Mailbox == this.objectPropertyBagData.ModifierMailbox;
			}
		}

		public override Context CurrentOperationContext
		{
			get
			{
				return this.Mailbox.CurrentOperationContext;
			}
		}

		public override ReplidGuidMap ReplidGuidMap
		{
			get
			{
				return this.Mailbox.ReplidGuidMap;
			}
		}

		protected override StorePropTag MapPropTag(Context context, uint propertyTag)
		{
			return this.Mailbox.GetStorePropTag(context, propertyTag, this.GetObjectType());
		}

		protected override void AssignPropertiesToUse(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties)
		{
			using (LockManager.Lock(this.objectPropertyBagData, LockManager.LockType.LeafMonitorLock))
			{
				if (this.objectPropertyBagData.Properties == null)
				{
					this.objectPropertyBagData.Properties = properties;
				}
			}
		}

		internal static int AllocateComponentDataSlot()
		{
			return SharedObjectPropertyBagData.AllocateComponentDataSlot();
		}

		internal object GetComponentData(int slotNumber)
		{
			base.CheckNotDead();
			return this.objectPropertyBagData.GetComponentData(slotNumber);
		}

		internal void SetComponentData(int slotNumber, object value)
		{
			base.CheckNotDead();
			this.objectPropertyBagData.SetComponentData(slotNumber, value);
		}

		public object CompareExchangeComponentData(int slotNumber, object comparand, object value)
		{
			base.CheckNotDead();
			return this.objectPropertyBagData.CompareExchangeComponentData(slotNumber, comparand, value);
		}

		internal void OnMailboxDisconnect()
		{
			this.markedAsActive = false;
		}

		protected override void OnAccess(Context context)
		{
			if (!this.markedAsActive)
			{
				using (LockManager.Lock(this.objectPropertyBagData, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
				{
					this.DataRow.ReloadCacheIfDiscarded(context);
				}
				this.objectPropertyBagData.Cache.MarkAsActiveInTheCache(this.objectPropertyBagData, this.objectPropertyBagData.PropertyBagId);
				this.markedAsActive = true;
				this.Mailbox.OnPropertyBagActive(this);
			}
			base.OnAccess(context);
		}

		protected override void OnDirty(Context context)
		{
			MailboxState sharedState = this.Mailbox.SharedState;
			this.objectPropertyBagData.OnDirty(context, this.Mailbox);
			base.OnDirty(context);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.objectPropertyBagData != null)
			{
				this.Mailbox.RemovePropertyBag(this.objectPropertyBagData.PropertyBagId);
				this.objectPropertyBagData.Cache.ReleasePropertyBagData(this.objectPropertyBagData, this.objectPropertyBagData.PropertyBagId);
				this.objectPropertyBagData = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		public override void Flush(Context context, bool flushLargeDirtyStreams)
		{
			try
			{
				base.Flush(context, flushLargeDirtyStreams);
			}
			finally
			{
				if (!base.IsDirty)
				{
					this.objectPropertyBagData.ResetModifierMailbox();
				}
			}
		}

		protected override void Delete(Context context, bool notifyParent)
		{
			SharedObjectPropertyBagData sharedObjectPropertyBagData = null;
			try
			{
				sharedObjectPropertyBagData = this.objectPropertyBagData;
				base.Delete(context, notifyParent);
			}
			finally
			{
				sharedObjectPropertyBagData.ResetModifierMailbox();
			}
		}

		private readonly Mailbox mailbox;

		private SharedObjectPropertyBagData objectPropertyBagData;

		private bool markedAsActive;

		public struct NonDiscardableComponentData<T> : IComponentData where T : struct
		{
			public T Value { get; private set; }

			public NonDiscardableComponentData(T value)
			{
				this = default(SharedObjectPropertyBag.NonDiscardableComponentData<T>);
				this.Value = value;
			}

			bool IComponentData.DoCleanup(Context context)
			{
				return false;
			}
		}
	}
}
