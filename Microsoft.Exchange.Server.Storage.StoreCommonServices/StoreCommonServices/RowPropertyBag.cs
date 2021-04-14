using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class RowPropertyBag : IRowPropertyBag, ISimpleReadOnlyPropertyBag, ISimpleReadOnlyPropertyStorage
	{
		public RowPropertyBag(Table table, ObjectPropertySchema propertySchema, StorePropTag mailboxNumberPropTag, IRowAccess rowAccess)
		{
			this.table = table;
			this.propertySchema = propertySchema;
			this.mailboxNumberPropTag = mailboxNumberPropTag;
			this.rowAccess = rowAccess;
		}

		protected void SetRowAccess(IRowAccess rowAccess)
		{
			this.rowAccess = rowAccess;
		}

		protected virtual ICollection<ushort> GetDefaultPromotedProperties(Context context)
		{
			IMailboxContext mailboxContext = context.PrimaryMailboxContext;
			if (mailboxContext == null || mailboxContext.IsUnifiedMailbox)
			{
				int mailboxNumber = (int)this.GetPropertyValue(context, this.mailboxNumberPropTag);
				mailboxContext = context.GetMailboxContext(mailboxNumber);
			}
			return mailboxContext.GetDefaultPromotedMessagePropertyIds(context);
		}

		public ObjectPropertySchema ObjectPropertySchema
		{
			get
			{
				return this.propertySchema;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		object IRowPropertyBag.GetPropertyValue(Connection connection, StorePropTag propTag)
		{
			return this.GetPropertyValue((Context)connection.OuterExecutionContext, propTag);
		}

		public object GetPropertyValue(Context context, StorePropTag propTag)
		{
			if (this.propertySchema != null)
			{
				return this.propertySchema.GetPropertyValue(context, this, propTag);
			}
			return this.GetBlobPropertyValue(context, propTag);
		}

		public virtual bool TryGetProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
		{
			throw new InvalidOperationException("This method should never be called for a RowPropertyBag");
		}

		public virtual void EnumerateProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
		{
			if (this.propertySchema != null)
			{
				this.propertySchema.EnumerateProperties(context, this, action, showValue);
				return;
			}
			this.EnumerateBlobProperties(context, action, showValue);
		}

		public object GetBlobPropertyValue(Context context, StorePropTag propTag)
		{
			object obj = null;
			if (this.table.SpecialCols.PropertyBlob != null)
			{
				byte[] blob = (byte[])this.rowAccess.GetPhysicalColumn(this.table.SpecialCols.PropertyBlob);
				PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(blob, 0);
				int index;
				if ((!blobReader.TryGetPropertyValueByTag(propTag.PropTag, out index, out obj) || blobReader.IsPropertyValueReference(index) || blobReader.GetPropertyAdditionalInfo(index) == AdditionalPropertyInfo.Truncated) && this.table.SpecialCols.OffPagePropertyBlob != null)
				{
					if (obj == null)
					{
						ICollection<ushort> defaultPromotedProperties = this.GetDefaultPromotedProperties(context);
						if (defaultPromotedProperties == null || !defaultPromotedProperties.Contains(propTag.PropId))
						{
							obj = ValueReference.Zero;
						}
					}
					if (obj != null)
					{
						if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.DbInteractionDetailTracer.TraceDebug<StorePropTag>(0L, "Property {0} caused us go to off-page blob", propTag);
						}
						if (this.ShouldReadBlobAsStream(this.table.SpecialCols.OffPagePropertyBlob))
						{
							return this.GetBlobPropertyValueFromBlobStream(context, propTag, this.table.SpecialCols.OffPagePropertyBlob);
						}
						blob = (byte[])this.rowAccess.GetPhysicalColumn(this.table.SpecialCols.OffPagePropertyBlob);
						blobReader = new PropertyBlob.BlobReader(blob, 0);
						obj = blobReader.GetPropertyValueByTag(propTag.PropTag);
					}
				}
			}
			return obj;
		}

		public virtual bool TryGetBlobProperty(Context context, ushort propId, out StorePropTag propTag, out object value)
		{
			throw new InvalidOperationException("This method should never be called for a RowPropertyBag");
		}

		object ISimpleReadOnlyPropertyStorage.GetPhysicalColumnValue(Context context, PhysicalColumn column)
		{
			return this.rowAccess.GetPhysicalColumn(column);
		}

		public void EnumerateBlobProperties(Context context, Func<StorePropTag, object, bool> action, bool showValue)
		{
			if (this.table.SpecialCols.PropertyBlob != null)
			{
				byte[] blob = (byte[])this.rowAccess.GetPhysicalColumn(this.table.SpecialCols.PropertyBlob);
				PropertyBlob.BlobReader onPageBlobReader = new PropertyBlob.BlobReader(blob, 0);
				for (int i = 0; i < onPageBlobReader.PropertyCount; i++)
				{
					if (!onPageBlobReader.IsPropertyValueNull(i) && !onPageBlobReader.IsPropertyValueReference(i))
					{
						StorePropTag arg = StorePropTag.CreateWithoutInfo(onPageBlobReader.GetPropertyTag(i));
						object arg2 = null;
						if (showValue)
						{
							arg2 = onPageBlobReader.GetPropertyValue(i);
						}
						if (!action(arg, arg2))
						{
							return;
						}
					}
				}
				if (this.table.SpecialCols.OffPagePropertyBlob != null)
				{
					if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Property enumeration caused us go to off-page blob");
					}
					if (this.ShouldReadBlobAsStream(this.table.SpecialCols.OffPagePropertyBlob))
					{
						this.EnumerateBlobPropertiesFromBlobStream(context, this.table.SpecialCols.OffPagePropertyBlob, onPageBlobReader, action, showValue);
						return;
					}
					byte[] blob2 = (byte[])this.rowAccess.GetPhysicalColumn(this.table.SpecialCols.OffPagePropertyBlob);
					PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(blob2, 0);
					for (int j = 0; j < blobReader.PropertyCount; j++)
					{
						StorePropTag arg3 = StorePropTag.CreateWithoutInfo(blobReader.GetPropertyTag(j));
						object arg4 = null;
						int index;
						if (!onPageBlobReader.TryFindPropertyById(arg3.PropId, out index) || onPageBlobReader.IsPropertyValueReference(index))
						{
							if (showValue)
							{
								arg4 = blobReader.GetPropertyValue(j);
							}
							if (!action(arg3, arg4))
							{
								return;
							}
						}
					}
				}
			}
		}

		private bool ShouldReadBlobAsStream(PhysicalColumn blobColumn)
		{
			return blobColumn.StreamSupport && this.rowAccess is IColumnStreamAccess && ((IColumnStreamAccess)this.rowAccess).GetColumnSize(blobColumn) >= 65536;
		}

		private object GetBlobPropertyValueFromBlobStream(Context context, StorePropTag propTag, PhysicalColumn blobColumn)
		{
			IColumnStreamAccess columnStreamAccess = (IColumnStreamAccess)this.rowAccess;
			if (columnStreamAccess.GetColumnSize(blobColumn) == 0)
			{
				return null;
			}
			object propertyValueByTag;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = new PhysicalColumnStream(columnStreamAccess, blobColumn, true);
				disposeGuard.Add<Stream>(stream);
				Stream stream2 = new BufferedStream(stream, 8192);
				disposeGuard.Add<Stream>(stream2);
				PropertyBlob.BlobStreamReader blobStreamReader = new PropertyBlob.BlobStreamReader(stream2);
				propertyValueByTag = blobStreamReader.GetPropertyValueByTag(propTag.PropTag);
			}
			return propertyValueByTag;
		}

		private void EnumerateBlobPropertiesFromBlobStream(Context context, PhysicalColumn blobColumn, PropertyBlob.BlobReader onPageBlobReader, Func<StorePropTag, object, bool> action, bool showValue)
		{
			IColumnStreamAccess columnStreamAccess = (IColumnStreamAccess)this.rowAccess;
			if (columnStreamAccess.GetColumnSize(blobColumn) == 0)
			{
				return;
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = new PhysicalColumnStream(columnStreamAccess, blobColumn, true);
				disposeGuard.Add<Stream>(stream);
				Stream stream2 = new BufferedStream(stream, 8192);
				disposeGuard.Add<Stream>(stream2);
				PropertyBlob.BlobStreamReader blobStreamReader = new PropertyBlob.BlobStreamReader(stream2);
				foreach (KeyValuePair<uint, object> keyValuePair in blobStreamReader.LoadProperties(showValue, false))
				{
					StorePropTag arg = StorePropTag.CreateWithoutInfo(keyValuePair.Key);
					int index;
					if ((!onPageBlobReader.TryFindPropertyById(arg.PropId, out index) || onPageBlobReader.IsPropertyValueReference(index)) && !action(arg, keyValuePair.Value))
					{
						break;
					}
				}
			}
		}

		private Table table;

		private ObjectPropertySchema propertySchema;

		private IRowAccess rowAccess;

		private StorePropTag mailboxNumberPropTag;
	}
}
