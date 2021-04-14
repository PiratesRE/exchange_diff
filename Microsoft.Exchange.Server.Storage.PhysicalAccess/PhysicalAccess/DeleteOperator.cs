using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class DeleteOperator : DataAccessOperator
	{
		protected DeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation) : base(connectionProvider, new DeleteOperator.DeleteOperatorDefinition(culture, tableOperator.OperatorDefinition, frequentOperation))
		{
			this.tableOperator = tableOperator;
		}

		public TableOperator TableOperator
		{
			get
			{
				return this.tableOperator;
			}
		}

		public DeleteOperator.DeleteOperatorDefinition OperatorDefinition
		{
			get
			{
				return (DeleteOperator.DeleteOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			operatorAction(this.tableOperator);
		}

		public override IExecutionPlanner GetExecutionPlanner()
		{
			return base.GetExecutionPlanner() ?? DataAccessOperator.GetExecutionPlannerOrNull(this.tableOperator);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.tableOperator != null)
			{
				this.tableOperator.Dispose();
			}
		}

		private readonly TableOperator tableOperator;

		public class DeleteOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			public DeleteOperatorDefinition(CultureInfo culture, TableOperator.TableOperatorDefinition tableOperatorDefinition, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.tableOperatorDefinition = tableOperatorDefinition;
			}

			internal override string OperatorName
			{
				get
				{
					return "DELETE";
				}
			}

			private TableOperator.TableOperatorDefinition TableOperatorDefinition
			{
				get
				{
					return this.tableOperatorDefinition;
				}
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
				operatorDefinitionAction(this.TableOperatorDefinition);
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("delete");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (this.TableOperatorDefinition != null && this.TableOperatorDefinition.Table != null)
				{
					sb.Append(" from ");
					sb.Append(this.TableOperatorDefinition.Table.Name);
				}
				if (this.TableOperatorDefinition != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  operator:[");
					base.Indent(sb, multiLine, nestingLevel + 1, false);
					if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
					{
						sb.Append("op:[");
						sb.Append(this.TableOperatorDefinition.OperatorName);
						sb.Append(" ");
						sb.Append(this.TableOperatorDefinition.GetHashCode());
						sb.Append("]");
					}
					this.TableOperatorDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
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
				this.tableOperatorDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				detail = (55224 ^ num2);
				simple = (55224 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				DeleteOperator.DeleteOperatorDefinition deleteOperatorDefinition = other as DeleteOperator.DeleteOperatorDefinition;
				return deleteOperatorDefinition != null && this.tableOperatorDefinition.IsEqualsForStatisticPurposes(deleteOperatorDefinition.tableOperatorDefinition);
			}

			private readonly TableOperator.TableOperatorDefinition tableOperatorDefinition;
		}
	}
}
