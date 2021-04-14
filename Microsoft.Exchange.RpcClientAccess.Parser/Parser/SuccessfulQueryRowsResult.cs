using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulQueryRowsResult : RopResult
	{
		internal PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		internal PropertyValue[][] Rows
		{
			get
			{
				return this.GetValues();
			}
		}

		internal BookmarkOrigin BookmarkOrigin
		{
			get
			{
				return this.bookmarkOrigin;
			}
		}

		internal SuccessfulQueryRowsResult(BookmarkOrigin bookmarkOrigin, RowCollector rowCollector) : base(RopId.QueryRows, ErrorCode.None, null)
		{
			this.bookmarkOrigin = bookmarkOrigin;
			this.columns = rowCollector.Columns;
			this.propertyRows = rowCollector.Rows;
		}

		internal SuccessfulQueryRowsResult(Reader reader, PropertyTag[] columns, Encoding string8Encoding) : base(reader)
		{
			this.columns = columns;
			this.bookmarkOrigin = (BookmarkOrigin)reader.ReadByte();
			this.propertyRows = reader.ReadSizeAndPropertyRowList(columns, string8Encoding, WireFormatStyle.Rop);
		}

		public override string ToString()
		{
			return string.Format("SuccessfulQueryRowsResult: [BookmarkOrigin: {0}] [Rows: {1}]", this.bookmarkOrigin, this.propertyRows.Count);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte((byte)this.bookmarkOrigin);
			writer.WriteSizeAndPropertyRowList(this.propertyRows, base.String8Encoding, WireFormatStyle.Rop);
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
			stringBuilder.Append(" Bookmark=").Append(this.bookmarkOrigin);
			stringBuilder.Append(" Rows=").Append((this.propertyRows != null) ? this.propertyRows.Count : 0);
		}

		internal const int BookmarkOriginSize = 1;

		private readonly BookmarkOrigin bookmarkOrigin;

		private readonly PropertyTag[] columns;

		private readonly List<PropertyRow> propertyRows;
	}
}
