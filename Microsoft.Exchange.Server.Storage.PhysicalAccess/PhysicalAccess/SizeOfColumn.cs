using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class SizeOfColumn : Column
	{
		protected SizeOfColumn(string name, Column termColumn, bool compressedSize) : base(name, typeof(int), termColumn.IsNullable, Visibility.Public, 0, 4, termColumn.Table)
		{
			this.termColumn = termColumn;
			this.compressedSize = compressedSize;
			base.CacheHashCode();
		}

		public Column TermColumn
		{
			get
			{
				return this.termColumn;
			}
		}

		public bool CompressedSize
		{
			get
			{
				return this.compressedSize;
			}
		}

		public static int? GetColumnSize(Column column, object value)
		{
			if (value == null)
			{
				return null;
			}
			if (column.Size != 0)
			{
				return new int?(column.Size);
			}
			if (column.Type == typeof(string))
			{
				return new int?(((string)value).Length * 2);
			}
			if (column.Type == typeof(byte[]))
			{
				return new int?(((byte[])value).Length);
			}
			if (column.Type == typeof(byte[][]))
			{
				int num = 0;
				foreach (byte[] array2 in (byte[][])value)
				{
					if (array2 != null)
					{
						num += array2.Length;
					}
				}
				return new int?(num);
			}
			if (column.Type == typeof(short[]))
			{
				return new int?(((short[])value).Length * 2);
			}
			throw new NotSupportedException(string.Format("Don't know how to retrieve the size of this data type: {0}", value.GetType()));
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state)
		{
			callback(this, state);
			this.termColumn.EnumerateColumns(callback, state);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			if (this.Name != null)
			{
				sb.Append(this.Name);
			}
			if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || this.Name == null)
			{
				if (this.Name != null)
				{
					sb.Append(":");
				}
				if (this.compressedSize)
				{
					sb.Append("COMPRESSED");
				}
				sb.Append("SIZEOF[");
				this.termColumn.AppendToString(sb, formatOptions);
				sb.Append("]");
			}
		}

		protected internal override bool ActualColumnEquals(Column other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			SizeOfColumn sizeOfColumn = other as SizeOfColumn;
			return sizeOfColumn != null && this.Name == sizeOfColumn.Name && this.TermColumn == sizeOfColumn.TermColumn && this.CompressedSize == sizeOfColumn.CompressedSize;
		}

		protected override int CalculateHashCode()
		{
			int num = 305419896;
			if (this.Name != null)
			{
				num ^= this.Name.GetHashCode();
			}
			num ^= this.TermColumn.GetHashCode();
			return num ^ this.CompressedSize.GetHashCode();
		}

		protected override int GetSize(ITWIR context)
		{
			return 4;
		}

		protected override object GetValue(ITWIR context)
		{
			return context.GetColumnSize(this.TermColumn);
		}

		private readonly Column termColumn;

		private readonly bool compressedSize;
	}
}
