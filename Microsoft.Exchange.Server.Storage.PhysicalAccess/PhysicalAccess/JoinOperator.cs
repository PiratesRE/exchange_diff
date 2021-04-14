using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class JoinOperator : SimpleQueryOperator
	{
		protected JoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new JoinOperator.JoinOperatorDefinition(culture, table, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		protected JoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.outerQuery = definition.OuterQueryDefinition.CreateOperator(connectionProvider);
		}

		public int PreReadCacheSize
		{
			get
			{
				return this.preReadCacheSize;
			}
			set
			{
				this.preReadCacheSize = value;
			}
		}

		public bool PreReadAhead
		{
			get
			{
				return this.preReadAhead;
			}
			set
			{
				this.preReadAhead = value;
			}
		}

		protected IList<Column> KeyColumns
		{
			get
			{
				return this.OperatorDefinition.KeyColumns;
			}
		}

		public SimpleQueryOperator OuterQuery
		{
			get
			{
				return this.outerQuery;
			}
		}

		public IList<Column> LongValueColumnsToPreread
		{
			get
			{
				return this.OperatorDefinition.LongValueColumnsToPreread;
			}
		}

		public new JoinOperator.JoinOperatorDefinition OperatorDefinition
		{
			get
			{
				return (JoinOperator.JoinOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			if (this.outerQuery != null)
			{
				this.outerQuery.EnumerateDescendants(operatorAction);
			}
		}

		public override void RemoveChildren()
		{
			this.outerQuery = null;
		}

		public override IExecutionPlanner GetExecutionPlanner()
		{
			return base.GetExecutionPlanner() ?? DataAccessOperator.GetExecutionPlannerOrNull(this.outerQuery);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.outerQuery != null)
			{
				this.outerQuery.Dispose();
			}
		}

		protected const int DefaultPreReadCacheSize = 150;

		private int preReadCacheSize = 150;

		private bool preReadAhead = true;

		private SimpleQueryOperator outerQuery;

		public class JoinOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public JoinOperatorDefinition(CultureInfo culture, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition, bool frequentOperation) : base(culture, table, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, frequentOperation)
			{
				this.keyColumns = keyColumns;
				this.longValueColumnsToPreread = longValueColumnsToPreread;
				this.outerQueryDefinition = outerQueryDefinition;
			}

			internal override string OperatorName
			{
				get
				{
					return "JOIN";
				}
			}

			public IList<Column> KeyColumns
			{
				get
				{
					return this.keyColumns;
				}
			}

			public IList<Column> LongValueColumnsToPreread
			{
				get
				{
					return this.longValueColumnsToPreread;
				}
			}

			public SimpleQueryOperator.SimpleQueryOperatorDefinition OuterQueryDefinition
			{
				get
				{
					return this.outerQueryDefinition;
				}
			}

			public override SortOrder SortOrder
			{
				get
				{
					return this.OuterQueryDefinition.SortOrder;
				}
			}

			public override bool Backwards
			{
				get
				{
					return this.OuterQueryDefinition.Backwards;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateJoinOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
				if (this.OuterQueryDefinition != null)
				{
					this.OuterQueryDefinition.EnumerateDescendants(operatorDefinitionAction);
				}
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select join");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (this.OuterQueryDefinition != null)
				{
					if (this.OuterQueryDefinition.Table != null)
					{
						sb.Append(" ");
						sb.Append(this.OuterQueryDefinition.Table.Name);
						if (this.OuterQueryDefinition is TableFunctionOperator.TableFunctionOperatorDefinition)
						{
							sb.Append("()");
						}
					}
					else
					{
						sb.Append(" <outer:>");
					}
				}
				else
				{
					sb.Append(" <null>");
				}
				if (base.Table != null)
				{
					sb.Append(" and ");
					sb.Append(base.Table.Name);
				}
				if (base.ColumnsToFetch != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  fetch:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, base.ColumnsToFetch, null, formatOptions);
					sb.Append("]");
				}
				if (this.KeyColumns != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  keyColumns:[");
					for (int i = 0; i < this.KeyColumns.Count; i++)
					{
						if (i != 0)
						{
							sb.Append(", ");
						}
						sb.Append(this.KeyColumns[i].Name);
					}
					sb.Append("]");
				}
				if (base.Criteria != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  where:[");
					base.Criteria.AppendToString(sb, formatOptions);
					sb.Append("]");
				}
				if (base.SkipTo != 0)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  skipTo:[");
					sb.Append(base.SkipTo);
					sb.Append("]");
				}
				if (base.MaxRows != 0)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  maxRows:[");
					if ((formatOptions & StringFormatOptions.SkipParametersData) == StringFormatOptions.None || base.MaxRows == 1)
					{
						sb.Append(base.MaxRows);
					}
					else
					{
						sb.Append("X");
					}
					sb.Append("]");
				}
				if (this.OuterQueryDefinition != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  outer:[");
					base.Indent(sb, multiLine, nestingLevel + 1, false);
					if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
					{
						sb.Append("op:[");
						sb.Append(this.OuterQueryDefinition.OperatorName);
						sb.Append(" ");
						sb.Append(this.OuterQueryDefinition.GetHashCode());
						sb.Append("] ");
					}
					this.OuterQueryDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  ]");
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
			private void VerifyJoinCriteria()
			{
				for (int i = 0; i < this.KeyColumns.Count; i++)
				{
				}
			}

			[Conditional("DEBUG")]
			private void VerifyIndex()
			{
				for (int i = 0; i < base.Table.Indexes.Count; i++)
				{
					if (base.Table.Indexes[i].Columns.Count >= this.KeyColumns.Count)
					{
						bool flag = true;
						for (int j = 0; j < this.KeyColumns.Count; j++)
						{
							if (base.Table.Indexes[i].Columns[j] != this.KeyColumns[j])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return;
						}
					}
				}
			}

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				int num;
				int num2;
				this.outerQueryDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				detail = (53096 ^ num2);
				simple = (53096 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				JoinOperator.JoinOperatorDefinition joinOperatorDefinition = other as JoinOperator.JoinOperatorDefinition;
				return joinOperatorDefinition != null && this.outerQueryDefinition.IsEqualsForStatisticPurposes(joinOperatorDefinition.outerQueryDefinition);
			}

			private readonly IList<Column> keyColumns;

			private readonly IList<Column> longValueColumnsToPreread;

			private SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition;
		}
	}
}
