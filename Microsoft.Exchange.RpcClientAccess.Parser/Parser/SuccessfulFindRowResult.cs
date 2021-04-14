using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFindRowResult : RopResult
	{
		internal PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		internal bool PositionChanged
		{
			get
			{
				return this.positionChanged;
			}
		}

		internal SuccessfulFindRowResult(bool positionChanged, RowCollector rowCollector) : base(RopId.FindRow, ErrorCode.None, null)
		{
			this.positionChanged = positionChanged;
			if (rowCollector.RowCount > 1)
			{
				throw new ArgumentException("RowCollector can only have one row for SuccessfulFindRowResult");
			}
			this.columns = rowCollector.Columns;
			this.propertyRows = rowCollector.Rows;
		}

		internal SuccessfulFindRowResult(Reader reader, PropertyTag[] columns, Encoding string8Encoding) : base(reader)
		{
			this.columns = columns;
			this.positionChanged = reader.ReadBool();
			byte b = reader.ReadBool() ? 1 : 0;
			this.propertyRows = new List<PropertyRow>((int)b);
			for (int i = 0; i < (int)b; i++)
			{
				PropertyRow item = PropertyRow.Parse(reader, columns, WireFormatStyle.Rop);
				item.ResolveString8Values(string8Encoding);
				this.propertyRows.Add(item);
			}
			base.String8Encoding = string8Encoding;
		}

		public override string ToString()
		{
			return string.Format("SuccessfulFindRowResult: [PositionChanged: {0}]", this.positionChanged);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBool(this.positionChanged, 1);
			writer.WriteBool(this.propertyRows.Count > 0, 1);
			foreach (PropertyRow propertyRow in this.propertyRows)
			{
				propertyRow.Serialize(writer, base.String8Encoding, WireFormatStyle.Rop);
			}
		}

		internal PropertyValue[][] GetValues()
		{
			PropertyValue[][] array = new PropertyValue[this.propertyRows.Count][];
			for (int i = 0; i < this.propertyRows.Count; i++)
			{
				array[i] = this.propertyRows[i].PropertyValues;
			}
			return array;
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" PositionChanged=").Append(this.positionChanged);
			stringBuilder.Append(" RowCount=").Append((this.propertyRows != null) ? this.propertyRows.Count : 0);
		}

		internal const int PositionChangedSize = 1;

		internal const int RowReturnedSize = 1;

		private readonly bool positionChanged;

		private readonly PropertyTag[] columns;

		private readonly List<PropertyRow> propertyRows;
	}
}
