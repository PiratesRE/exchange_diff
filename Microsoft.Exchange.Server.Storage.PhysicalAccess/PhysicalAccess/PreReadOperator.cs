using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class PreReadOperator : DataAccessOperator
	{
		protected PreReadOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation) : base(connectionProvider, new PreReadOperator.PreReadOperatorDefinition(culture, table, index, keyRanges, longValueColumns, frequentOperation))
		{
		}

		internal IList<KeyRange> KeyRanges
		{
			get
			{
				return this.OperatorDefinition.KeyRanges;
			}
		}

		internal Table Table
		{
			get
			{
				return this.OperatorDefinition.Table;
			}
		}

		internal Index Index
		{
			get
			{
				return this.OperatorDefinition.Index;
			}
		}

		internal IList<Column> LongValueColumns
		{
			get
			{
				return this.OperatorDefinition.LongValueColumns;
			}
		}

		public PreReadOperator.PreReadOperatorDefinition OperatorDefinition
		{
			get
			{
				return (PreReadOperator.PreReadOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
		}

		public override void RemoveChildren()
		{
		}

		public class PreReadOperatorDefinition : DataAccessOperator.DataAccessOperatorDefinition
		{
			public PreReadOperatorDefinition(CultureInfo culture, Table table, Index index, IList<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation) : base(culture, frequentOperation)
			{
				this.table = table;
				this.index = index;
				this.keyRanges = keyRanges;
				this.longValueColumns = longValueColumns;
			}

			internal override string OperatorName
			{
				get
				{
					return "PREREAD";
				}
			}

			internal IList<KeyRange> KeyRanges
			{
				get
				{
					return this.keyRanges;
				}
			}

			internal IList<Column> LongValueColumns
			{
				get
				{
					return this.longValueColumns;
				}
			}

			internal Table Table
			{
				get
				{
					return this.table;
				}
			}

			internal Index Index
			{
				get
				{
					return this.index;
				}
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				sb.Append("PreRead from ");
				sb.Append(this.Table.Name);
				if (this.KeyRanges.Count != 1 || !this.KeyRanges[0].IsAllRows)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  Index:[");
					sb.Append(this.index.Name);
					sb.Append("]");
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
					sb.Append("]  LongValueColumns:[");
					if (this.longValueColumns != null)
					{
						for (int j = 0; j < this.longValueColumns.Count; j++)
						{
							if (j != 0)
							{
								sb.Append(" ,");
							}
							this.longValueColumns[j].AppendToString(sb, formatOptions);
						}
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
				detail = (64616 ^ this.Table.GetHashCode() ^ this.Index.GetHashCode());
				simple = (64616 ^ this.Table.TableClass.GetHashCode());
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				PreReadOperator.PreReadOperatorDefinition preReadOperatorDefinition = other as PreReadOperator.PreReadOperatorDefinition;
				return preReadOperatorDefinition != null && this.Table.Equals(preReadOperatorDefinition.Table) && this.Index.Equals(preReadOperatorDefinition.Index);
			}

			private readonly IList<KeyRange> keyRanges;

			private readonly Table table;

			private readonly Index index;

			private readonly IList<Column> longValueColumns;
		}
	}
}
