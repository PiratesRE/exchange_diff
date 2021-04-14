using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct SortOrder : IEnumerable<SortColumn>, IEnumerable, IEquatable<SortOrder>
	{
		public SortOrder(IList<Column> columns, IList<bool> ascending)
		{
			this.columns = columns;
			this.ascending = ascending;
		}

		public static SortOrder Empty
		{
			get
			{
				return SortOrder.empty;
			}
		}

		public IList<Column> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public IList<bool> Ascending
		{
			get
			{
				return this.ascending;
			}
		}

		public int Count
		{
			get
			{
				if (this.columns != null)
				{
					return this.columns.Count;
				}
				return 0;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		public SortColumn this[int index]
		{
			get
			{
				return new SortColumn(this.columns[index], this.ascending[index]);
			}
		}

		public static bool IsMatch(SortOrder desiredSortOrder, SortOrder actualSortOrder, ISet<Column> ignoreConstantColumns, out bool reverseSortOrder)
		{
			reverseSortOrder = false;
			int count = desiredSortOrder.Count;
			if (count > 0)
			{
				int count2 = actualSortOrder.Count;
				if (count2 == 0)
				{
					return false;
				}
				bool flag = true;
				int i = 0;
				int j = 0;
				while (j < count)
				{
					if (i < count2 && !(desiredSortOrder.Columns[j] != actualSortOrder.Columns[i]))
					{
						goto IL_BB;
					}
					if (ignoreConstantColumns == null || !ignoreConstantColumns.Contains(desiredSortOrder.Columns[j]))
					{
						while (i < count2)
						{
							if (ignoreConstantColumns != null && ignoreConstantColumns.Contains(actualSortOrder.Columns[i]))
							{
								i++;
							}
							else
							{
								if (desiredSortOrder.Columns[j] != actualSortOrder.Columns[i])
								{
									reverseSortOrder = false;
									return false;
								}
								goto IL_BB;
							}
						}
						reverseSortOrder = false;
						return false;
					}
					IL_12F:
					j++;
					continue;
					IL_BB:
					if (flag)
					{
						reverseSortOrder = (desiredSortOrder.Ascending[j] != actualSortOrder.Ascending[i]);
						flag = false;
					}
					else if ((!reverseSortOrder && desiredSortOrder.Ascending[j] != actualSortOrder.Ascending[i]) || (reverseSortOrder && desiredSortOrder.Ascending[j] == actualSortOrder.Ascending[i]))
					{
						reverseSortOrder = false;
						return false;
					}
					i++;
					goto IL_12F;
				}
			}
			return true;
		}

		public static bool IsMatchOrReverse(SortOrder desiredSortOrder, SortOrder actualSortOrder)
		{
			bool flag;
			return SortOrder.IsMatch(desiredSortOrder, actualSortOrder, null, out flag);
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

		public SortOrder Reverse()
		{
			if (this.Count == 0)
			{
				return this;
			}
			SortOrderBuilder sortOrderBuilder = new SortOrderBuilder();
			for (int i = 0; i < this.Count; i++)
			{
				sortOrderBuilder.Add(this.columns[i], !this.ascending[i]);
			}
			return sortOrderBuilder.ToSortOrder();
		}

		internal void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (i != 0)
				{
					sb.Append(", ");
				}
				this.columns[i].AppendToString(sb, formatOptions);
				if (!this.ascending[i])
				{
					sb.Append(" desc");
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(50 * this.Count);
			this.AppendToStringBuilder(stringBuilder, StringFormatOptions.IncludeDetails);
			return stringBuilder.ToString();
		}

		public bool Equals(SortOrder other)
		{
			if (this.Count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
			{
				if (this.columns[i] != other.Columns[i] || this.ascending[i] != other.Ascending[i])
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object other)
		{
			return other is SortOrder && this.Equals((SortOrder)other);
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.Count; i++)
			{
				num += this.columns[i].GetHashCode() + (this.ascending[i] ? 0 : 1);
			}
			return num;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator<SortColumn> IEnumerable<SortColumn>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public SortOrder.Enumerator GetEnumerator()
		{
			return new SortOrder.Enumerator(this);
		}

		private static readonly SortOrder empty = default(SortOrder);

		private IList<Column> columns;

		private IList<bool> ascending;

		public struct Enumerator : IEnumerator<SortColumn>, IDisposable, IEnumerator
		{
			internal Enumerator(SortOrder sortOrder)
			{
				this.sortOrder = sortOrder;
				this.index = -1;
			}

			public SortColumn Current
			{
				get
				{
					return this.sortOrder[this.index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				if (this.index < this.sortOrder.Count)
				{
					this.index++;
					if (this.index < this.sortOrder.Count)
					{
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				this.index = -1;
			}

			public void Dispose()
			{
				this.sortOrder = default(SortOrder);
			}

			private SortOrder sortOrder;

			private int index;
		}
	}
}
