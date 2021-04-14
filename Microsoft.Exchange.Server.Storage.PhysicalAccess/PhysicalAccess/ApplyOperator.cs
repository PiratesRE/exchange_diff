using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class ApplyOperator : SimpleQueryOperator
	{
		protected ApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new ApplyOperator.ApplyOperatorDefinition(culture, tableFunction, tableFunctionParameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		protected ApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.outerQuery = definition.OuterQueryDefinition.CreateOperator(connectionProvider);
		}

		protected SimpleQueryOperator OuterQuery
		{
			get
			{
				return this.outerQuery;
			}
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.OperatorDefinition.TableFunction;
			}
		}

		public IList<Column> TableFunctionParameters
		{
			get
			{
				return this.OperatorDefinition.TableFunctionParameters;
			}
		}

		public new ApplyOperator.ApplyOperatorDefinition OperatorDefinition
		{
			get
			{
				return (ApplyOperator.ApplyOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			if (this.OuterQuery != null)
			{
				this.OuterQuery.EnumerateDescendants(operatorAction);
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

		private SimpleQueryOperator outerQuery;

		public class ApplyOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public ApplyOperatorDefinition(CultureInfo culture, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition, bool frequentOperation) : base(culture, tableFunction, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, frequentOperation)
			{
				this.tableFunctionParameters = tableFunctionParameters;
				this.outerQueryDefinition = outerQueryDefinition;
			}

			internal override string OperatorName
			{
				get
				{
					return "APPLY";
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

			public TableFunction TableFunction
			{
				get
				{
					return (TableFunction)base.Table;
				}
			}

			public IList<Column> TableFunctionParameters
			{
				get
				{
					return this.tableFunctionParameters;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateApplyOperator(connectionProvider, this);
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
				sb.Append("select apply");
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
				sb.Append(" and ");
				sb.Append(this.TableFunction.Name);
				sb.Append("(");
				if (this.TableFunctionParameters != null)
				{
					for (int i = 0; i < this.TableFunctionParameters.Count; i++)
					{
						if (i > 0)
						{
							sb.Append(", ");
						}
						sb.Append(this.TableFunctionParameters[i].Name);
					}
				}
				sb.Append(")");
				if (base.ColumnsToFetch != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  fetch:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, base.ColumnsToFetch, null, formatOptions);
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
					sb.Append("    ]");
				}
				if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  frequentOp:[");
					sb.Append(base.FrequentOperation);
					sb.Append("]");
				}
			}

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				int num;
				int num2;
				this.outerQueryDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				detail = (47032 ^ base.Table.GetHashCode() ^ num2);
				simple = (47032 ^ base.Table.TableClass.GetHashCode() ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				ApplyOperator.ApplyOperatorDefinition applyOperatorDefinition = other as ApplyOperator.ApplyOperatorDefinition;
				return applyOperatorDefinition != null && base.Table.Equals(applyOperatorDefinition.Table) && this.outerQueryDefinition.IsEqualsForStatisticPurposes(applyOperatorDefinition.outerQueryDefinition);
			}

			private readonly IList<Column> tableFunctionParameters;

			private SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition;
		}
	}
}
