using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class DistinctOperator : SimpleQueryOperator
	{
		protected DistinctOperator(IConnectionProvider connectionProvider, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation) : this(connectionProvider, new DistinctOperator.DistinctOperatorDefinition(skipTo, maxRows, outerQuery.OperatorDefinition, frequentOperation))
		{
		}

		protected DistinctOperator(IConnectionProvider connectionProvider, DistinctOperator.DistinctOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.outerQuery = definition.OuterQueryDefinition.CreateOperator(connectionProvider);
		}

		public SimpleQueryOperator OuterQuery
		{
			get
			{
				return this.outerQuery;
			}
		}

		public new DistinctOperator.DistinctOperatorDefinition OperatorDefinition
		{
			get
			{
				return (DistinctOperator.DistinctOperatorDefinition)base.OperatorDefinitionBase;
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

		private SimpleQueryOperator outerQuery;

		public class DistinctOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public DistinctOperatorDefinition(int skipTo, int maxRows, SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition, bool frequentOperation) : base(outerQueryDefinition.Culture, outerQueryDefinition.Table, outerQueryDefinition.ColumnsToFetch, outerQueryDefinition.Criteria, outerQueryDefinition.RenameDictionary, skipTo, maxRows, frequentOperation)
			{
				this.outerQueryDefinition = outerQueryDefinition;
			}

			internal override string OperatorName
			{
				get
				{
					return "UNIQUE";
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
					return SortOrder.Empty;
				}
			}

			public override bool Backwards
			{
				get
				{
					return false;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateDistinctOperator(connectionProvider, this);
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
				sb.Append("select distinct");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
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
					sb.Append("  from:[");
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

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				int num;
				int num2;
				this.outerQueryDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				detail = (47921 ^ num2);
				simple = (47921 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				DistinctOperator.DistinctOperatorDefinition distinctOperatorDefinition = other as DistinctOperator.DistinctOperatorDefinition;
				return distinctOperatorDefinition != null && this.outerQueryDefinition.IsEqualsForStatisticPurposes(distinctOperatorDefinition.outerQueryDefinition);
			}

			private SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition;
		}
	}
}
