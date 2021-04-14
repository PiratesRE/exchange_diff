using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class IndexAndOperator : SimpleQueryOperator
	{
		protected IndexAndOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation) : this(connectionProvider, new IndexAndOperator.IndexAndOperatorDefinition(culture, columnsToFetch, (from op in queryOperators
		select op.OperatorDefinition).ToArray<SimpleQueryOperator.SimpleQueryOperatorDefinition>(), frequentOperation))
		{
		}

		protected IndexAndOperator(IConnectionProvider connectionProvider, IndexAndOperator.IndexAndOperatorDefinition definition) : base(connectionProvider, definition)
		{
			this.queryOperators = ((definition.QueryOperatorDefinitions != null) ? definition.QueryOperatorDefinitions.CreateOperators(connectionProvider).ToArray<SimpleQueryOperator>() : null);
		}

		protected SimpleQueryOperator[] QueryOperators
		{
			get
			{
				return this.queryOperators;
			}
		}

		public new IndexAndOperator.IndexAndOperatorDefinition OperatorDefinition
		{
			get
			{
				return (IndexAndOperator.IndexAndOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
			if (this.QueryOperators != null)
			{
				foreach (SimpleQueryOperator dataAccessOperator in this.QueryOperators)
				{
					dataAccessOperator.EnumerateDescendants(operatorAction);
				}
			}
		}

		public override void RemoveChildren()
		{
			this.queryOperators = null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.queryOperators != null)
			{
				foreach (SimpleQueryOperator simpleQueryOperator in this.queryOperators)
				{
					simpleQueryOperator.Dispose();
				}
			}
		}

		private SimpleQueryOperator[] queryOperators;

		public class IndexAndOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public IndexAndOperatorDefinition(CultureInfo culture, IList<Column> columnsToFetch, SimpleQueryOperator.SimpleQueryOperatorDefinition[] queryOperatorDefinitions, bool frequentOperation) : base(culture, null, columnsToFetch, null, null, 0, 0, frequentOperation)
			{
				this.queryOperatorDefinitions = queryOperatorDefinitions;
			}

			internal override string OperatorName
			{
				get
				{
					return "IndexAND";
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

			public SimpleQueryOperator.SimpleQueryOperatorDefinition[] QueryOperatorDefinitions
			{
				get
				{
					return this.queryOperatorDefinitions;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateIndexAndOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
				if (this.QueryOperatorDefinitions != null)
				{
					foreach (SimpleQueryOperator.SimpleQueryOperatorDefinition dataAccessOperatorDefinition in this.QueryOperatorDefinitions)
					{
						dataAccessOperatorDefinition.EnumerateDescendants(operatorDefinitionAction);
					}
				}
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select indexAnd:[");
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
				foreach (SimpleQueryOperator.SimpleQueryOperatorDefinition simpleQueryOperatorDefinition in this.QueryOperatorDefinitions)
				{
					base.Indent(sb, multiLine, nestingLevel + 1, false);
					if ((formatOptions & StringFormatOptions.IncludeNestedObjectsId) == StringFormatOptions.IncludeNestedObjectsId)
					{
						sb.Append("op:[");
						sb.Append(simpleQueryOperatorDefinition.OperatorName);
						sb.Append(" ");
						sb.Append(simpleQueryOperatorDefinition.GetHashCode());
						sb.Append("] ");
					}
					simpleQueryOperatorDefinition.AppendToStringBuilder(sb, formatOptions, nestingLevel + 1);
				}
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
				for (int i = 0; i < this.QueryOperatorDefinitions.Length; i++)
				{
					SimpleQueryOperator.SimpleQueryOperatorDefinition simpleQueryOperatorDefinition = this.QueryOperatorDefinitions[i];
					for (int j = 0; j < base.ColumnsToFetch.Count; j++)
					{
					}
				}
			}

			internal override void CalculateHashValueForStatisticPurposes(out int simple, out int detail)
			{
				int num;
				int num2;
				this.queryOperatorDefinitions[0].CalculateHashValueForStatisticPurposes(out num, out num2);
				int num3;
				int num4;
				this.queryOperatorDefinitions[1].CalculateHashValueForStatisticPurposes(out num3, out num4);
				detail = (42936 ^ this.queryOperatorDefinitions.Length ^ num2 ^ num4);
				simple = (42936 ^ num ^ num3);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				IndexAndOperator.IndexAndOperatorDefinition indexAndOperatorDefinition = other as IndexAndOperator.IndexAndOperatorDefinition;
				return indexAndOperatorDefinition != null && this.queryOperatorDefinitions.Length == indexAndOperatorDefinition.queryOperatorDefinitions.Length && this.queryOperatorDefinitions[0].IsEqualsForStatisticPurposes(indexAndOperatorDefinition.queryOperatorDefinitions[0]) && this.queryOperatorDefinitions[1].IsEqualsForStatisticPurposes(indexAndOperatorDefinition.queryOperatorDefinitions[1]);
			}

			private SimpleQueryOperator.SimpleQueryOperatorDefinition[] queryOperatorDefinitions;
		}
	}
}
