using System;
using System.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct ColumnValue
	{
		public ColumnValue(Column column, object value)
		{
			this.column = column;
			this.value = value;
		}

		public Column Column
		{
			get
			{
				return this.column;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public static bool operator ==(ColumnValue cv1, ColumnValue cv2)
		{
			return cv1.Equals(cv2);
		}

		public static bool operator !=(ColumnValue cv1, ColumnValue cv2)
		{
			return !(cv1 == cv2);
		}

		public override int GetHashCode()
		{
			return this.column.GetHashCode();
		}

		public override bool Equals(object other)
		{
			if (other is ColumnValue)
			{
				ColumnValue columnValue = (ColumnValue)other;
				if (object.Equals(columnValue.Column, this.Column))
				{
					return ValueHelper.ValuesEqual(columnValue.Value, this.Value);
				}
			}
			return false;
		}

		[Conditional("DEBUG")]
		private void ValidateColumnValue()
		{
			Type type;
			if (this.value != null)
			{
				type = this.value.GetType();
			}
			else
			{
				type = typeof(DBNull);
			}
			if (this.value != null && (this.column.Type == typeof(string) || this.column.Type.IsArray))
			{
				if (type == typeof(string))
				{
					string text = (string)this.value;
					return;
				}
				if (type.IsArray)
				{
					Array array = (Array)this.value;
				}
			}
		}

		private readonly Column column;

		private readonly object value;
	}
}
