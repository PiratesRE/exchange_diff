using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class ObjectPropertyBag : PropertyBag, IColumnValueBag, IDisposableImpl, IDisposable, IColumnStreamAccess
	{
		protected ObjectPropertyBag(Context context, bool changeTrackingEnabled) : base(changeTrackingEnabled)
		{
			this.disposableImpl = new DisposableImpl<ObjectPropertyBag>(this);
			this.renameDictionary = this.GetColumnRenames(context);
		}

		~ObjectPropertyBag()
		{
			this.disposableImpl.FinalizeImpl(this);
		}

		public bool IsDisposed
		{
			get
			{
				return this.disposableImpl.IsDisposed;
			}
		}

		public virtual bool NeedsToPublishNotification
		{
			get
			{
				return this.IsDirty;
			}
		}

		public bool IsSaved
		{
			get
			{
				return this.DataRow != null && !this.DataRow.IsNew;
			}
		}

		public bool IsDead
		{
			get
			{
				return this.DataRow == null || this.DataRow.IsDead;
			}
		}

		public Table Table
		{
			get
			{
				this.CheckNotDead();
				return this.DataRow.Table;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return !this.IsDead && (base.IsDirty || this.DataRow.IsDirty);
			}
		}

		protected virtual bool IsDirtyExceptDataRow
		{
			get
			{
				return base.IsDirty;
			}
		}

		public override bool IsChanged
		{
			get
			{
				this.CheckNotDead();
				return base.IsChanged || this.dataRowChanged;
			}
		}

		public bool OffPagePropertyBlobLoaded
		{
			get
			{
				return this.offpagePropertyBlobLoaded;
			}
		}

		internal abstract DataRow DataRow { get; set; }

		internal DataRow DataRowForTest
		{
			get
			{
				return this.DataRow;
			}
		}

		public override void MakeClean()
		{
			this.CheckNotDead();
			base.MakeClean();
		}

		public override void CommitChanges()
		{
			this.CheckNotDead();
			if (base.ChangeTrackingEnabled && this.IsChanged)
			{
				base.CommitChanges();
				this.dataRowChanged = false;
				this.changedColumns.SetAll(false);
				Array.Clear(this.originalColumnValues, 0, this.originalColumnValues.Length);
			}
		}

		public bool CheckTableExists(Context context)
		{
			return this.DataRow != null && this.DataRow.CheckTableExists(context);
		}

		protected abstract ObjectType GetObjectType();

		protected void LoadData(Context context)
		{
			if (this.DataRow != null)
			{
				this.LoadOnPageBlob(context);
				if (!this.IsSaved)
				{
					this.LoadOffPageBlob(context);
				}
			}
		}

		public override object GetBlobPropertyValue(Context context, StorePropTag propTag)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			object obj;
			if (base.TryGetBlobPropertyValue(context, propTag, out obj) && !(obj is ValueReference))
			{
				return obj;
			}
			if (this.OffPagePropertyBlobLoaded || (this.IsPropertyPromotedByDefault(context, propTag.PropId) && !(obj is ValueReference)))
			{
				return null;
			}
			this.LoadOffPageBlob(context);
			return base.GetBlobPropertyValue(context, propTag);
		}

		public override bool TryGetBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			if (base.TryGetBlobProperty(context, propId, out propTag, out value) && !(value is ValueReference))
			{
				return value != null;
			}
			if (this.OffPagePropertyBlobLoaded || (this.IsPropertyPromotedByDefault(context, propTag.PropId) && !(value is ValueReference)))
			{
				value = null;
				return false;
			}
			this.LoadOffPageBlob(context);
			return base.TryGetBlobProperty(context, propId, out propTag, out value);
		}

		public override object GetPhysicalColumnValue(Context context, PhysicalColumn column)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			Column column2;
			if (this.renameDictionary != null && this.renameDictionary.TryGetValue(column, out column2))
			{
				return column2.Evaluate(this);
			}
			return this.DataRow.GetValue(context, column);
		}

		public override void EnumerateBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			this.LoadOffPageBlobIfNecessary(context);
			base.EnumerateBlobProperties(context, action, showValue);
		}

		public override void SetBlobProperty(Context context, StorePropTag propTag, object value)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			if (!this.OffPagePropertyBlobLoaded)
			{
				StorePropTag storePropTag;
				object obj;
				bool flag = base.TryGetBlobProperty(context, propTag.PropId, out storePropTag, out obj);
				if ((!flag && !this.IsPropertyPromotedByDefault(context, propTag.PropId)) || (flag && obj is ValueReference))
				{
					this.LoadOffPageBlob(context);
				}
			}
			base.SetBlobProperty(context, propTag, value, !this.OffPagePropertyBlobLoaded);
		}

		public override void SetPhysicalColumn(Context context, PhysicalColumn column, object value)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			this.SetColumn(context, column, value);
		}

		public override ErrorCode OpenPhysicalColumnReadStream(Context context, PhysicalColumn column, out Stream stream)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			return this.OpenReadStream(context, column, out stream);
		}

		public override ErrorCode OpenPhysicalColumnWriteStream(Context context, PhysicalColumn column, out Stream stream)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			stream = this.OpenWriteStream(context, column);
			return ErrorCode.NoError;
		}

		public override bool IsPhysicalColumnChanged(Context context, PhysicalColumn column)
		{
			return this.IsColumnChanged(context, column);
		}

		public override object GetOriginalPhysicalColumnValue(Context context, PhysicalColumn column)
		{
			return this.GetOriginalColumnValue(context, column);
		}

		public object GetColumnValue(Context context, Column column)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			if (extendedPropertyColumn != null)
			{
				return this.GetPropertyValue(context, extendedPropertyColumn.StorePropTag);
			}
			return this.GetPhysicalColumnValue(context, (PhysicalColumn)column);
		}

		public bool CheckAlive(Context context)
		{
			return !this.IsDead;
		}

		public void SetColumn(Context context, PhysicalColumn column, object value)
		{
			this.CheckNotDead();
			this.OnAccess(context);
			if (!this.SaveColumnPreImage(context, column, true, value))
			{
				return;
			}
			int num = 0;
			bool flag = this.TrackSizeChangeForColumn(column);
			if (flag)
			{
				num = -this.DataRow.GetColumnSize(context, column).GetValueOrDefault();
			}
			if (!this.IsDirty)
			{
				this.OnDirty(context);
			}
			this.DataRow.SetValue(context, column, value);
			if (flag)
			{
				num += this.DataRow.GetColumnSize(context, column).GetValueOrDefault();
			}
			this.OnColumnChanged(column, (long)num);
		}

		protected void DirtyColumn(Context context, PhysicalColumn column)
		{
			this.CheckNotDead();
			if (!this.IsDirty)
			{
				this.OnDirty(context);
			}
			this.DataRow.DirtyValue(context, column);
		}

		public bool IsColumnChanged(Context context, Column column)
		{
			this.CheckNotDead();
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			if (extendedPropertyColumn != null)
			{
				return this.IsPropertyChanged(context, extendedPropertyColumn.StorePropTag);
			}
			if (!this.dataRowChanged)
			{
				return false;
			}
			Column column2;
			if (this.renameDictionary == null || !this.renameDictionary.TryGetValue(column, out column2))
			{
				return this.changedColumns[((PhysicalColumn)column).Index];
			}
			if (column2 is FunctionColumn)
			{
				object originalColumnValue = this.GetOriginalColumnValue(context, column);
				object obj = column2.Evaluate(this);
				return obj.Equals(originalColumnValue);
			}
			return this.IsColumnChanged(context, column2);
		}

		public object GetOriginalColumnValue(Context context, Column column)
		{
			this.CheckNotDead();
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			if (extendedPropertyColumn != null)
			{
				return this.GetOriginalPropertyValue(context, extendedPropertyColumn.StorePropTag);
			}
			Column column2;
			if (this.renameDictionary != null && this.renameDictionary.TryGetValue(column, out column2))
			{
				return column2.Evaluate(base.OriginalBag);
			}
			if (this.dataRowChanged && this.changedColumns[((PhysicalColumn)column).Index])
			{
				return this.originalColumnValues[((PhysicalColumn)column).Index];
			}
			return this.DataRow.GetValue(context, (PhysicalColumn)column);
		}

		protected Stream OpenWriteStream(Context context, PhysicalColumn column)
		{
			this.CheckNotDead();
			this.SetColumn(context, column, new byte[0]);
			return new PhysicalColumnStream(this, column, false);
		}

		private ErrorCode OpenReadStream(Context context, PhysicalColumn column, out Stream stream)
		{
			this.CheckNotDead();
			stream = null;
			if (this.DataRow.GetColumnSize(context, column) == null)
			{
				return ErrorCode.CreateNotFound((LID)44304U);
			}
			stream = new PhysicalColumnStream(this, column, true);
			return ErrorCode.NoError;
		}

		public void SetPrimaryKey(params ColumnValue[] primaryKeyValues)
		{
			this.CheckNotDead();
			this.DataRow.SetPrimaryKey(primaryKeyValues);
		}

		protected virtual void CopyOnWrite(Context context)
		{
		}

		internal virtual void AutoSave(Context context)
		{
		}

		public void Flush(Context context)
		{
			this.Flush(context, true);
		}

		public virtual void Flush(Context context, bool flushLargeDirtyStreams)
		{
			this.CheckNotDead();
			if (base.IsDirty || this.DataRow.IsNew)
			{
				this.UpdateBlobColumns(context);
			}
			if (this.DataRow.IsDirty)
			{
				this.OnBeforeDataRowFlushOrDelete(context, false);
				try
				{
					this.DataRow.Flush(context, flushLargeDirtyStreams);
				}
				catch (DuplicateKeyException exception)
				{
					context.OnExceptionCatch(exception);
					if (!this.RecoverFromDuplicateKey(context))
					{
						throw;
					}
					DiagnosticContext.TraceLocation((LID)64384U);
					this.DataRow.Flush(context, flushLargeDirtyStreams);
				}
				this.OnAfterDataRowFlushOrDelete(context, false);
			}
		}

		public virtual void Delete(Context context)
		{
			this.Delete(context, true);
		}

		protected virtual bool RecoverFromDuplicateKey(Context context)
		{
			return false;
		}

		protected virtual void Delete(Context context, bool notifyParent)
		{
			this.CheckNotDead();
			base.MakeClean();
			if (notifyParent)
			{
				this.OnBeforeDataRowFlushOrDelete(context, true);
			}
			try
			{
				this.DataRowDeletionImplementation(context);
			}
			finally
			{
				this.DataRow.Dispose();
				this.DataRow = null;
				if (notifyParent)
				{
					this.OnAfterDataRowFlushOrDelete(context, true);
				}
			}
		}

		protected virtual void DataRowDeletionImplementation(Context context)
		{
			this.DataRow.Delete(context);
		}

		public void MarkAsDeleted(Context context)
		{
			this.CheckNotDead();
			base.MakeClean();
			base.CommitChanges();
			this.DataRow.MarkAsDeleted(context);
			this.DataRow.Dispose();
			this.DataRow = null;
		}

		public override void DiscardPrivateCache(Context context)
		{
			this.dataRowChanged = false;
			this.changedColumns = null;
			this.originalColumnValues = null;
			if (!this.IsDead && this.Table.SpecialCols.OffPagePropertyBlob != null)
			{
				this.offpagePropertyBlobLoaded = false;
			}
			base.DiscardPrivateCache(context);
		}

		public override ulong GetChangedPropertyGroups(Context context)
		{
			ulong num = base.GetChangedPropertyGroups(context);
			if (this.dataRowChanged && this.Schema != null)
			{
				for (int i = 0; i < this.originalColumnValues.Length; i++)
				{
					if (this.changedColumns[i])
					{
						num |= this.Schema.ColumnGroups[i];
					}
				}
			}
			return num;
		}

		protected virtual bool TrackSizeChangeForColumn(Column column)
		{
			return false;
		}

		protected virtual bool TrackValueChangeForColumn(Column column)
		{
			return true;
		}

		protected virtual void OnColumnChanged(Column column, long deltaSize)
		{
		}

		protected virtual void OnBeforeDataRowFlushOrDelete(Context context, bool delete)
		{
		}

		protected virtual void OnAfterDataRowFlushOrDelete(Context context, bool delete)
		{
		}

		internal int ReadColumn(Context context, PhysicalColumn column, long position, byte[] buffer, int offset, int count)
		{
			this.CheckNotDead();
			return this.DataRow.ReadStream(context, column, position, buffer, offset, count);
		}

		internal void WriteColumn(Context context, PhysicalColumn column, long position, byte[] buffer, int offset, int count)
		{
			this.CheckNotDead();
			this.SaveColumnPreImage(context, column, false, null);
			if (this.DataRow.WriteThrough)
			{
				this.CopyOnWrite(context);
			}
			if (!this.IsDirty)
			{
				this.OnDirty(context);
			}
			long deltaSize;
			this.DataRow.WriteStream(context, column, position, buffer, offset, count, out deltaSize);
			this.OnColumnChanged(column, deltaSize);
		}

		internal int GetColumnLength(Context context, PhysicalColumn column)
		{
			this.CheckNotDead();
			return this.DataRow.GetColumnSize(context, column).GetValueOrDefault();
		}

		internal void LoadOffPageBlobForTest(Context context)
		{
			this.LoadOffPageBlobIfNecessary(context);
		}

		int IColumnStreamAccess.GetColumnSize(PhysicalColumn column)
		{
			return this.GetColumnLength(this.CurrentOperationContext, column);
		}

		int IColumnStreamAccess.ReadStream(PhysicalColumn physicalColumn, long position, byte[] buffer, int offset, int count)
		{
			return this.ReadColumn(this.CurrentOperationContext, physicalColumn, position, buffer, offset, count);
		}

		void IColumnStreamAccess.WriteStream(PhysicalColumn physicalColumn, long position, byte[] buffer, int offset, int count)
		{
			this.WriteColumn(this.CurrentOperationContext, physicalColumn, position, buffer, offset, count);
		}

		private void UpdateBlobColumns(Context context)
		{
			this.CheckNotDead();
			if (this.Table.SpecialCols.PropertyBlob != null || this.Table.SpecialCols.OffPagePropertyBlob != null)
			{
				if (!this.IsDirty)
				{
					this.OnDirty(context);
				}
				object obj = null;
				try
				{
					HashSet<ushort> additionalPromotedProperties = null;
					HashSet<ushort> hashSet = null;
					HashSet<ushort> alwaysPromotedProperties = null;
					if (this.Table.SpecialCols.OffPagePropertyBlob != null)
					{
						additionalPromotedProperties = this.GetAdditionalPromotedPropertyIds(context);
						hashSet = this.GetDefaultPromotedPropertyIds(context);
						alwaysPromotedProperties = this.GetAlwaysPromotedPropertyIds(context);
					}
					byte[] array;
					if (this.Table.SpecialCols.OffPagePropertyBlob != null && this.OffPagePropertyBlobLoaded)
					{
						PropertyBlob.BuildTwoBlobs(this.Properties, alwaysPromotedProperties, hashSet, additionalPromotedProperties, out array, out obj);
					}
					else
					{
						array = PropertyBlob.BuildBlob(this.Properties, hashSet);
						obj = null;
						if (this.Table.SpecialCols.OffPagePropertyBlob != null && array != null && array.Length > 3110)
						{
							this.LoadOffPageBlob(context);
							PropertyBlob.BuildTwoBlobs(this.Properties, alwaysPromotedProperties, hashSet, additionalPromotedProperties, out array, out obj);
						}
					}
					if (this.Table.SpecialCols.PropertyBlob != null)
					{
						this.DataRow.SetValue(context, this.Table.SpecialCols.PropertyBlob, array);
					}
					if (this.Table.SpecialCols.OffPagePropertyBlob != null && this.OffPagePropertyBlobLoaded)
					{
						this.DataRow.SetValue(context, this.Table.SpecialCols.OffPagePropertyBlob, obj);
						obj = null;
					}
				}
				finally
				{
					IDisposable disposable = obj as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			base.MakeClean();
		}

		private void LoadOnPageBlob(Context context)
		{
			this.CheckNotDead();
			if (this.Table.SpecialCols.PropertyBlob != null && this.Properties == null)
			{
				this.offpagePropertyBlobLoaded = false;
				byte[] propertyBlob = (byte[])this.DataRow.GetValue(context, this.Table.SpecialCols.PropertyBlob);
				Dictionary<ushort, KeyValuePair<StorePropTag, object>> dictionary = null;
				base.LoadPropertiesFromPropertyBlob(context, propertyBlob, ref dictionary);
				if (dictionary == null)
				{
					dictionary = new Dictionary<ushort, KeyValuePair<StorePropTag, object>>(3);
				}
				this.AssignPropertiesToUse(dictionary);
			}
			if (this.Table.SpecialCols.OffPagePropertyBlob == null)
			{
				this.offpagePropertyBlobLoaded = true;
			}
		}

		private void Refresh(Context context)
		{
			this.CheckNotDead();
			this.LoadOnPageBlob(context);
		}

		private void LoadOffPageBlob(Context context)
		{
			this.CheckNotDead();
			if (this.Table.SpecialCols.OffPagePropertyBlob != null)
			{
				object value = this.DataRow.GetValue(context, this.Table.SpecialCols.OffPagePropertyBlob);
				if (value != null)
				{
					Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties = this.Properties;
					byte[] array = value as byte[];
					if (array != null)
					{
						base.LoadPropertiesFromPropertyBlob(context, array, ref properties);
					}
					else
					{
						using (Stream stream = new PhysicalColumnStream(this, this.Table.SpecialCols.OffPagePropertyBlob, true))
						{
							base.LoadPropertiesFromPropertyBlobStream(context, stream, ref properties);
						}
					}
					if (properties != this.Properties)
					{
						this.AssignPropertiesToUse(properties);
					}
				}
				this.RemoveNullProperties();
			}
			this.offpagePropertyBlobLoaded = true;
		}

		public virtual HashSet<ushort> GetDefaultPromotedPropertyIds(Context context)
		{
			return null;
		}

		public virtual HashSet<ushort> GetAlwaysPromotedPropertyIds(Context context)
		{
			return null;
		}

		public virtual HashSet<ushort> GetAdditionalPromotedPropertyIds(Context context)
		{
			return null;
		}

		private bool IsPropertyPromotedByDefault(Context context, ushort propId)
		{
			HashSet<ushort> defaultPromotedPropertyIds = this.GetDefaultPromotedPropertyIds(context);
			return defaultPromotedPropertyIds != null && defaultPromotedPropertyIds.Contains(propId);
		}

		private void RemoveNullProperties()
		{
			if (this.Properties != null && this.Properties.Count != 0)
			{
				ushort[] array = new ushort[this.Properties.Count];
				int num = 0;
				foreach (KeyValuePair<ushort, KeyValuePair<StorePropTag, object>> keyValuePair in this.Properties)
				{
					if (keyValuePair.Value.Value == null || keyValuePair.Value.Value is ValueReference)
					{
						array[num++] = keyValuePair.Key;
					}
				}
				for (int i = 0; i < num; i++)
				{
					this.Properties.Remove(array[i]);
				}
			}
		}

		private bool SaveColumnPreImage(Context context, PhysicalColumn column, bool compareValue, object newValue)
		{
			this.CheckNotDead();
			bool flag = this.DataRow.ColumnFetched(column);
			if (compareValue && flag)
			{
				object value = this.DataRow.GetValue(context, column);
				if (ValueHelper.ValuesEqual(value, newValue))
				{
					return false;
				}
			}
			if (base.ChangeTrackingEnabled && this.TrackValueChangeForColumn(column))
			{
				if (this.dataRowChanged && this.changedColumns[column.Index])
				{
					if (compareValue && ValueHelper.ValuesEqual(this.originalColumnValues[column.Index], newValue))
					{
						this.changedColumns[column.Index] = false;
						this.originalColumnValues[column.Index] = null;
					}
				}
				else
				{
					object value2 = this.DataRow.GetValue(context, column);
					if (compareValue && !flag && ValueHelper.ValuesEqual(value2, newValue))
					{
						return false;
					}
					this.SaveColumnPreImage(column, value2);
				}
			}
			return true;
		}

		protected void SaveColumnPreImage(PhysicalColumn column, object currentValue)
		{
			if (this.changedColumns == null)
			{
				this.changedColumns = new BitArray(this.Table.Columns.Count);
				this.originalColumnValues = new object[this.Table.Columns.Count];
			}
			this.changedColumns[column.Index] = true;
			this.originalColumnValues[column.Index] = currentValue;
			this.dataRowChanged = true;
		}

		protected void ForgetColumnPreImage(PhysicalColumn column)
		{
			if (this.changedColumns == null)
			{
				return;
			}
			this.changedColumns[column.Index] = false;
			this.originalColumnValues[column.Index] = null;
		}

		public override void Scrub(Context context)
		{
			this.LoadOffPageBlobIfNecessary(context);
			base.Scrub(context);
		}

		public void Dispose()
		{
			this.disposableImpl.DisposeImpl(this);
		}

		protected void CheckDisposed()
		{
			this.disposableImpl.CheckDisposed(this);
		}

		protected virtual DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ObjectPropertyBag>(this);
		}

		protected virtual void InternalDispose(bool calledFromDispose)
		{
		}

		protected virtual IReadOnlyDictionary<Column, Column> GetColumnRenames(Context context)
		{
			return null;
		}

		DisposeTracker IDisposableImpl.InternalGetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		void IDisposableImpl.InternalDispose(bool calledFromDispose)
		{
			this.InternalDispose(calledFromDispose);
		}

		protected void CheckNotDead()
		{
			if (this.IsDead)
			{
				throw new StoreException((LID)34936U, ErrorCodeValue.ObjectDeleted, "Object is already deleted");
			}
		}

		private void LoadOffPageBlobIfNecessary(Context context)
		{
			if (!this.OffPagePropertyBlobLoaded)
			{
				this.LoadOffPageBlob(context);
			}
		}

		protected virtual void OnAccess(Context context)
		{
			if (this.Properties == null && !this.IsDirty && !base.ChangeTrackingEnabled)
			{
				this.Refresh(context);
			}
		}

		private readonly IReadOnlyDictionary<Column, Column> renameDictionary;

		private DisposableImpl<ObjectPropertyBag> disposableImpl;

		protected bool offpagePropertyBlobLoaded;

		protected bool dataRowChanged;

		private BitArray changedColumns;

		private object[] originalColumnValues;
	}
}
