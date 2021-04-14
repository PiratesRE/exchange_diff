using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class Item : PrivateObjectPropertyBag
	{
		protected Item(Context context, Table table, PhysicalColumn sizeColumn, Mailbox mailbox, bool changeTrackingEnabled, bool newItem, bool existsInDatabase, bool writeThrough, params ColumnValue[] initialValues) : base(context, table, newItem, changeTrackingEnabled, !existsInDatabase, writeThrough, initialValues)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				mailbox.IsValid();
				this.mailbox = mailbox;
				if (!base.IsDead)
				{
					base.LoadData(context);
					this.sizeColumn = sizeColumn;
					this.isNew = newItem;
					if (existsInDatabase)
					{
						byte[] array = (byte[])this.GetPropertyValue(context, this.ItemSubobjectsBinPropTag);
						if (array != null)
						{
							this.subobjects = new SubobjectCollection(this, array);
						}
						this.currentSize = (long)base.GetColumnValue(context, sizeColumn);
						if (!newItem)
						{
							this.originalSize = this.currentSize;
						}
					}
					else
					{
						base.SetColumn(context, sizeColumn, this.currentSize);
					}
					disposeGuard.Success();
				}
			}
		}

		protected Item(Context context, Table table, PhysicalColumn sizeColumn, Mailbox mailbox, bool changeTrackingEnabled, bool writeThrough, Reader reader) : base(context, table, changeTrackingEnabled, writeThrough, reader)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				mailbox.IsValid();
				this.mailbox = mailbox;
				if (!base.IsDead)
				{
					base.LoadData(context);
					this.sizeColumn = sizeColumn;
					this.isNew = false;
					byte[] array = (byte[])this.GetPropertyValue(context, this.ItemSubobjectsBinPropTag);
					if (array != null)
					{
						this.subobjects = new SubobjectCollection(this, array);
					}
					this.currentSize = (long)base.GetColumnValue(context, sizeColumn);
					this.originalSize = this.currentSize;
					disposeGuard.Success();
				}
			}
		}

		public bool IsNew
		{
			get
			{
				return this.isNew;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return base.IsDirty || (this.subobjects != null && this.subobjects.IsDirty);
			}
		}

		protected override bool IsDirtyExceptDataRow
		{
			get
			{
				return base.IsDirtyExceptDataRow || (this.subobjects != null && this.subobjects.IsDirty);
			}
		}

		public Mailbox Mailbox
		{
			get
			{
				return this.mailbox;
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

		public long CurrentSize
		{
			get
			{
				return this.currentSize;
			}
		}

		public long CurrentSizeEstimateWithoutChildren
		{
			get
			{
				long num = this.CurrentSize;
				if (this.Subobjects != null)
				{
					foreach (int childNumber in this.Subobjects.GetChildNumbers())
					{
						long childSize = this.Subobjects.GetChildSize(childNumber);
						if (childSize != -1L)
						{
							num -= childSize;
						}
					}
				}
				return num;
			}
		}

		public long OriginalSize
		{
			get
			{
				return this.originalSize;
			}
		}

		public int ChildrenCount
		{
			get
			{
				if (this.subobjects != null)
				{
					return this.subobjects.ChildrenCount;
				}
				return 0;
			}
		}

		public int DescendantCount
		{
			get
			{
				if (this.subobjects != null)
				{
					return this.subobjects.DescendantCount;
				}
				return 0;
			}
		}

		public SubobjectCollection Subobjects
		{
			get
			{
				return this.subobjects;
			}
		}

		public int SubobjectsChangeCookie
		{
			get
			{
				return this.subobjectsChangeCookie;
			}
		}

		public override ObjectPropertySchema Schema
		{
			get
			{
				if (this.propertySchema == null)
				{
					this.propertySchema = PropertySchema.GetObjectSchema(this.Mailbox.Database, this.GetObjectType());
				}
				return this.propertySchema;
			}
		}

		protected abstract StorePropTag ItemSubobjectsBinPropTag { get; }

		internal SubobjectReferenceState SubobjectReferenceState
		{
			get
			{
				if (this.subobjectReferenceState == null)
				{
					this.subobjectReferenceState = SubobjectReferenceState.GetState(this.Mailbox, true);
				}
				return this.subobjectReferenceState;
			}
		}

		protected int ReserveChildNumber()
		{
			if (this.subobjects == null)
			{
				this.subobjects = new SubobjectCollection(this);
			}
			return this.subobjects.ReserveChildNumber();
		}

		protected void UpdateSubobjects(Context context)
		{
			if (this.subobjects != null && this.subobjects.IsDirty)
			{
				byte[] value = this.subobjects.Serialize(true, true);
				this.SetProperty(context, this.ItemSubobjectsBinPropTag, value);
			}
		}

		protected override StorePropTag MapPropTag(Context context, uint propertyTag)
		{
			return this.Mailbox.GetStorePropTag(context, propertyTag, this.GetObjectType());
		}

		public override void Flush(Context context, bool flushLargeDirtyStreams)
		{
			if (this.IsDirty)
			{
				this.CopyOnWrite(context);
				this.UpdateSubobjects(context);
				base.SetColumn(context, this.sizeColumn, this.currentSize);
				base.Flush(context, flushLargeDirtyStreams);
			}
		}

		public int GetLargeDirtyStreamsSize()
		{
			return this.DataRow.GetLargeDirtyStreamsSize();
		}

		public virtual IChunked PrepareToSaveChanges(Context context)
		{
			this.Flush(context, false);
			return new ChunkedEnumerable("stream flush", this.DataRow.FlushDirtyStreams(context), this.Mailbox.SharedState, TimeSpan.FromMilliseconds(10.0), TimeSpan.FromMilliseconds(1500.0));
		}

		public virtual bool SaveChanges(Context context)
		{
			this.CopyOnWrite(context);
			this.Flush(context, true);
			this.isNew = false;
			this.CommitChanges();
			this.originalSize = this.currentSize;
			return true;
		}

		internal virtual void SaveChild(Context context, ISubobject child)
		{
			if (base.IsDead)
			{
				throw new StoreException((LID)56952U, ErrorCodeValue.ObjectDeleted);
			}
			long sizeChange = child.CurrentSize - this.subobjects.GetChildSize(child.ChildNumber);
			this.SizeChange(sizeChange);
			long? childInid = this.subobjects.GetChildInid(child.ChildNumber);
			long value = child.CurrentInid.Value;
			this.subobjects.AddOrUpdateChild(context, child.ChildNumber, value, child.CurrentSize);
			if (child.OriginalInid == childInid)
			{
				if (child.Subobjects != null)
				{
					this.subobjects.Add(context, child.Subobjects);
					this.subobjects.DeleteDeleted(context, child.Subobjects);
				}
			}
			else
			{
				if (childInid != null)
				{
					using (Item item = this.OpenChild(context, child.ChildNumber, childInid.Value))
					{
						if (item.Subobjects != null)
						{
							this.subobjects.Delete(context, item.Subobjects);
						}
					}
				}
				if (child.Subobjects != null)
				{
					this.subobjects.Add(context, child.Subobjects);
				}
			}
			this.subobjectsChangeCookie++;
		}

		internal virtual void DeleteChild(Context context, ISubobject child)
		{
			if (base.IsDead)
			{
				throw new StoreException((LID)44664U, ErrorCodeValue.ObjectDeleted);
			}
			long? childInid = this.subobjects.GetChildInid(child.ChildNumber);
			if (childInid == null)
			{
				return;
			}
			if (child.OriginalInid == childInid)
			{
				if (child.Subobjects != null)
				{
					this.subobjects.Delete(context, child.Subobjects);
					this.subobjects.DeleteDeleted(context, child.Subobjects);
				}
			}
			else
			{
				using (Item item = this.OpenChild(context, child.ChildNumber, childInid.Value))
				{
					if (item.Subobjects != null)
					{
						this.subobjects.Delete(context, item.Subobjects);
					}
				}
			}
			this.SizeChange(-this.subobjects.GetChildSize(child.ChildNumber));
			this.subobjects.DeleteChild(context, child.ChildNumber, child.CurrentSize);
			this.subobjectsChangeCookie++;
		}

		public override void Scrub(Context context)
		{
			if (this.subobjects != null)
			{
				foreach (int childNumber in this.subobjects.GetChildNumbers())
				{
					long value = this.subobjects.GetChildInid(childNumber).Value;
					using (Item item = this.OpenChild(context, childNumber, value))
					{
						item.Delete(context);
					}
				}
				this.subobjects.ResetNextChildNumber();
				this.subobjects.SetDirty();
			}
			base.Scrub(context);
		}

		protected void DeepCopySubobjects(Context context)
		{
			if (this.subobjects != null)
			{
				foreach (int childNumber in this.subobjects.GetChildNumbers())
				{
					long value = this.subobjects.GetChildInid(childNumber).Value;
					using (Item item = this.CopyChild(context, childNumber, value))
					{
						item.SaveChanges(context);
					}
				}
			}
		}

		protected abstract Item OpenChild(Context context, int childNumber, long inid);

		protected abstract Item CopyChild(Context context, int childNumber, long inid);

		protected override bool TrackSizeChangeForColumn(Column column)
		{
			return true;
		}

		protected override bool TrackValueChangeForColumn(Column column)
		{
			return true;
		}

		protected override void OnDirty(Context context)
		{
			base.OnDirty(context);
		}

		protected override void OnColumnChanged(Column column, long deltaSize)
		{
			this.SizeChange(deltaSize);
		}

		protected override void OnPropertyChanged(StorePropTag propTag, long deltaSize)
		{
			this.SizeChange(deltaSize);
		}

		protected void ResetIsNew()
		{
			this.isNew = false;
		}

		internal void SizeChange(long sizeChange)
		{
			this.currentSize += sizeChange;
		}

		protected void ReleaseDescendants(Context context, bool calledFromFinalizer)
		{
			SubobjectCollection subobjectCollection = Interlocked.Exchange<SubobjectCollection>(ref this.subobjects, null);
			if (subobjectCollection != null)
			{
				subobjectCollection.ReleaseAll(calledFromFinalizer, context);
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			this.ReleaseDescendants((calledFromDispose && this.Mailbox != null) ? this.Mailbox.CurrentOperationContext : null, !calledFromDispose);
			base.InternalDispose(calledFromDispose);
		}

		[Conditional("DEBUG")]
		internal void AssertHasChild(long inid)
		{
		}

		[Conditional("DEBUG")]
		internal void AssertHasAllDescendants(SubobjectCollection subobjects)
		{
		}

		private readonly Mailbox mailbox;

		private PhysicalColumn sizeColumn;

		private bool isNew;

		private long currentSize;

		private long originalSize;

		private SubobjectCollection subobjects;

		private int subobjectsChangeCookie;

		private SubobjectReferenceState subobjectReferenceState;

		private ObjectPropertySchema propertySchema;
	}
}
