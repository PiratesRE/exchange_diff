using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class UpdateOperator : DataAccessOperator
	{
		protected UpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation) : base(connectionProvider, new UpdateOperator.UpdateOperatorDefinition(culture, tableOperator.OperatorDefinition, columnsToUpdate, valuesToUpdate, frequentOperation))
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

		protected IList<object> ValuesToUpdate
		{
			get
			{
				return this.OperatorDefinition.ValuesToUpdate;
			}
		}

		protected IList<Column> ColumnsToUpdate
		{
			get
			{
				return this.OperatorDefinition.ColumnsToUpdate;
			}
		}

		public UpdateOperator.UpdateOperatorDefinition OperatorDefinition
		{
			get
			{
				return (UpdateOperator.UpdateOperatorDefinition)base.OperatorDefinitionBase;
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

		public class UpdateOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			public UpdateOperatorDefinition(CultureInfo culture, TableOperator.TableOperatorDefinition tableOperatorDefinition, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.tableOperatorDefinition = tableOperatorDefinition;
				this.columnsToUpdate = columnsToUpdate;
				this.valuesToUpdate = valuesToUpdate;
				DataAccessOperator.DataAccessOperatorDefinition.CheckValueSizes(this.columnsToUpdate, this.valuesToUpdate);
			}

			internal override string OperatorName
			{
				get
				{
					return "UPDATE";
				}
			}

			internal IList<object> ValuesToUpdate
			{
				get
				{
					return this.valuesToUpdate;
				}
			}

			internal IList<Column> ColumnsToUpdate
			{
				get
				{
					return this.columnsToUpdate;
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
				sb.Append("update");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (this.TableOperatorDefinition != null && this.TableOperatorDefinition.Table != null)
				{
					sb.Append(" ");
					sb.Append(this.TableOperatorDefinition.Table.Name);
				}
				if (this.ColumnsToUpdate != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  columns:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, this.ColumnsToUpdate, this.ValuesToUpdate, formatOptions);
					sb.Append("]");
				}
				if (this.TableOperatorDefinition != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  from:[");
					base.Indent(sb, multiLine, nestingLevel + 1, false);
					if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
					{
						sb.Append("op:[");
						sb.Append(this.TableOperatorDefinition.OperatorName);
						sb.Append(" ");
						sb.Append(this.TableOperatorDefinition.GetHashCode());
						sb.Append("] ");
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
				detail = (59240 ^ ((this.columnsToUpdate != null) ? this.columnsToUpdate.Count : 0) ^ num2);
				simple = (59240 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				UpdateOperator.UpdateOperatorDefinition updateOperatorDefinition = other as UpdateOperator.UpdateOperatorDefinition;
				if (updateOperatorDefinition == null || (this.columnsToUpdate != null && updateOperatorDefinition.columnsToUpdate == null) || (this.columnsToUpdate == null && updateOperatorDefinition.columnsToUpdate != null) || (this.columnsToUpdate != null && updateOperatorDefinition.columnsToUpdate != null && this.columnsToUpdate.Count != updateOperatorDefinition.columnsToUpdate.Count) || !this.tableOperatorDefinition.IsEqualsForStatisticPurposes(updateOperatorDefinition.tableOperatorDefinition))
				{
					return false;
				}
				if (this.columnsToUpdate != null)
				{
					for (int i = 0; i < this.columnsToUpdate.Count; i++)
					{
						if (this.columnsToUpdate[i].Name != updateOperatorDefinition.columnsToUpdate[i].Name)
						{
							return false;
						}
					}
				}
				return true;
			}

			private readonly TableOperator.TableOperatorDefinition tableOperatorDefinition;

			private readonly IList<object> valuesToUpdate;

			private readonly IList<Column> columnsToUpdate;
		}
	}
}
