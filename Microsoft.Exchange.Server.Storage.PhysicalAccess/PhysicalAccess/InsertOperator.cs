using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class InsertOperator : DataAccessOperator
	{
		protected InsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Column columnToFetch, bool frequentOperation) : base(connectionProvider, new InsertOperator.InsertOperatorDefinition(culture, table, (simpleQueryOperator != null) ? simpleQueryOperator.OperatorDefinition : null, columnsToInsert, valuesToInsert, columnToFetch, frequentOperation))
		{
			this.simpleQueryOperator = simpleQueryOperator;
		}

		public Table Table
		{
			get
			{
				return this.OperatorDefinition.Table;
			}
		}

		public IList<Column> ColumnsToInsert
		{
			get
			{
				return this.OperatorDefinition.ColumnsToInsert;
			}
		}

		protected IList<object> ValuesToInsert
		{
			get
			{
				return this.OperatorDefinition.ValuesToInsert;
			}
		}

		public SimpleQueryOperator SimpleQueryOperator
		{
			get
			{
				return this.simpleQueryOperator;
			}
		}

		protected Column ColumnToFetch
		{
			get
			{
				return this.OperatorDefinition.ColumnToFetch;
			}
		}

		public InsertOperator.InsertOperatorDefinition OperatorDefinition
		{
			get
			{
				return (InsertOperator.InsertOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			if (this.simpleQueryOperator != null)
			{
				this.simpleQueryOperator.EnumerateDescendants(operatorAction);
			}
		}

		public override IExecutionPlanner GetExecutionPlanner()
		{
			return base.GetExecutionPlanner() ?? DataAccessOperator.GetExecutionPlannerOrNull(this.simpleQueryOperator);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.simpleQueryOperator != null)
			{
				this.simpleQueryOperator.Dispose();
			}
		}

		private readonly SimpleQueryOperator simpleQueryOperator;

		public class InsertOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			public InsertOperatorDefinition(CultureInfo culture, Table table, SimpleQueryOperator.SimpleQueryOperatorDefinition simpleQueryOperatorDefinition, IList<Column> columnsToInsert, IList<object> valuesToInsert, Column columnToFetch, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.table = table;
				this.columnsToInsert = columnsToInsert;
				this.valuesToInsert = valuesToInsert;
				this.columnToFetch = columnToFetch;
				this.simpleQueryOperatorDefinition = simpleQueryOperatorDefinition;
				if (this.simpleQueryOperatorDefinition == null)
				{
					DataAccessOperator.DataAccessOperatorDefinition.CheckValueSizes(this.columnsToInsert, this.valuesToInsert);
				}
			}

			internal override string OperatorName
			{
				get
				{
					return "INSERT";
				}
			}

			public Table Table
			{
				get
				{
					return this.table;
				}
			}

			public IList<Column> ColumnsToInsert
			{
				get
				{
					return this.columnsToInsert;
				}
			}

			public IList<object> ValuesToInsert
			{
				get
				{
					return this.valuesToInsert;
				}
			}

			public SimpleQueryOperator.SimpleQueryOperatorDefinition SimpleQueryOperatorDefinition
			{
				get
				{
					return this.simpleQueryOperatorDefinition;
				}
			}

			public Column ColumnToFetch
			{
				get
				{
					return this.columnToFetch;
				}
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
				if (this.SimpleQueryOperatorDefinition != null)
				{
					this.SimpleQueryOperatorDefinition.EnumerateDescendants(operatorDefinitionAction);
				}
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("insert");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (this.Table != null)
				{
					sb.Append(" into ");
					sb.Append(this.Table.Name);
				}
				if (this.ColumnsToInsert != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  columns:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, this.ColumnsToInsert, this.ValuesToInsert, formatOptions);
					sb.Append("]");
				}
				if (this.SimpleQueryOperatorDefinition != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  from:[");
					base.Indent(sb, multiLine, nestingLevel + 1, false);
					if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
					{
						sb.Append("op:[");
						sb.Append(this.SimpleQueryOperatorDefinition.OperatorName);
						sb.Append(" ");
						sb.Append(this.SimpleQueryOperatorDefinition.GetHashCode());
						sb.Append("] ");
					}
					this.SimpleQueryOperatorDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  ]");
				}
				if (this.ColumnToFetch != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  fetch:[");
					this.ColumnToFetch.AppendToString(sb, formatOptions);
					sb.Append("]");
				}
				if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  frequentOp:[");
					sb.Append(base.FrequentOperation);
					sb.Append("]");
				}
			}

			[Conditional("DEBUG")]
			private void SanityCheckColumnsDebugOnly()
			{
				this.ColumnToFetch != null;
				if (this.SimpleQueryOperatorDefinition != null)
				{
					for (int i = 0; i < this.ColumnsToInsert.Count; i++)
					{
					}
				}
				IList<object> list = this.valuesToInsert;
			}

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				int num = 0;
				int num2 = 0;
				if (this.simpleQueryOperatorDefinition != null)
				{
					this.simpleQueryOperatorDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				}
				detail = (36712 ^ this.Table.GetHashCode() ^ num2);
				simple = (36712 ^ this.Table.TableClass.GetHashCode() ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				InsertOperator.InsertOperatorDefinition insertOperatorDefinition = other as InsertOperator.InsertOperatorDefinition;
				return insertOperatorDefinition != null && this.Table.Equals(insertOperatorDefinition.Table) && ((this.simpleQueryOperatorDefinition == null && insertOperatorDefinition.simpleQueryOperatorDefinition == null) || (this.simpleQueryOperatorDefinition != null && insertOperatorDefinition.simpleQueryOperatorDefinition != null && this.simpleQueryOperatorDefinition.IsEqualsForStatisticPurposes(insertOperatorDefinition.simpleQueryOperatorDefinition)));
			}

			private readonly SimpleQueryOperator.SimpleQueryOperatorDefinition simpleQueryOperatorDefinition;

			private readonly Table table;

			private readonly IList<Column> columnsToInsert;

			private readonly IList<object> valuesToInsert;

			private readonly Column columnToFetch;
		}
	}
}
