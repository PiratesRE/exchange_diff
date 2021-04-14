using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class FullTextIndex : IIndex
	{
		public FullTextIndex(string name, Column[] columns)
		{
			this.name = name;
			this.columns = new List<Column>(columns);
			this.indexTableFunction = new FullTextIndexTableFunctionTableFunction().TableFunction;
			this.renameDictionary = new Dictionary<Column, Column>(this.columns.Count);
			foreach (Column column in this.columns)
			{
				bool flag = false;
				for (int i = 0; i < this.indexTableFunction.Columns.Count; i++)
				{
					if (this.indexTableFunction.Columns[i].Name == column.Name)
					{
						this.renameDictionary.Add(column, this.indexTableFunction.Columns[i]);
						flag = true;
					}
				}
				if (!flag)
				{
					this.renameDictionary.Add(column, column);
				}
			}
		}

		public bool ShouldBeCurrent
		{
			get
			{
				return false;
			}
		}

		public IReadOnlyDictionary<Column, Column> RenameDictionary
		{
			get
			{
				return this.renameDictionary;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IList<Column> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public ISet<Column> ConstantColumns
		{
			get
			{
				return null;
			}
		}

		public SortOrder LogicalSortOrder
		{
			get
			{
				return SortOrder.Empty;
			}
		}

		public Table Table
		{
			get
			{
				return this.Columns[0].Table;
			}
		}

		public Table IndexTable
		{
			get
			{
				return this.indexTableFunction;
			}
		}

		public IList<object> IndexKeyPrefix
		{
			get
			{
				return null;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return SortOrder.Empty;
			}
		}

		public bool GetIndexColumn(Column column, bool acceptTruncated, out Column indexColumn)
		{
			indexColumn = column;
			if ((column.Table == this.indexTableFunction || this.RenameDictionary.TryGetValue(column, out indexColumn)) && (acceptTruncated || column.MaxLength <= indexColumn.MaxLength))
			{
				return true;
			}
			indexColumn = null;
			return false;
		}

		private readonly string name;

		private TableFunction indexTableFunction;

		private Dictionary<Column, Column> renameDictionary;

		private IList<Column> columns;
	}
}
