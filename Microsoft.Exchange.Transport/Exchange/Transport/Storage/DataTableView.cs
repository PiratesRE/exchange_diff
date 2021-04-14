using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class DataTableView
	{
		public DataTableView(DataTable table)
		{
			this.table = table;
			this.Initialize();
		}

		public DataTableView(DataTable table, int[] viewColumns)
		{
			this.table = table;
			this.Initialize(viewColumns);
		}

		public DataTable Table
		{
			get
			{
				return this.table;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.viewToRowIndex.Length;
			}
		}

		public int GetColumnViewIndex(int rowIndex)
		{
			int num = this.rowToViewIndex[rowIndex];
			if (num == -1)
			{
				throw new ArgumentException(string.Format("Column {0} not in view", rowIndex), "rowIndex");
			}
			return num;
		}

		public int GetColumnRowIndex(int viewIndex)
		{
			return this.viewToRowIndex[viewIndex];
		}

		private void Initialize()
		{
			int cacheCount = this.table.CacheCount;
			int[] array = new int[cacheCount];
			int num = 0;
			for (int i = 0; i < this.table.Schemas.Count; i++)
			{
				if (this.table.Schemas[i].Cached)
				{
					array[num] = i;
					num++;
				}
			}
			this.Initialize(array);
		}

		private void Initialize(int[] viewColumns)
		{
			this.rowToViewIndex = new int[this.table.Schemas.Count];
			this.viewToRowIndex = new int[viewColumns.Length];
			int num = 0;
			for (int i = 0; i < this.table.Schemas.Count; i++)
			{
				this.rowToViewIndex[i] = -1;
				if (this.table.Schemas[i].Cached)
				{
					for (int j = 0; j < viewColumns.Length; j++)
					{
						if (viewColumns[j] == i)
						{
							this.rowToViewIndex[i] = num;
							this.viewToRowIndex[num] = i;
							num++;
							break;
						}
					}
				}
			}
			if (num != viewColumns.Length)
			{
				throw new IndexOutOfRangeException("The view columns array contained indexes that do not belong to any row column or duplicates");
			}
		}

		private DataTable table;

		private int[] rowToViewIndex;

		private int[] viewToRowIndex;
	}
}
