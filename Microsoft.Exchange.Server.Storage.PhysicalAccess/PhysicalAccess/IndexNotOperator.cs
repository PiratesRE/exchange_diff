using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class IndexNotOperator : SimpleQueryOperator
	{
		protected IndexNotOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator queryOperator, SimpleQueryOperator notOperator, bool frequentOperation) : this(connectionProvider, new IndexNotOperator.IndexNotOperatorDefinition(culture, columnsToFetch, queryOperator.OperatorDefinition, notOperator.OperatorDefinition, frequentOperation))
		{
		}

		protected IndexNotOperator(IConnectionProvider connectionProvider, IndexNotOperator.IndexNotOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.queryOperator = ((definition.QueryOperatorDefinition != null) ? definition.QueryOperatorDefinition.CreateOperator(connectionProvider) : null);
			this.notOperator = ((definition.NotOperatorDefinition != null) ? definition.NotOperatorDefinition.CreateOperator(connectionProvider) : null);
		}

		protected SimpleQueryOperator QueryOperator
		{
			get
			{
				return this.queryOperator;
			}
		}

		protected SimpleQueryOperator NotOperator
		{
			get
			{
				return this.notOperator;
			}
		}

		public new IndexNotOperator.IndexNotOperatorDefinition OperatorDefinition
		{
			get
			{
				return (IndexNotOperator.IndexNotOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			if (this.queryOperator != null)
			{
				this.queryOperator.EnumerateDescendants(operatorAction);
			}
			if (this.notOperator != null)
			{
				this.notOperator.EnumerateDescendants(operatorAction);
			}
		}

		public override void RemoveChildren()
		{
			this.queryOperator = null;
			this.notOperator = null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.queryOperator != null)
				{
					this.queryOperator.Dispose();
				}
				if (this.notOperator != null)
				{
					this.notOperator.Dispose();
				}
			}
		}

		private SimpleQueryOperator queryOperator;

		private SimpleQueryOperator notOperator;

		public class IndexNotOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public IndexNotOperatorDefinition(CultureInfo culture, IList<Column> columnsToFetch, SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition, SimpleQueryOperator.SimpleQueryOperatorDefinition notOperatorDefinition, bool frequentOperation) : base(culture, null, columnsToFetch, null, null, 0, 0, frequentOperation)
			{
				this.queryOperatorDefinition = queryOperatorDefinition;
				this.notOperatorDefinition = notOperatorDefinition;
			}

			internal override string OperatorName
			{
				get
				{
					return "IndexNOT";
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

			public SimpleQueryOperator.SimpleQueryOperatorDefinition QueryOperatorDefinition
			{
				get
				{
					return this.queryOperatorDefinition;
				}
			}

			public SimpleQueryOperator.SimpleQueryOperatorDefinition NotOperatorDefinition
			{
				get
				{
					return this.notOperatorDefinition;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateIndexNotOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
				if (this.QueryOperatorDefinition != null)
				{
					this.QueryOperatorDefinition.EnumerateDescendants(operatorDefinitionAction);
				}
				if (this.NotOperatorDefinition != null)
				{
					this.NotOperatorDefinition.EnumerateDescendants(operatorDefinitionAction);
				}
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select indexNot:[");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				bool flag = true;
				foreach (Column column in base.ColumnsToFetch)
				{
					if (!flag)
					{
						sb.Append(", ");
					}
					column.AppendToString(sb, StringFormatOptions.None);
					flag = false;
				}
				sb.Append("]");
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
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  not:[");
				base.Indent(sb, multiLine, nestingLevel + 1, false);
				if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
				{
					sb.Append("op:[");
					sb.Append(this.NotOperatorDefinition.OperatorName);
					sb.Append(" ");
					sb.Append(this.NotOperatorDefinition.GetHashCode());
					sb.Append("] ");
				}
				this.NotOperatorDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  ]");
				if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  frequentOp:[");
					sb.Append(base.FrequentOperation);
					sb.Append("]");
				}
			}

			[Conditional("DEBUG")]
			private void AssertColumnIntegrity()
			{
				for (int i = 0; i < base.ColumnsToFetch.Count; i++)
				{
				}
			}

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				int num;
				int num2;
				this.queryOperatorDefinition.CalculateHashValueForStatisticPurposes(out num, out num2);
				int num3;
				int num4;
				this.notOperatorDefinition.CalculateHashValueForStatisticPurposes(out num3, out num4);
				detail = (59320 ^ num2 ^ num4);
				simple = (59320 ^ num ^ num3);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				IndexNotOperator.IndexNotOperatorDefinition indexNotOperatorDefinition = other as IndexNotOperator.IndexNotOperatorDefinition;
				return indexNotOperatorDefinition != null && this.queryOperatorDefinition.IsEqualsForStatisticPurposes(indexNotOperatorDefinition.queryOperatorDefinition) && this.notOperatorDefinition.IsEqualsForStatisticPurposes(indexNotOperatorDefinition.notOperatorDefinition);
			}

			private SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition;

			private SimpleQueryOperator.SimpleQueryOperatorDefinition notOperatorDefinition;
		}
	}
}
