using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class ReferenceItem
	{
		public ReferenceItem(SortBy column, object value, long secondarySortValue)
		{
			Util.ThrowOnNull(column, "column");
			Util.ThrowOnNull(value, "value");
			this.sortBy = column;
			Type type = Nullable.GetUnderlyingType(column.ColumnDefinition.Type) ?? column.ColumnDefinition.Type;
			if (type.Equals(typeof(ExDateTime)))
			{
				this.sortColumnValue = ExDateTime.Parse(((value is ExDateTime) ? ((ExDateTime)value).UniversalTime : ExDateTime.MinValue.UniversalTime).ToString(ReferenceItem.DateTimeWithoutMillisecondsFormatString, DateTimeFormatInfo.InvariantInfo));
			}
			else
			{
				this.sortColumnValue = value;
			}
			this.secondarySortValue = secondarySortValue;
		}

		public long SecondarySortValue
		{
			get
			{
				return this.secondarySortValue;
			}
		}

		public PropertyDefinition SortColumn
		{
			get
			{
				return this.sortBy.ColumnDefinition;
			}
		}

		public SortBy SortBy
		{
			get
			{
				return this.sortBy;
			}
		}

		public object SortColumnValue
		{
			get
			{
				return this.sortColumnValue;
			}
		}

		public int MailboxIdHash
		{
			get
			{
				return (int)(this.secondarySortValue >> 32);
			}
		}

		public int DocId
		{
			get
			{
				return (int)(this.secondarySortValue & (long)((ulong)-1));
			}
		}

		public static ReferenceItem Parse(SortBy sortBy, string serializedItem)
		{
			int num = -1;
			if (!int.TryParse(serializedItem.Substring(0, ReferenceItem.LengthOfSortColumnValueLengthPart), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
			{
				throw new ArgumentException("Invalid serialized item as the sort column value length is invalid");
			}
			if (num < 0 || num > ReferenceItem.MaximumStringPropertyLength)
			{
				throw new ArgumentException("Invalid serialized item as the sort column value length is negative or greater than 256");
			}
			if (serializedItem.Length != ReferenceItem.LengthOfSortColumnValueLengthPart + num + ReferenceItem.LengthOfSecondarySortValuePart)
			{
				throw new ArgumentException("Invalid serialized item as the length is not consistent");
			}
			object value = ReferenceItem.ConvertStringToSortColumnValue(sortBy.ColumnDefinition, serializedItem.Substring(ReferenceItem.LengthOfSortColumnValueLengthPart, num));
			long num2 = 0L;
			if (!long.TryParse(serializedItem.Substring(ReferenceItem.LengthOfSortColumnValueLengthPart + num), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2))
			{
				throw new ArgumentException("Invalid serialized item as the reference id is invalid");
			}
			if (num2 < 0L)
			{
				throw new ArgumentException("Invalid serialized item as the secondary sort value is negative");
			}
			return new ReferenceItem(sortBy, value, num2);
		}

		public override bool Equals(object obj)
		{
			ReferenceItem referenceItem = obj as ReferenceItem;
			return referenceItem != null && this.SortBy.ColumnDefinition.Equals(referenceItem.SortBy.ColumnDefinition) && this.SortBy.SortOrder == referenceItem.SortBy.SortOrder && this.SortColumnValue.Equals(referenceItem.SortColumnValue) && this.SecondarySortValue == referenceItem.SecondarySortValue;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = ReferenceItem.ConvertSortValueToString(this.sortBy.ColumnDefinition, this.sortColumnValue);
			stringBuilder.AppendFormat("{0:X16}", this.SecondarySortValue);
			return stringBuilder.ToString();
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			ReferenceItem referenceItem = obj as ReferenceItem;
			if (referenceItem == null)
			{
				throw new ArgumentException("Object is not a ReferenceItem");
			}
			if (!this.SortBy.ColumnDefinition.Equals(referenceItem.SortBy.ColumnDefinition))
			{
				throw new ArgumentException("Cannot compare two reference items with different sort columns");
			}
			if (this.SortBy.SortOrder != referenceItem.SortBy.SortOrder)
			{
				throw new ArgumentException("Cannot compare two reference items with different sort order");
			}
			if (this.SortColumnValue == null && referenceItem.SortColumnValue != null)
			{
				return -1;
			}
			if (this.SortColumnValue != null && referenceItem.SortColumnValue == null)
			{
				return 1;
			}
			if (this.SortColumnValue.Equals(referenceItem.SortColumnValue))
			{
				return this.SecondarySortValue.CompareTo(referenceItem.SecondarySortValue);
			}
			IComparable comparable = this.SortColumnValue as IComparable;
			IComparable obj2 = referenceItem.SortColumnValue as IComparable;
			return comparable.CompareTo(obj2);
		}

		private static StringBuilder ConvertSortValueToString(PropertyDefinition column, object value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Type left = Nullable.GetUnderlyingType(column.Type) ?? column.Type;
			if (left != typeof(int) && left != typeof(long) && left != typeof(ExDateTime))
			{
				throw new ArgumentException("Invalid sort column. We support only sort property with type int, long, ExDateTime");
			}
			if (left == typeof(int))
			{
				stringBuilder.AppendFormat("{0:X4}{1:X8}", 8, value);
			}
			if (left == typeof(long))
			{
				stringBuilder.AppendFormat("{0:X4}{1:X16}", 16, value);
			}
			if (left == typeof(ExDateTime))
			{
				string text = ((ExDateTime)value).UniversalTime.ToString(ReferenceItem.DateTimeFormatString, DateTimeFormatInfo.InvariantInfo);
				stringBuilder.AppendFormat("{0:X4}{1}", text.Length, text);
			}
			return stringBuilder;
		}

		private static object ConvertStringToSortColumnValue(PropertyDefinition column, string stringValue)
		{
			Type left = Nullable.GetUnderlyingType(column.Type) ?? column.Type;
			object result = null;
			if (left != typeof(int) && left != typeof(long) && left != typeof(ExDateTime))
			{
				throw new ArgumentException("Invalid sort column. We support only sort property with type int, long, ExDateTime");
			}
			if (left == typeof(int))
			{
				int num;
				if (!int.TryParse(stringValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
				{
					throw new ArgumentException("Invalid string value as int");
				}
				result = num;
			}
			if (left == typeof(long))
			{
				long num2;
				if (!long.TryParse(stringValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2))
				{
					throw new ArgumentException("Invalid string value as long");
				}
				result = num2;
			}
			if (left == typeof(ExDateTime))
			{
				ExDateTime exDateTime;
				if (!ExDateTime.TryParseExact(ExTimeZone.UtcTimeZone, stringValue, ReferenceItem.DateTimeFormatString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out exDateTime))
				{
					throw new ArgumentException("Invalid string value as ExDateTime");
				}
				result = exDateTime;
			}
			return result;
		}

		private static readonly int LengthOfSortColumnValueLengthPart = 4;

		private static readonly int LengthOfSecondarySortValuePart = 16;

		private static readonly string DateTimeFormatString = "yyyy-MM-ddTHH:mm:ss.fffffff";

		private static readonly string DateTimeWithoutMillisecondsFormatString = "yyyy-MM-ddTHH:mm:ss";

		private static readonly int MaximumStringPropertyLength = 256;

		private readonly SortBy sortBy;

		private readonly long secondarySortValue;

		private readonly object sortColumnValue;
	}
}
