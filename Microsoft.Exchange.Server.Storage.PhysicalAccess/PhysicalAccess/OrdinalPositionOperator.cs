using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class OrdinalPositionOperator : DataAccessOperator
	{
		protected OrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation) : this(connectionProvider, new OrdinalPositionOperator.OrdinalPositionOperatorDefinition(culture, (queryOperator != null) ? queryOperator.OperatorDefinition : null, keySortOrder, key, frequentOperation))
		{
		}

		protected OrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.queryOperator = ((definition.QueryOperatorDefinition != null) ? definition.QueryOperatorDefinition.CreateOperator(connectionProvider) : null);
		}

		protected SimpleQueryOperator QueryOperator
		{
			get
			{
				return this.queryOperator;
			}
		}

		protected SortOrder KeySortOrder
		{
			get
			{
				return this.OperatorDefinition.KeySortOrder;
			}
		}

		protected StartStopKey Key
		{
			get
			{
				return this.OperatorDefinition.Key;
			}
		}

		public OrdinalPositionOperator.OrdinalPositionOperatorDefinition OperatorDefinition
		{
			get
			{
				return (OrdinalPositionOperator.OrdinalPositionOperatorDefinition)base.OperatorDefinitionBase;
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

		public class OrdinalPositionOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			public OrdinalPositionOperatorDefinition(CultureInfo culture, SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition, SortOrder keySortOrder, StartStopKey key, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.queryOperatorDefinition = queryOperatorDefinition;
				this.keySortOrder = keySortOrder;
				this.key = key;
			}

			internal override string OperatorName
			{
				get
				{
					return "ORDINAL";
				}
			}

			public SortOrder KeySortOrder
			{
				get
				{
					return this.keySortOrder;
				}
			}

			public StartStopKey Key
			{
				get
				{
					return this.key;
				}
			}

			public SimpleQueryOperator.SimpleQueryOperatorDefinition QueryOperatorDefinition
			{
				get
				{
					return this.queryOperatorDefinition;
				}
			}

			public OrdinalPositionOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateOrdinalPositionOperator(connectionProvider, this);
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
				sb.Append("select ordinal position");
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
				if (!this.Key.IsEmpty)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  key:");
					this.Key.AppendToStringBuilder(sb, formatOptions);
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
			private void ValidateKey()
			{
				for (int i = 0; i < this.key.Count; i++)
				{
				}
				if (this.QueryOperatorDefinition != null)
				{
					for (int j = 0; j < this.key.Count; j++)
					{
					}
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
				detail = (46952 ^ num2);
				simple = (46952 ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				OrdinalPositionOperator.OrdinalPositionOperatorDefinition ordinalPositionOperatorDefinition = other as OrdinalPositionOperator.OrdinalPositionOperatorDefinition;
				return ordinalPositionOperatorDefinition != null && ((this.queryOperatorDefinition == null && ordinalPositionOperatorDefinition.queryOperatorDefinition == null) || (this.queryOperatorDefinition != null && ordinalPositionOperatorDefinition.queryOperatorDefinition != null && this.queryOperatorDefinition.IsEqualsForStatisticPurposes(ordinalPositionOperatorDefinition.queryOperatorDefinition)));
			}

			private readonly SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition;

			private readonly SortOrder keySortOrder;

			private readonly StartStopKey key;
		}
	}
}
