using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SortOperator : SimpleQueryOperator
	{
		protected SortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : this(connectionProvider, new SortOperator.SortOperatorDefinition(culture, queryOperator.OperatorDefinition, skipTo, maxRows, sortOrder, keyRanges, backwards, frequentOperation))
		{
		}

		protected SortOperator(IConnectionProvider connectionProvider, SortOperator.SortOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.queryOperator = definition.QueryOperatorDefinition.CreateOperator(connectionProvider);
		}

		protected SimpleQueryOperator QueryOperator
		{
			get
			{
				return this.queryOperator;
			}
		}

		internal IList<KeyRange> KeyRanges
		{
			get
			{
				return this.OperatorDefinition.KeyRanges;
			}
		}

		public new SortOperator.SortOperatorDefinition OperatorDefinition
		{
			get
			{
				return (SortOperator.SortOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			if (this.queryOperator != null)
			{
				this.queryOperator.EnumerateDescendants(operatorAction);
			}
		}

		public override void RemoveChildren()
		{
			this.queryOperator = null;
		}

		public override IExecutionPlanner GetExecutionPlanner()
		{
			return base.GetExecutionPlanner() ?? DataAccessOperator.GetExecutionPlannerOrNull(this.queryOperator);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.queryOperator != null)
			{
				this.queryOperator.Dispose();
			}
		}

		private SimpleQueryOperator queryOperator;

		public class SortOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public SortOperatorDefinition(CultureInfo culture, SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : base(culture, null, queryOperatorDefinition.ColumnsToFetch, null, null, skipTo, maxRows, frequentOperation)
			{
				this.queryOperatorDefinition = queryOperatorDefinition;
				this.sortOrder = sortOrder;
				this.keyRanges = KeyRange.Normalize(keyRanges, sortOrder, (culture == null) ? null : culture.CompareInfo, backwards);
				this.backwards = backwards;
			}

			internal override string OperatorName
			{
				get
				{
					return "SORT";
				}
			}

			public override SortOrder SortOrder
			{
				get
				{
					return this.sortOrder;
				}
			}

			public IList<KeyRange> KeyRanges
			{
				get
				{
					return this.keyRanges;
				}
			}

			public override bool Backwards
			{
				get
				{
					return this.backwards;
				}
			}

			public SimpleQueryOperator.SimpleQueryOperatorDefinition QueryOperatorDefinition
			{
				get
				{
					return this.queryOperatorDefinition;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateSortOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
				if (this.QueryOperatorDefinition != null)
				{
					this.QueryOperatorDefinition.EnumerateDescendants(operatorDefinitionAction);
				}
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select sort");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (this.QueryOperatorDefinition != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  from:[");
					base.Indent(sb, multiLine, nestingLevel + 1, false);
					if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
					{
						sb.Append("op:[");
						sb.Append(this.QueryOperatorDefinition.OperatorName);
						sb.Append(" ");
						sb.Append(this.QueryOperatorDefinition.GetHashCode());
						sb.Append("] ");
					}
					this.QueryOperatorDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  ]");
				}
				if (!this.SortOrder.IsEmpty)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  order_by:[");
					this.SortOrder.AppendToStringBuilder(sb, formatOptions);
					sb.Append("]");
				}
				if (this.KeyRanges.Count != 1 || !this.KeyRanges[0].IsAllRows)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  KeyRanges:[");
					if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || this.KeyRanges.Count <= 4)
					{
						for (int i = 0; i < this.KeyRanges.Count; i++)
						{
							if (i != 0)
							{
								sb.Append(" ,");
							}
							this.KeyRanges[i].AppendToStringBuilder(sb, formatOptions);
						}
					}
					else
					{
						if ((formatOptions & StringFormatOptions.SkipParametersData) == StringFormatOptions.None)
						{
							sb.Append(this.KeyRanges.Count);
						}
						else
						{
							sb.Append("multiple");
						}
						sb.Append(" ranges");
					}
					sb.Append("]");
				}
				if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || this.Backwards)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  backwards:[");
					sb.Append(this.Backwards);
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
				this.queryOperatorDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				detail = (63336 ^ this.sortOrder.GetHashCode() ^ this.keyRanges.Count ^ num2);
				simple = (63336 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				SortOperator.SortOperatorDefinition sortOperatorDefinition = other as SortOperator.SortOperatorDefinition;
				return sortOperatorDefinition != null && this.sortOrder.Equals(sortOperatorDefinition.sortOrder) && this.keyRanges.Count == sortOperatorDefinition.keyRanges.Count && this.queryOperatorDefinition.IsEqualsForStatisticPurposes(sortOperatorDefinition.queryOperatorDefinition);
			}

			private readonly SortOrder sortOrder;

			private readonly IList<KeyRange> keyRanges;

			private readonly bool backwards;

			private SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition;
		}
	}
}
