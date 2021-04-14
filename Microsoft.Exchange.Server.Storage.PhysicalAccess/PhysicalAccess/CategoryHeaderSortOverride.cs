using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class CategoryHeaderSortOverride
	{
		public Column Column
		{
			get
			{
				return this.sortColumnForAggregation.Column;
			}
		}

		public bool Ascending
		{
			get
			{
				return this.sortColumnForAggregation.Ascending;
			}
		}

		public bool AggregateByMaxValue
		{
			get
			{
				return this.aggregateByMaxValue;
			}
		}

		public CategoryHeaderSortOverride(Column column, bool ascending, bool aggregateByMaxValue)
		{
			this.sortColumnForAggregation = new SortColumn(column, ascending);
			this.aggregateByMaxValue = aggregateByMaxValue;
		}

		public static int NumberOfOverrides(CategoryHeaderSortOverride[] categoryHeaderSortOverrides)
		{
			int num = 0;
			foreach (CategoryHeaderSortOverride categoryHeaderSortOverride in categoryHeaderSortOverrides)
			{
				if (categoryHeaderSortOverride != null)
				{
					num++;
				}
			}
			return num;
		}

		public static bool ContainsColumn(IList<CategoryHeaderSortOverride> categoryHeaderSortOverrides, Column column)
		{
			foreach (CategoryHeaderSortOverride categoryHeaderSortOverride in categoryHeaderSortOverrides)
			{
				if (categoryHeaderSortOverride != null && categoryHeaderSortOverride.Column == column)
				{
					return true;
				}
			}
			return false;
		}

		public static CategoryHeaderSortOverride Deserialize(byte[] buffer, ref int offset, Func<int, string, Column> convertToColumn)
		{
			string arg = SerializedValue.ParseString(buffer, ref offset);
			int arg2 = SerializedValue.ParseInt32(buffer, ref offset);
			bool ascending = SerializedValue.ParseBoolean(buffer, ref offset);
			bool flag = SerializedValue.ParseBoolean(buffer, ref offset);
			return new CategoryHeaderSortOverride(convertToColumn(arg2, arg), ascending, flag);
		}

		public int Serialize(byte[] buffer, int startingOffset)
		{
			string value;
			uint value2;
			this.Column.GetNameOrIdForSerialization(out value, out value2);
			int num = startingOffset + SerializedValue.SerializeString(value, buffer, startingOffset);
			num += SerializedValue.SerializeInt32((int)value2, buffer, num);
			num += SerializedValue.SerializeBoolean(this.Ascending, buffer, num);
			num += SerializedValue.SerializeBoolean(this.AggregateByMaxValue, buffer, num);
			return num - startingOffset;
		}

		private readonly SortColumn sortColumnForAggregation;

		private readonly bool aggregateByMaxValue;
	}
}
