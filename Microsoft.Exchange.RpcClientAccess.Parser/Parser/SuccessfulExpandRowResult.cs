using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulExpandRowResult : RopResult
	{
		internal SuccessfulExpandRowResult(int expandedRowCount, RowCollector rowCollector) : base(RopId.ExpandRow, ErrorCode.None, null)
		{
			this.expandedRowCount = expandedRowCount;
			this.columns = rowCollector.Columns;
			this.propertyRows = rowCollector.Rows;
		}

		internal SuccessfulExpandRowResult(Reader reader, PropertyTag[] columns, Encoding string8Encoding) : base(reader)
		{
			this.expandedRowCount = reader.ReadInt32();
			this.columns = columns;
			this.propertyRows = reader.ReadSizeAndPropertyRowList(columns, string8Encoding, WireFormatStyle.Rop);
		}

		internal PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		internal List<PropertyRow> PropertyRows
		{
			get
			{
				return this.propertyRows;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.expandedRowCount);
			writer.WriteSizeAndPropertyRowList(this.propertyRows, base.String8Encoding, WireFormatStyle.Rop);
		}

		private readonly int expandedRowCount;

		private readonly PropertyTag[] columns;

		private List<PropertyRow> propertyRows;
	}
}
