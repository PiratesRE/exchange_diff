using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class CountOperator : DataAccessOperator
	{
		protected CountOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, bool frequentOperation) : this(connectionProvider, new CountOperator.CountOperatorDefinition(culture, (queryOperator != null) ? queryOperator.OperatorDefinition : null, frequentOperation))
		{
		}

		protected CountOperator(IConnectionProvider connectionProvider, CountOperator.CountOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.queryOperator = ((definition.QueryOperatorDefinition != null) ? definition.QueryOperatorDefinition.CreateOperator(connectionProvider) : null);
		}

		public CountOperator.CountOperatorDefinition OperatorDefinition
		{
			get
			{
				return (CountOperator.CountOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		internal SimpleQueryOperator QueryOperator
		{
			get
			{
				return this.queryOperator;
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

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.queryOperator != null)
			{
				this.queryOperator.Dispose();
			}
		}

		private readonly SimpleQueryOperator queryOperator;

		public class CountOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			public CountOperatorDefinition(CultureInfo culture, SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.queryOperatorDefinition = queryOperatorDefinition;
			}

			internal override string OperatorName
			{
				get
				{
					return "COUNT";
				}
			}

			public SimpleQueryOperator.SimpleQueryOperatorDefinition QueryOperatorDefinition
			{
				get
				{
					return this.queryOperatorDefinition;
				}
			}

			public CountOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateCountOperator(connectionProvider, this);
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
				sb.Append("select count");
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
						sb.Append("]");
					}
					this.QueryOperatorDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
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
				int num = 0;
				int num2 = 0;
				if (this.queryOperatorDefinition != null)
				{
					this.queryOperatorDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				}
				detail = (38840 ^ num2);
				simple = (38840 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				CountOperator.CountOperatorDefinition countOperatorDefinition = other as CountOperator.CountOperatorDefinition;
				return countOperatorDefinition != null && ((this.queryOperatorDefinition == null && countOperatorDefinition.queryOperatorDefinition == null) || (this.queryOperatorDefinition != null && countOperatorDefinition.queryOperatorDefinition != null && this.queryOperatorDefinition.IsEqualsForStatisticPurposes(countOperatorDefinition.queryOperatorDefinition)));
			}

			private readonly SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition;
		}
	}
}
