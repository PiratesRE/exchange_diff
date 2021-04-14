using System;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct SortColumn
	{
		public SortColumn(Column column, bool ascending)
		{
			this.column = column;
			this.ascending = ascending;
		}

		public Column Column
		{
			get
			{
				return this.column;
			}
		}

		public bool Ascending
		{
			get
			{
				return this.ascending;
			}
		}

		public static int MaxSortColumnLength(Type type)
		{
			if (!(type == typeof(string)))
			{
				return 255;
			}
			return 127;
		}

		public static int MaxSortColumnLength(PropertyType propType)
		{
			if (propType != PropertyType.Unicode)
			{
				return 255;
			}
			return 127;
		}

		public const int MaxSortColumnLengthBytes = 255;

		private Column column;

		private bool ascending;
	}
}
