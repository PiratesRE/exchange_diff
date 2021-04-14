using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class SimplePseudoIndex : IPseudoIndex, IIndex
	{
		public SimplePseudoIndex(Table table, Table indexTable, object[] tableFunctionParameters, SortOrder logicalSortOrder, IReadOnlyDictionary<Column, Column> renameDictionary, IList<object> indexKeyPrefix, bool shouldBeCurrent)
		{
			this.table = table;
			this.indexTable = indexTable;
			this.tableFunctionParameters = tableFunctionParameters;
			this.logicalSortOrder = logicalSortOrder;
			this.renameDictionary = renameDictionary;
			this.indexKeyPrefix = indexKeyPrefix;
			this.shouldBeCurrent = shouldBeCurrent;
			this.columns = new List<Column>(this.renameDictionary.Keys);
		}

		public bool ShouldBeCurrent
		{
			get
			{
				return this.shouldBeCurrent;
			}
		}

		public IReadOnlyDictionary<Column, Column> RenameDictionary
		{
			get
			{
				return this.renameDictionary;
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
				return this.logicalSortOrder;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public Table IndexTable
		{
			get
			{
				return this.indexTable;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.indexTable.PrimaryKeyIndex.SortOrder;
			}
		}

		public object[] IndexTableFunctionParameters
		{
			get
			{
				return this.tableFunctionParameters;
			}
		}

		public int RedundantKeyColumnsCount
		{
			get
			{
				return 0;
			}
		}

		public CategorizedTableParams GetCategorizedTableParams(Context context)
		{
			return null;
		}

		public IList<object> IndexKeyPrefix
		{
			get
			{
				return this.indexKeyPrefix;
			}
		}

		public bool GetIndexColumn(Column column, bool acceptTruncated, out Column indexColumn)
		{
			indexColumn = column;
			if ((column.Table == this.indexTable || this.RenameDictionary.TryGetValue(column, out indexColumn)) && (acceptTruncated || column.MaxLength <= indexColumn.MaxLength))
			{
				return true;
			}
			indexColumn = null;
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("SimplePseudoIndex:[Table:[");
			stringBuilder.Append(this.table.Name);
			stringBuilder.Append("] IndexTable:[");
			stringBuilder.Append(this.indexTable);
			stringBuilder.Append("] tableFunctionParameters:[");
			stringBuilder.Append(this.tableFunctionParameters);
			stringBuilder.Append("] LogicalSortOrder:[");
			stringBuilder.Append(this.logicalSortOrder);
			stringBuilder.Append("] RenameDictionary:[");
			stringBuilder.AppendAsString(this.renameDictionary);
			stringBuilder.Append("] IndexKeyPrefix:[");
			stringBuilder.AppendAsString(this.indexKeyPrefix);
			stringBuilder.Append("] ShouldBeCurrent:[");
			stringBuilder.Append(this.shouldBeCurrent);
			stringBuilder.Append("] Columns:[");
			stringBuilder.AppendAsString(this.columns);
			stringBuilder.Append("]]");
			return stringBuilder.ToString();
		}

		private Table table;

		private Table indexTable;

		private object[] tableFunctionParameters;

		private SortOrder logicalSortOrder;

		private IReadOnlyDictionary<Column, Column> renameDictionary;

		private IList<object> indexKeyPrefix;

		private bool shouldBeCurrent;

		private IList<Column> columns;
	}
}
