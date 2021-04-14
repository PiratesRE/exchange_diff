using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class SortOrderBuilder : IEnumerable<SortColumn>, IEnumerable
	{
		public SortOrderBuilder() : this(6)
		{
		}

		public SortOrderBuilder(int space)
		{
			this.columns = new List<Column>(space);
			this.ascending = new List<bool>(space);
		}

		public SortOrderBuilder(SortOrder sortOrder)
		{
			if (sortOrder.Count == 0)
			{
				this.columns = new List<Column>(6);
				this.ascending = new List<bool>(6);
				return;
			}
			this.columns = sortOrder.Columns;
			this.ascending = sortOrder.Ascending;
			this.listsAreReadOnly = true;
		}

		public int Count
		{
			get
			{
				return this.columns.Count;
			}
		}

		public SortColumn this[int index]
		{
			get
			{
				return new SortColumn(this.columns[index], this.ascending[index]);
			}
		}

		public static explicit operator SortOrder(SortOrderBuilder builder)
		{
			return builder.ToSortOrder();
		}

		public SortOrder ToSortOrder()
		{
			this.listsAreReadOnly = true;
			return this.PrivateToSortOrder();
		}

		private SortOrder PrivateToSortOrder()
		{
			return new SortOrder(this.columns, this.ascending);
		}

		public void CopyFrom(SortOrder sortOrder)
		{
			if (sortOrder.Count == 0)
			{
				this.Clear();
				return;
			}
			this.columns = sortOrder.Columns;
			this.ascending = sortOrder.Ascending;
			this.listsAreReadOnly = true;
		}

		public void Add(Column column, bool ascending)
		{
			if (this.listsAreReadOnly)
			{
				this.columns = new List<Column>(this.columns);
				this.ascending = new List<bool>(this.ascending);
				this.listsAreReadOnly = false;
			}
			this.columns.Add(column);
			this.ascending.Add(ascending);
		}

		public void Add(Column column)
		{
			this.Add(column, true);
		}

		public void Clear()
		{
			if (this.listsAreReadOnly)
			{
				this.columns = new List<Column>(6);
				this.ascending = new List<bool>(6);
				this.listsAreReadOnly = false;
				return;
			}
			this.columns.Clear();
			this.ascending.Clear();
		}

		public bool Contains(Column column)
		{
			return this.IndexOf(column) != -1;
		}

		public int IndexOf(Column column)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (column == this.columns[i])
				{
					return i;
				}
			}
			return -1;
		}

		public void Reverse()
		{
			if (this.Count != 0)
			{
				if (this.listsAreReadOnly)
				{
					this.columns = new List<Column>(this.columns);
					this.ascending = new List<bool>(this.ascending);
					this.listsAreReadOnly = false;
				}
				for (int i = 0; i < this.Count; i++)
				{
					this.ascending[i] = !this.ascending[i];
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(50 * this.columns.Count);
			this.PrivateToSortOrder().AppendToStringBuilder(stringBuilder, StringFormatOptions.IncludeDetails);
			return stringBuilder.ToString();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.PrivateToSortOrder().GetEnumerator();
		}

		IEnumerator<SortColumn> IEnumerable<SortColumn>.GetEnumerator()
		{
			return this.PrivateToSortOrder().GetEnumerator();
		}

		private SortOrder.Enumerator GetEnumerator()
		{
			return this.PrivateToSortOrder().GetEnumerator();
		}

		private const int AverageSortOrderSize = 6;

		private IList<Column> columns;

		private IList<bool> ascending;

		private bool listsAreReadOnly;
	}
}
