using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class TableOperator : SimpleQueryOperator
	{
		protected TableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation) : this(connectionProvider, new TableOperator.TableOperatorDefinition(culture, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, opportunedPreread, frequentOperation))
		{
		}

		protected TableOperator(IConnectionProvider connectionProvider, TableOperator.TableOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		public int PrereadChunkSize
		{
			get
			{
				return this.prereadChunkSize;
			}
			set
			{
				this.prereadChunkSize = value;
			}
		}

		internal IList<KeyRange> KeyRanges
		{
			get
			{
				return this.OperatorDefinition.KeyRanges;
			}
		}

		internal Index Index
		{
			get
			{
				return this.OperatorDefinition.Index;
			}
		}

		public IList<Column> LongValueColumnsToPreread
		{
			get
			{
				return this.OperatorDefinition.LongValueColumnsToPreread;
			}
		}

		public bool OpportunedPreread
		{
			get
			{
				return this.OperatorDefinition.OpportunedPreread;
			}
		}

		public new TableOperator.TableOperatorDefinition OperatorDefinition
		{
			get
			{
				return (TableOperator.TableOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
		}

		public override void RemoveChildren()
		{
		}

		public override bool OperatorUsesTablePartition(Table table, IList<object> partitionKeyPrefix)
		{
			if (base.Table != table || this.KeyRanges.Count == 0)
			{
				return false;
			}
			int numberOfPartioningColumns = base.Table.SpecialCols.NumberOfPartioningColumns;
			int num = numberOfPartioningColumns;
			StartStopKey startKey = this.KeyRanges[0].StartKey;
			return num == StartStopKey.CommonKeyPrefix(startKey.Values, partitionKeyPrefix, CultureHelper.DefaultCultureInfo.CompareInfo);
		}

		protected const int DefaultPrereadChunkSize = 150;

		private int prereadChunkSize = 150;

		public class TableOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public TableOperatorDefinition(CultureInfo culture, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation) : base(culture, table, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, frequentOperation)
			{
				this.index = index;
				this.keyRanges = keyRanges;
				this.backwards = backwards;
				this.opportunedPreread = opportunedPreread;
				this.keyRanges = KeyRange.Normalize(this.keyRanges, index.SortOrder, base.CompareInfo, backwards);
				this.longValueColumnsToPreread = longValueColumnsToPreread;
			}

			internal override string OperatorName
			{
				get
				{
					return "TABLE";
				}
			}

			public IList<KeyRange> KeyRanges
			{
				get
				{
					return this.keyRanges;
				}
			}

			public Index Index
			{
				get
				{
					return this.index;
				}
			}

			public bool OpportunedPreread
			{
				get
				{
					return this.opportunedPreread;
				}
			}

			public override bool Backwards
			{
				get
				{
					return this.backwards;
				}
			}

			public override SortOrder SortOrder
			{
				get
				{
					return this.Index.SortOrder;
				}
			}

			public IList<Column> LongValueColumnsToPreread
			{
				get
				{
					return this.longValueColumnsToPreread;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateTableOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select");
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				if (base.Table != null)
				{
					sb.Append(" from ");
					sb.Append(base.Table.Name);
				}
				if (base.ColumnsToFetch != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  fetch:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, base.ColumnsToFetch, null, formatOptions);
					sb.Append("]");
				}
				if (this.LongValueColumnsToPreread != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  lvpreread:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, this.LongValueColumnsToPreread, null, formatOptions);
					sb.Append("]");
				}
				if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || this.Index != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  index:[");
					sb.Append(this.Index.Name);
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
				int num;
				int num2;
				base.CalculateHashValueForStatisticPurposes(out num, out num2);
				detail = (42856 ^ base.Table.GetHashCode() ^ this.keyRanges.Count ^ num2);
				simple = (42856 ^ base.Table.TableClass.GetHashCode() ^ num);
			}

			internal override bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other)
			{
				if (object.ReferenceEquals(this, other))
				{
					return true;
				}
				TableOperator.TableOperatorDefinition tableOperatorDefinition = other as TableOperator.TableOperatorDefinition;
				return tableOperatorDefinition != null && base.Table.Equals(tableOperatorDefinition.Table) && this.keyRanges.Count == tableOperatorDefinition.keyRanges.Count && base.IsEqualsForStatisticPurposes(other);
			}

			private readonly IList<KeyRange> keyRanges;

			private readonly Index index;

			private readonly bool backwards;

			private readonly bool opportunedPreread;

			private IList<Column> longValueColumnsToPreread;
		}
	}
}
