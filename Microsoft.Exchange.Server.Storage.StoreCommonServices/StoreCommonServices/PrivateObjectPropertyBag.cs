using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class PrivateObjectPropertyBag : ObjectPropertyBag
	{
		protected PrivateObjectPropertyBag(Context context, Table table, bool skipDataRowValidation, bool changeTrackingEnabled, bool newBag, bool writeThrough, params ColumnValue[] initialValues) : base(context, changeTrackingEnabled)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				DataRow dataRow = SharedObjectPropertyBagDataCache.LoadDataRow(context, newBag, table, writeThrough, initialValues);
				if (!skipDataRowValidation && dataRow != null && !this.IsValidDataRow(context, dataRow))
				{
					dataRow.Dispose();
					dataRow = null;
				}
				this.dataRow = dataRow;
				disposeGuard.Success();
			}
		}

		protected PrivateObjectPropertyBag(Context context, Table table, bool changeTrackingEnabled, bool writeThrough, Reader reader) : base(context, changeTrackingEnabled)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				DataRow dataRow = Factory.OpenDataRow(context.Culture, context, table, writeThrough, reader);
				if (dataRow != null && !this.IsValidDataRow(context, dataRow))
				{
					dataRow.Dispose();
					dataRow = null;
				}
				this.dataRow = dataRow;
				disposeGuard.Success();
			}
		}

		internal override DataRow DataRow
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

		protected override Dictionary<ushort, KeyValuePair<StorePropTag, object>> Properties
		{
			get
			{
				return this.properties;
			}
		}

		protected override bool PropertiesDirty
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

		protected virtual bool IsValidDataRow(Context context, DataRow dataRow)
		{
			return true;
		}

		protected override void AssignPropertiesToUse(Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties)
		{
			this.properties = properties;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.dataRow != null)
			{
				this.dataRow.Dispose();
				this.dataRow = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		private Dictionary<ushort, KeyValuePair<StorePropTag, object>> properties;

		private bool propertiesDirty;

		private DataRow dataRow;
	}
}
