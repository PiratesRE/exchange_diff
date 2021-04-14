using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class TableFunctionOperator : SimpleQueryOperator
	{
		protected TableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : this(connectionProvider, new TableFunctionOperator.TableFunctionOperatorDefinition(culture, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation))
		{
		}

		protected TableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		internal IList<KeyRange> KeyRanges
		{
			get
			{
				return this.OperatorDefinition.KeyRanges;
			}
		}

		public object[] Parameters
		{
			get
			{
				return this.OperatorDefinition.Parameters;
			}
		}

		private new TableFunctionOperator.TableFunctionOperatorDefinition OperatorDefinition
		{
			get
			{
				return (TableFunctionOperator.TableFunctionOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public virtual uint RowsReturned
		{
			get
			{
				return 0U;
			}
		}

		public TableFunction TableFunction
		{
			get
			{
				return (TableFunction)base.Table;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
		}

		public override void RemoveChildren()
		{
		}

		public class TableFunctionOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public TableFunctionOperatorDefinition(CultureInfo culture, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation) : base(culture, tableFunction, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, frequentOperation)
			{
				this.parameters = parameters;
				this.keyRanges = KeyRange.Normalize(keyRanges, base.Table.PrimaryKeyIndex.SortOrder, (culture == null) ? null : culture.CompareInfo, backwards);
				this.backwards = backwards;
			}

			internal override string OperatorName
			{
				get
				{
					return "TBLFUNC";
				}
			}

			public IList<KeyRange> KeyRanges
			{
				get
				{
					return this.keyRanges;
				}
			}

			public object[] Parameters
			{
				get
				{
					return this.parameters;
				}
			}

			public override SortOrder SortOrder
			{
				get
				{
					return base.Table.PrimaryKeyIndex.SortOrder;
				}
			}

			public override bool Backwards
			{
				get
				{
					return this.backwards;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateTableFunctionOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select function");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (base.Table != null)
				{
					sb.Append(" ");
					sb.Append(base.Table.Name);
				}
				else
				{
					sb.Append(" <null>");
				}
				sb.Append("(");
				if (this.Parameters != null)
				{
					for (int i = 0; i < this.Parameters.Length; i++)
					{
						if (i != 0)
						{
							sb.Append(", ");
						}
						sb.Append("[");
						if ((formatOptions & StringFormatOptions.SkipParametersData) == StringFormatOptions.SkipParametersData && this.Parameters[i] is IEnumerable)
						{
							sb.Append("X");
						}
						else if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || !(this.Parameters[i] is byte[]) || ((byte[])this.Parameters[i]).Length <= 32)
						{
							sb.AppendAsString(this.Parameters[i]);
						}
						else
						{
							sb.Append("<long_blob>");
						}
						sb.Append("]");
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
				if (this.KeyRanges.Count != 1 || !this.KeyRanges[0].IsAllRows)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  KeyRanges:[");
					if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || this.KeyRanges.Count <= 4)
					{
						for (int j = 0; j < this.KeyRanges.Count; j++)
						{
							if (j != 0)
							{
								sb.Append(", ");
							}
							this.KeyRanges[j].AppendToStringBuilder(sb, formatOptions);
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
				detail = (38760 ^ base.Table.GetHashCode() ^ this.keyRanges.Count);
				simple = (38760 ^ base.Table.TableClass.GetHashCode());
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				TableFunctionOperator.TableFunctionOperatorDefinition tableFunctionOperatorDefinition = other as TableFunctionOperator.TableFunctionOperatorDefinition;
				return tableFunctionOperatorDefinition != null && base.Table.Equals(tableFunctionOperatorDefinition.Table) && this.keyRanges.Count == tableFunctionOperatorDefinition.keyRanges.Count;
			}

			private readonly IList<KeyRange> keyRanges;

			private readonly bool backwards;

			private readonly object[] parameters;
		}
	}
}
