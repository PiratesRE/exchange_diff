using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RowCollector : BaseObject
	{
		internal RowCollector(int maxSize, bool throwOnFirstAddFailure, Encoding string8Encoding)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (maxSize < 2)
				{
					throw new BufferTooSmallException();
				}
				this.maxSize = maxSize;
				this.propertyRows = new List<PropertyRow>();
				this.throwOnFirstAddFailure = throwOnFirstAddFailure;
				this.string8Encoding = string8Encoding;
				this.writer = new CountWriter();
				this.writer.WriteUInt16(0);
				disposeGuard.Success();
			}
		}

		internal RowCollector(int maxSize, PropertyTag[] columns, PropertyValue[][] rows) : this(maxSize, columns, rows, CodePageMap.GetCodePage(CTSGlobals.AsciiEncoding))
		{
		}

		internal RowCollector(int maxSize, PropertyTag[] columns, PropertyValue[][] rows, int codePageId) : this(maxSize, false, CodePageMap.GetEncoding(codePageId))
		{
			this.SetColumns(columns);
			foreach (PropertyValue[] rowValues in rows)
			{
				if (!this.TryAddRow(rowValues))
				{
					return;
				}
			}
		}

		internal int MaxSize
		{
			get
			{
				return this.maxSize;
			}
		}

		internal ushort RowCount
		{
			get
			{
				return (ushort)this.propertyRows.Count;
			}
		}

		internal PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		internal List<PropertyRow> Rows
		{
			get
			{
				return this.propertyRows;
			}
		}

		internal Encoding String8Encoding
		{
			get
			{
				return this.string8Encoding;
			}
		}

		public void SetColumns(PropertyTag[] columns)
		{
			base.CheckDisposed();
			if (this.columns == null || ArrayComparer<PropertyTag>.Comparer.Equals(this.columns, columns))
			{
				this.columns = columns;
				return;
			}
			throw new InvalidOperationException("Can't assign a different list of columns");
		}

		public bool TryAddRow(PropertyValue[] rowValues)
		{
			base.CheckDisposed();
			if (rowValues == null)
			{
				throw new ArgumentNullException("rowValues");
			}
			if (this.columns == null)
			{
				throw new InvalidOperationException("No columns were set before adding rows");
			}
			if (this.columns.Length != rowValues.Length)
			{
				string paramName = string.Format("rowValues does not contain the correct number of columns.  Expected: {0}  Found: {1}", this.columns.Length, rowValues.Length);
				throw new ArgumentException("rowValues", paramName);
			}
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection) && RowCollector.FaultInjectRowsOverflow(this.RowCount))
			{
				return false;
			}
			for (int i = 0; i < this.columns.Length; i++)
			{
				if (this.columns[i].PropertyId != rowValues[i].PropertyTag.PropertyId)
				{
					throw new ArgumentException(string.Format("Column {0} contains an incorrect ID.  Expected: {1}  Found: {2}", i, this.columns[i].PropertyId, rowValues[i].PropertyTag.PropertyId));
				}
				if (this.columns[i].PropertyType != PropertyType.Unspecified && rowValues[i].PropertyTag.PropertyType != PropertyType.Error && PropertyTag.RemoveMviWithMvIfNeeded(this.columns[i]).PropertyType != rowValues[i].PropertyTag.PropertyType)
				{
					throw new ArgumentException(string.Format("Column {0} contains an incorrect Type.  Expected: {1}  Found: {2}", i, this.columns[i].PropertyType, rowValues[i].PropertyTag.PropertyType));
				}
			}
			PropertyRow item = new PropertyRow(this.columns, rowValues);
			item.Serialize(this.writer, this.string8Encoding, WireFormatStyle.Rop);
			if (this.writer.Position <= (long)this.maxSize)
			{
				this.propertyRows.Add(item);
				return true;
			}
			if (this.RowCount == 0 && this.throwOnFirstAddFailure)
			{
				throw new BufferTooSmallException();
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Row Collector:" + Environment.NewLine);
			stringBuilder.AppendLine("MaxSize: " + this.MaxSize);
			if (this.columns != null)
			{
				stringBuilder.AppendLine("Number of Columns: " + this.columns.Length);
			}
			stringBuilder.AppendLine("Number of Rows: " + this.RowCount);
			return stringBuilder.ToString();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RowCollector>(this);
		}

		private static bool FaultInjectRowsOverflow(ushort rowCount)
		{
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2955291965U, ref num);
			return num > 0 && (int)rowCount >= num;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.writer);
			base.InternalDispose();
		}

		private readonly bool throwOnFirstAddFailure;

		private readonly int maxSize;

		private readonly Encoding string8Encoding;

		private readonly CountWriter writer;

		private PropertyTag[] columns;

		private List<PropertyRow> propertyRows;
	}
}
