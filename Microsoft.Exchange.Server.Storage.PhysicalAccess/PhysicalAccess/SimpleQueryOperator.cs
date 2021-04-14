using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SimpleQueryOperator : DataAccessOperator, IColumnResolver
	{
		protected SimpleQueryOperator(IConnectionProvider connectionProvider, SimpleQueryOperator.SimpleQueryOperatorDefinition operatorDefinition) : base(connectionProvider, operatorDefinition)
		{
		}

		public Table Table
		{
			get
			{
				return this.OperatorDefinition.Table;
			}
		}

		public SearchCriteria Criteria
		{
			get
			{
				return this.OperatorDefinition.Criteria;
			}
		}

		public IList<Column> ColumnsToFetch
		{
			get
			{
				return this.OperatorDefinition.ColumnsToFetch;
			}
		}

		public IReadOnlyDictionary<Column, Column> RenameDictionary
		{
			get
			{
				return this.OperatorDefinition.RenameDictionary;
			}
		}

		public int SkipTo
		{
			get
			{
				return this.OperatorDefinition.SkipTo;
			}
		}

		public int MaxRows
		{
			get
			{
				return this.OperatorDefinition.MaxRows;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.OperatorDefinition.SortOrder;
			}
		}

		public bool Backwards
		{
			get
			{
				return this.OperatorDefinition.Backwards;
			}
		}

		public SimpleQueryOperator.SimpleQueryOperatorDefinition OperatorDefinition
		{
			get
			{
				return (SimpleQueryOperator.SimpleQueryOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public abstract Reader ExecuteReader(bool disposeQueryOperator);

		public override object ExecuteScalar()
		{
			throw new NotSupportedException("ExecuteScalar is not supported by this operator");
		}

		public virtual bool OperatorUsesTablePartition(Table table, IList<object> partitionKeyPrefix)
		{
			return this.Table == table;
		}

		Column IColumnResolver.Resolve(Column column)
		{
			if (this.RenameDictionary != null)
			{
				column = this.ResolveColumn(column);
			}
			return column;
		}

		protected Column ResolveColumn(Column column)
		{
			int num = 0;
			Column column2;
			while (this.RenameDictionary.TryGetValue(column, out column2))
			{
				Globals.AssertRetail(++num < 10, "Rename chain is too long, possible circular renames?");
				column = column2;
			}
			return column;
		}

		protected void TraceAppendColumns(StringBuilder sb, ITWIR dataAccessor, IList<Column> columns)
		{
			for (int i = 0; i < columns.Count; i++)
			{
				if (i != 0)
				{
					sb.Append(", ");
				}
				Column column = columns[i];
				column.AppendToString(sb, StringFormatOptions.None);
				sb.Append("=[");
				try
				{
					sb.AppendAsString(dataAccessor.GetColumnValue(column));
				}
				catch (NonFatalDatabaseException ex)
				{
					base.Connection.OnExceptionCatch(ex);
					sb.Append("EXCEPTION:[");
					sb.Append(ex);
					sb.Append("]");
				}
				sb.Append("]");
			}
		}

		[Conditional("DEBUG")]
		private void AddDependentColumns(Column column, HashSet<Column> columns)
		{
			for (;;)
			{
				columns.Add(column);
				Column column2;
				if (this.RenameDictionary != null && this.RenameDictionary.TryGetValue(column, out column2))
				{
					column = column2;
				}
				else
				{
					ConversionColumn conversionColumn = column.ActualColumn as ConversionColumn;
					if (!(conversionColumn != null))
					{
						break;
					}
					column = conversionColumn.ArgumentColumn;
				}
			}
			FunctionColumn functionColumn = column.ActualColumn as FunctionColumn;
			if (functionColumn != null)
			{
				foreach (Column column3 in functionColumn.ArgumentColumns)
				{
				}
			}
		}

		[Conditional("DEBUG")]
		internal void AssertIfNotColumnToFetch(Column column)
		{
		}

		public abstract class SimpleQueryOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			protected SimpleQueryOperatorDefinition(CultureInfo culture, Table table, IList<Column> columnsToFetch, SearchCriteria criteria, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.table = table;
				this.columnsToFetch = columnsToFetch;
				this.criteria = criteria;
				this.skipTo = skipTo;
				this.maxRows = maxRows;
				this.renameDictionary = renameDictionary;
			}

			public Table Table
			{
				get
				{
					return this.table;
				}
			}

			public SearchCriteria Criteria
			{
				get
				{
					return this.criteria;
				}
			}

			public IList<Column> ColumnsToFetch
			{
				get
				{
					return this.columnsToFetch;
				}
			}

			public IReadOnlyDictionary<Column, Column> RenameDictionary
			{
				get
				{
					return this.renameDictionary;
				}
			}

			public int SkipTo
			{
				get
				{
					return this.skipTo;
				}
			}

			public int MaxRows
			{
				get
				{
					return this.maxRows;
				}
			}

			public abstract SortOrder SortOrder { get; }

			public abstract bool Backwards { get; }

			public abstract SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider);

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				detail = (((this.columnsToFetch != null) ? this.columnsToFetch.Count : 0) ^ this.maxRows ^ this.skipTo);
				simple = 0;
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				SimpleQueryOperator.SimpleQueryOperatorDefinition simpleQueryOperatorDefinition = other as SimpleQueryOperator.SimpleQueryOperatorDefinition;
				if (simpleQueryOperatorDefinition == null || (this.columnsToFetch != null && simpleQueryOperatorDefinition.columnsToFetch == null) || (this.columnsToFetch == null && simpleQueryOperatorDefinition.columnsToFetch != null) || (this.columnsToFetch != null && simpleQueryOperatorDefinition.columnsToFetch != null && this.columnsToFetch.Count != simpleQueryOperatorDefinition.columnsToFetch.Count) || this.maxRows != simpleQueryOperatorDefinition.maxRows || this.skipTo != simpleQueryOperatorDefinition.skipTo)
				{
					return false;
				}
				if (this.columnsToFetch != null)
				{
					for (int i = 0; i < this.columnsToFetch.Count; i++)
					{
						if (this.columnsToFetch[i].Name != simpleQueryOperatorDefinition.columnsToFetch[i].Name)
						{
							return false;
						}
					}
				}
				return true;
			}

			private readonly int maxRows;

			private readonly int skipTo;

			private Table table;

			private SearchCriteria criteria;

			private IList<Column> columnsToFetch;

			private IReadOnlyDictionary<Column, Column> renameDictionary;
		}
	}
}
