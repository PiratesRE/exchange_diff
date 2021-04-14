using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class CategorizedTableOperator : SimpleQueryOperator
	{
		protected CategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation) : this(connectionProvider, new CategorizedTableOperator.CategorizedTableOperatorDefinition(culture, table, categorizedTableParams, collapseState, columnsToFetch, additionalHeaderRenameDictionary, additionalLeafRenameDictionary, (restriction is SearchCriteriaTrue) ? null : restriction, skipTo, maxRows, keyRange, backwards, frequentOperation))
		{
		}

		protected CategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition) : base(connectionProvider, definition)
		{
			if (ExTraceGlobals.CategorizedTableOperatorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategorizedTableOperatorTracer.TraceDebug<CategorizedTableOperator, KeyRange>(0L, "{0}  keyRange:[{1}]", this, definition.KeyRange);
			}
		}

		internal Reader HeaderReader
		{
			get
			{
				if (this.headerReader == null)
				{
					return null;
				}
				return this.headerReader.InternalReader;
			}
		}

		internal Reader LeafReader
		{
			get
			{
				if (this.leafReader == null)
				{
					return null;
				}
				return this.leafReader.InternalReader;
			}
		}

		internal Table HeaderTable
		{
			get
			{
				return this.OperatorDefinition.HeaderTable;
			}
		}

		internal Table LeafTable
		{
			get
			{
				return this.OperatorDefinition.LeafTable;
			}
		}

		internal int CategoryCount
		{
			get
			{
				return this.OperatorDefinition.CategoryCount;
			}
		}

		internal bool BaseMessageViewInReverseOrder
		{
			get
			{
				return this.OperatorDefinition.BaseMessageViewInReverseOrder;
			}
		}

		internal CategorizedTableCollapseState CollapseState
		{
			get
			{
				return this.OperatorDefinition.CollapseState;
			}
		}

		internal IList<Column> HeaderColumnsToFetch
		{
			get
			{
				return this.OperatorDefinition.HeaderColumnsToFetch;
			}
		}

		internal IList<Column> LeafColumnsToFetch
		{
			get
			{
				return this.OperatorDefinition.LeafColumnsToFetch;
			}
		}

		internal IList<Column> LeafColumnsToFetchWithoutJoin
		{
			get
			{
				return this.OperatorDefinition.LeafColumnsToFetchWithoutJoin;
			}
		}

		internal bool AtLeastOneExternalColumn
		{
			get
			{
				return this.OperatorDefinition.AtLeastOneExternalColumn;
			}
		}

		internal IReadOnlyDictionary<Column, Column> HeaderRenameDictionary
		{
			get
			{
				return this.OperatorDefinition.HeaderRenameDictionary;
			}
		}

		internal IReadOnlyDictionary<Column, Column> LeafIndexRenameDictionary
		{
			get
			{
				return this.OperatorDefinition.LeafIndexRenameDictionary;
			}
		}

		internal IReadOnlyDictionary<Column, Column> LeafMessageRenameDictionary
		{
			get
			{
				return this.OperatorDefinition.LeafMessageRenameDictionary;
			}
		}

		internal SortOrder HeaderLogicalSortOrder
		{
			get
			{
				return this.OperatorDefinition.HeaderLogicalSortOrder;
			}
		}

		internal SortOrder LeafLogicalSortOrder
		{
			get
			{
				return this.OperatorDefinition.LeafLogicalSortOrder;
			}
		}

		internal IList<object> HeaderKeyPrefix
		{
			get
			{
				return this.OperatorDefinition.HeaderKeyPrefix;
			}
		}

		internal IList<object> LeafKeyPrefix
		{
			get
			{
				return this.OperatorDefinition.LeafKeyPrefix;
			}
		}

		internal KeyRange HeaderKeyRange
		{
			get
			{
				return this.OperatorDefinition.HeaderKeyRange;
			}
		}

		internal KeyRange LeafKeyRange
		{
			get
			{
				return this.OperatorDefinition.LeafKeyRange;
			}
		}

		internal bool IsInclusiveStartKey
		{
			get
			{
				return this.OperatorDefinition.IsInclusiveStartKey;
			}
		}

		internal IList<StorePropTag> HeaderOnlyPropTags
		{
			get
			{
				return this.OperatorDefinition.HeaderOnlyPropTags;
			}
		}

		internal Column DepthColumn
		{
			get
			{
				return this.OperatorDefinition.DepthColumn;
			}
		}

		internal Column CategIdColumn
		{
			get
			{
				return this.OperatorDefinition.CategIdColumn;
			}
		}

		internal Column RowTypeColumn
		{
			get
			{
				return this.OperatorDefinition.RowTypeColumn;
			}
		}

		public new CategorizedTableOperator.CategorizedTableOperatorDefinition OperatorDefinition
		{
			get
			{
				return (CategorizedTableOperator.CategorizedTableOperatorDefinition)base.OperatorDefinitionBase;
			}
		}

		public override Reader ExecuteReader(bool disposeQueryOperator)
		{
			base.TraceOperation("ExecuteReader");
			Reader result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				result = new CategorizedTableOperator.CategorizedTableReader(base.Connection, this, disposeQueryOperator);
			}
			return result;
		}

		public override void EnumerateDescendants(Action<DataAccessOperator> operatorAction)
		{
			operatorAction(this);
		}

		protected bool MoveFirst(out int rowsSkipped)
		{
			rowsSkipped = 0;
			if (this.leafReader != null)
			{
				this.leafReader.CloseReader();
				this.leafReader = null;
			}
			if (this.headerReader != null)
			{
				this.headerReader.CloseReader();
				this.headerReader = null;
			}
			this.rowsReturned = 0;
			StartStopKey startKey = this.LeafKeyRange.StartKey;
			bool flag = startKey.Count > this.LeafKeyPrefix.Count;
			bool flag2 = base.Backwards && !flag;
			TableOperator tableOperator = Factory.CreateTableOperator(base.Culture, base.Connection, this.HeaderTable, this.HeaderTable.PrimaryKeyIndex, this.HeaderColumnsToFetch, null, this.HeaderRenameDictionary, 0, 0, this.HeaderKeyRange, base.Backwards, base.FrequentOperation);
			this.headerReader = new CategorizedTableOperator.HeaderOrLeafReader(tableOperator.ExecuteReader(true));
			bool flag3 = this.HeaderReader.Read();
			if (!flag3)
			{
				base.TraceMove("MoveFirst", false);
			}
			else if (!this.IsHeaderVisible())
			{
				flag3 = this.MoveNext("MoveFirst", base.SkipTo, true, ref rowsSkipped);
			}
			else if ((flag && this.CheckHeaderRowMatchesLeafStartKey()) || (flag2 && this.CheckStartOnLastLeafRowOfHeader()))
			{
				if (!this.IsLowestHeaderLevel())
				{
					throw new StoreException((LID)37663U, ErrorCodeValue.InvalidBookmark, "Leaf bookmark was provided, but header bookmark does not correspond to a header row at the lowest category header level.");
				}
				this.GetLeafReaderIfNecessary(flag);
				if (this.LeafReader != null)
				{
					if (!this.LeafReader.Read())
					{
						Reader reader = this.LeafReader;
						flag3 = this.MoveNext("MoveFirst", base.SkipTo, true, ref rowsSkipped);
					}
					else
					{
						if (!this.CheckMatchingHeaderAndLeaf())
						{
							throw new StoreException((LID)62239U, ErrorCodeValue.InvalidBookmark, "Header and Leaf row bookmarks do not match.");
						}
						if (base.Criteria != null)
						{
							bool flag4 = base.Criteria.Evaluate(this.leafReader, base.CompareInfo);
							bool? flag5 = new bool?(true);
							if (!flag4 || flag5 == null)
							{
								return this.MoveNext("MoveFirst", base.SkipTo, false, ref rowsSkipped);
							}
						}
						if (base.SkipTo > 0)
						{
							rowsSkipped++;
							flag3 = this.MoveNext("MoveFirst", base.SkipTo - 1, false, ref rowsSkipped);
						}
						else
						{
							this.rowsReturned++;
							base.TraceMove("Leaf MoveFirst", true);
						}
					}
				}
				else
				{
					flag3 = this.MoveNext("MoveFirst", base.SkipTo, true, ref rowsSkipped);
				}
			}
			else
			{
				bool isInclusiveStartKey = this.IsInclusiveStartKey;
				StartStopKey startKey2 = this.HeaderKeyRange.StartKey;
				if (isInclusiveStartKey != startKey2.Inclusive && this.CheckHeaderRowMatchesHeaderStartKey())
				{
					flag3 = this.MoveNext("MoveFirst", base.SkipTo, false, ref rowsSkipped);
				}
				else
				{
					if (base.Criteria != null)
					{
						bool flag6 = base.Criteria.Evaluate(this.headerReader, base.CompareInfo);
						bool? flag7 = new bool?(true);
						if (!flag6 || flag7 == null)
						{
							return this.MoveNext("MoveFirst", base.SkipTo, base.Backwards, ref rowsSkipped);
						}
					}
					if (base.SkipTo > 0)
					{
						rowsSkipped++;
						flag3 = this.MoveNext("MoveFirst", base.SkipTo - 1, base.Backwards, ref rowsSkipped);
					}
					else
					{
						this.rowsReturned++;
						base.TraceMove("Header MoveFirst", true);
					}
				}
			}
			return flag3;
		}

		private bool MoveNext()
		{
			bool forceNextHeader = base.Backwards && this.LeafReader == null;
			int num = 0;
			return this.MoveNext("MoveNext", 0, forceNextHeader, ref num);
		}

		protected bool MoveNext(string operation, int numberLeftToSkip, bool forceNextHeader, ref int rowsSkipped)
		{
			if (base.MaxRows > 0 && this.rowsReturned >= base.MaxRows)
			{
				base.TraceMove(operation + " reached MaxRows", false);
				return false;
			}
			Column column = (numberLeftToSkip > 1 && (base.Criteria == null || base.Criteria is SearchCriteriaTrue)) ? this.GetContentCountColumn() : null;
			bool flag = false;
			IL_28C:
			while (!flag)
			{
				if (!forceNextHeader)
				{
					bool flag2 = false;
					if (column != null && numberLeftToSkip > 1 && this.LeafReader == null && this.IsLeafVisible())
					{
						int num = this.HeaderReader.GetInt32(column);
						if (base.Backwards)
						{
							num++;
						}
						if (num <= numberLeftToSkip)
						{
							numberLeftToSkip -= num;
							rowsSkipped += num;
							flag2 = true;
						}
					}
					if (!flag2)
					{
						this.GetLeafReaderIfNecessary(false);
					}
				}
				if (this.LeafReader != null)
				{
					if (this.IsHeaderExpanded() && !forceNextHeader)
					{
						while (this.LeafReader.Read())
						{
							if (base.Criteria != null)
							{
								bool flag3 = base.Criteria.Evaluate(this.leafReader, base.CompareInfo);
								bool? flag4 = new bool?(true);
								if (!flag3 || flag4 == null)
								{
									continue;
								}
							}
							if (numberLeftToSkip <= 0)
							{
								this.rowsReturned++;
								base.TraceMove("Leaf " + operation, true);
								return true;
							}
							rowsSkipped++;
							numberLeftToSkip--;
						}
					}
					this.leafReader.CloseReader();
					this.leafReader = null;
					if (base.Backwards)
					{
						if (base.Criteria != null)
						{
							bool flag5 = base.Criteria.Evaluate(this.headerReader, base.CompareInfo);
							bool? flag6 = new bool?(true);
							if (!flag5 || flag6 == null)
							{
								goto IL_1DF;
							}
						}
						if (numberLeftToSkip <= 0)
						{
							this.rowsReturned++;
							base.TraceMove("Header " + operation, true);
							return true;
						}
						rowsSkipped++;
						numberLeftToSkip--;
					}
				}
				IL_1DF:
				forceNextHeader = false;
				while (this.HeaderReader.Read())
				{
					if (this.IsHeaderVisible())
					{
						if (base.Backwards && this.IsLeafVisible())
						{
							goto IL_28C;
						}
						if (base.Criteria != null)
						{
							bool flag7 = base.Criteria.Evaluate(this.headerReader, base.CompareInfo);
							bool? flag8 = new bool?(true);
							if (!flag7 || flag8 == null)
							{
								if (this.IsLeafVisible())
								{
									goto IL_28C;
								}
								continue;
							}
						}
						if (numberLeftToSkip > 0)
						{
							rowsSkipped++;
							numberLeftToSkip--;
							goto IL_28C;
						}
						this.rowsReturned++;
						base.TraceMove("Header " + operation, true);
						return true;
					}
				}
				flag = true;
			}
			base.TraceMove(operation, false);
			return false;
		}

		private void GetLeafReaderIfNecessary(bool useLeafBookmark)
		{
			if (this.LeafReader == null && this.IsLeafVisible())
			{
				IList<object> list = new List<object>(this.LeafKeyPrefix);
				int count = this.LeafKeyPrefix.Count;
				for (int i = 0; i < this.CategoryCount; i++)
				{
					Column column = this.LeafLogicalSortOrder.Columns[i];
					object value = this.HeaderReader.GetValue(column);
					list.Add(value);
				}
				StartStopKey startStopKey = new StartStopKey(true, list);
				StartStopKey startKey = useLeafBookmark ? this.LeafKeyRange.StartKey : startStopKey;
				this.TraceLeafReader(startKey, useLeafBookmark);
				TableOperator tableOperator = Factory.CreateTableOperator(base.Culture, base.Connection, this.LeafTable, this.LeafTable.PrimaryKeyIndex, this.LeafColumnsToFetchWithoutJoin, null, this.LeafIndexRenameDictionary, 0, 0, new KeyRange(startKey, startStopKey), base.Backwards ^ this.BaseMessageViewInReverseOrder, base.FrequentOperation);
				SimpleQueryOperator simpleQueryOperator;
				if (this.AtLeastOneExternalColumn)
				{
					using (tableOperator)
					{
						simpleQueryOperator = Factory.CreateJoinOperator(base.Culture, base.Connection, base.Table, this.LeafColumnsToFetch, null, this.LeafMessageRenameDictionary, 0, 0, base.Table.PrimaryKeyIndex.Columns, tableOperator, base.FrequentOperation);
						goto IL_13C;
					}
				}
				simpleQueryOperator = tableOperator;
				try
				{
					IL_13C:
					this.leafReader = new CategorizedTableOperator.HeaderOrLeafReader(simpleQueryOperator.ExecuteReader(true));
					simpleQueryOperator = null;
				}
				finally
				{
					if (simpleQueryOperator != null)
					{
						simpleQueryOperator.Dispose();
						simpleQueryOperator = null;
					}
				}
			}
		}

		private void TraceLeafReader(StartStopKey startKey, bool useLeafBookmark)
		{
			if (ExTraceGlobals.CategorizedTableOperatorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("cn:[");
				stringBuilder.Append(base.Connection.GetHashCode());
				stringBuilder.Append("] ");
				stringBuilder.Append("GetLeafReader");
				stringBuilder.Append(" op:[");
				stringBuilder.Append(base.OperatorName);
				stringBuilder.Append(" ");
				stringBuilder.Append(this.GetHashCode());
				stringBuilder.Append("]");
				stringBuilder.Append(" useLeafBookmark:[");
				stringBuilder.Append(useLeafBookmark);
				stringBuilder.Append("]");
				stringBuilder.Append(" startKey(" + startKey.Count + "):[");
				startKey.AppendToStringBuilder(stringBuilder, StringFormatOptions.None);
				stringBuilder.Append("]");
				stringBuilder.Append(" atLeastOneExternalColumn:[");
				stringBuilder.Append(this.AtLeastOneExternalColumn);
				stringBuilder.Append("]");
				ExTraceGlobals.CategorizedTableOperatorTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private bool CheckStartOnLastLeafRowOfHeader()
		{
			bool result = false;
			if (this.IsLeafVisible())
			{
				StartStopKey startKey = this.HeaderKeyRange.StartKey;
				if (startKey.Count <= this.HeaderKeyPrefix.Count)
				{
					result = true;
				}
				else
				{
					StartStopKey startKey2 = this.HeaderKeyRange.StartKey;
					if (!startKey2.Inclusive)
					{
						result = true;
					}
					else if (!this.CheckHeaderRowMatchesHeaderStartKey())
					{
						result = true;
					}
				}
			}
			return result;
		}

		private bool CheckHeaderRowMatchesHeaderStartKey()
		{
			int num = this.HeaderKeyPrefix.Count;
			for (;;)
			{
				int num2 = num;
				StartStopKey startKey = this.HeaderKeyRange.StartKey;
				if (num2 >= startKey.Count)
				{
					return true;
				}
				StartStopKey startKey2 = this.HeaderKeyRange.StartKey;
				object x = startKey2.Values[num];
				Column column = this.HeaderLogicalSortOrder.Columns[num - this.HeaderKeyPrefix.Count];
				object value = this.HeaderReader.GetValue(column);
				if (!ValueHelper.ValuesEqual(x, value, base.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
				{
					break;
				}
				num++;
			}
			return false;
		}

		private bool CheckHeaderRowMatchesLeafStartKey()
		{
			for (int i = 0; i < this.CategoryCount; i++)
			{
				Column column = this.LeafLogicalSortOrder.Columns[i];
				object value = this.HeaderReader.GetValue(column);
				StartStopKey startKey = this.LeafKeyRange.StartKey;
				object y = startKey.Values[this.LeafKeyPrefix.Count + i];
				if (!ValueHelper.ValuesEqual(value, y, base.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckMatchingHeaderAndLeaf()
		{
			for (int i = 0; i < this.CategoryCount; i++)
			{
				Column column = this.LeafLogicalSortOrder.Columns[i];
				object value = this.HeaderReader.GetValue(column);
				object obj = this.LeafReader.GetValue(column);
				Column column2 = this.HeaderRenameDictionary[column];
				bool flag;
				obj = ValueHelper.TruncateValueIfNecessary(obj, column2.Type, column2.MaxLength, out flag);
				if (!ValueHelper.ValuesEqual(value, obj, base.CompareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsHeaderExpanded()
		{
			int @int = this.HeaderReader.GetInt32(this.DepthColumn);
			long int2 = this.HeaderReader.GetInt64(this.CategIdColumn);
			return this.CollapseState.IsHeaderExpanded(@int, int2);
		}

		private bool IsHeaderVisible()
		{
			int @int = this.HeaderReader.GetInt32(this.DepthColumn);
			long int2 = this.HeaderReader.GetInt64(this.CategIdColumn);
			return this.CollapseState.IsHeaderVisible(@int, int2);
		}

		protected bool IsLowestHeaderLevel()
		{
			int @int = this.HeaderReader.GetInt32(this.DepthColumn);
			return @int == this.CategoryCount - 1;
		}

		protected bool IsLeafVisible()
		{
			return this.IsLowestHeaderLevel() && this.IsHeaderExpanded();
		}

		protected Column GetContentCountColumn()
		{
			foreach (Column column in this.HeaderColumnsToFetch)
			{
				if (column is ExtendedPropertyColumn && ((ExtendedPropertyColumn)column).StorePropTag.PropTag == 906100739U)
				{
					return column;
				}
			}
			return null;
		}

		protected Reader CurrentReader(Column columnToRetrieve)
		{
			return (this.leafReader == null || this.MustRetrieveFromHeader(columnToRetrieve)) ? this.headerReader.InternalReader : this.leafReader.InternalReader;
		}

		private bool MustRetrieveFromHeader(Column columnToRetrieve)
		{
			if (this.HeaderOnlyPropTags == null)
			{
				return false;
			}
			ExtendedPropertyColumn extendedPropertyColumn = columnToRetrieve as ExtendedPropertyColumn;
			return extendedPropertyColumn != null && this.HeaderOnlyPropTags.Contains(extendedPropertyColumn.StorePropTag);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CategorizedTableOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.leafReader != null)
				{
					this.leafReader.CloseReader();
					this.leafReader = null;
				}
				if (this.headerReader != null)
				{
					this.headerReader.CloseReader();
					this.headerReader = null;
				}
			}
		}

		private CategorizedTableOperator.HeaderOrLeafReader headerReader;

		private CategorizedTableOperator.HeaderOrLeafReader leafReader;

		private int rowsReturned;

		public class CategorizedTableOperatorDefinition : SimpleQueryOperator.SimpleQueryOperatorDefinition
		{
			public CategorizedTableOperatorDefinition(CultureInfo culture, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation) : base(culture, table, columnsToFetch, restriction, null, skipTo, maxRows, frequentOperation)
			{
				this.categorizedTableParams = categorizedTableParams;
				this.collapseState = collapseState;
				this.isInclusiveStartKey = keyRange.StartKey.Inclusive;
				this.backwards = backwards;
				this.keyRange = keyRange;
				this.ConfigureHeaderAndLeafColumnsToFetchAndRenameDictionaries(table, columnsToFetch, categorizedTableParams, collapseState, restriction, additionalHeaderRenameDictionary, additionalLeafRenameDictionary);
				this.ComputeHeaderAndLeafKeyRanges(keyRange, categorizedTableParams, backwards);
			}

			internal override string OperatorName
			{
				get
				{
					return "CATEGORIZED TABLE";
				}
			}

			public override bool Backwards
			{
				get
				{
					return this.backwards;
				}
			}

			public CategorizedTableParams TableParams
			{
				get
				{
					return this.categorizedTableParams;
				}
			}

			public KeyRange KeyRange
			{
				get
				{
					return this.keyRange;
				}
			}

			internal Table HeaderTable
			{
				get
				{
					return this.categorizedTableParams.HeaderTable;
				}
			}

			internal Table LeafTable
			{
				get
				{
					return this.categorizedTableParams.LeafTable;
				}
			}

			internal int CategoryCount
			{
				get
				{
					return this.categorizedTableParams.CategoryCount;
				}
			}

			internal bool BaseMessageViewInReverseOrder
			{
				get
				{
					return this.categorizedTableParams.BaseMessageViewInReverseOrder;
				}
			}

			public CategorizedTableCollapseState CollapseState
			{
				get
				{
					return this.collapseState;
				}
			}

			internal IList<Column> HeaderColumnsToFetch
			{
				get
				{
					return this.headerColumnsToFetch;
				}
			}

			internal IList<Column> LeafColumnsToFetch
			{
				get
				{
					return this.leafColumnsToFetch;
				}
			}

			internal IList<Column> LeafColumnsToFetchWithoutJoin
			{
				get
				{
					return this.leafColumnsToFetchWithoutJoin;
				}
			}

			internal bool AtLeastOneExternalColumn
			{
				get
				{
					return this.atLeastOneExternalColumn;
				}
			}

			internal IReadOnlyDictionary<Column, Column> HeaderRenameDictionary
			{
				get
				{
					return this.headerRenameDictionary;
				}
			}

			internal IReadOnlyDictionary<Column, Column> LeafIndexRenameDictionary
			{
				get
				{
					return this.leafIndexRenameDictionary;
				}
			}

			internal IReadOnlyDictionary<Column, Column> LeafMessageRenameDictionary
			{
				get
				{
					return this.leafMessageRenameDictionary;
				}
			}

			internal KeyRange HeaderKeyRange
			{
				get
				{
					return this.headerKeyRange;
				}
			}

			internal KeyRange LeafKeyRange
			{
				get
				{
					return this.leafKeyRange;
				}
			}

			internal bool IsInclusiveStartKey
			{
				get
				{
					return this.isInclusiveStartKey;
				}
			}

			public override SortOrder SortOrder
			{
				get
				{
					return this.HeaderTable.PrimaryKeyIndex.SortOrder;
				}
			}

			internal SortOrder HeaderLogicalSortOrder
			{
				get
				{
					return this.categorizedTableParams.HeaderLogicalSortOrder;
				}
			}

			internal SortOrder LeafLogicalSortOrder
			{
				get
				{
					return this.categorizedTableParams.LeafLogicalSortOrder;
				}
			}

			internal IList<object> HeaderKeyPrefix
			{
				get
				{
					return this.categorizedTableParams.HeaderKeyPrefix;
				}
			}

			internal IList<object> LeafKeyPrefix
			{
				get
				{
					return this.categorizedTableParams.LeafKeyPrefix;
				}
			}

			internal IList<StorePropTag> HeaderOnlyPropTags
			{
				get
				{
					return this.categorizedTableParams.HeaderOnlyPropTags;
				}
			}

			internal Column DepthColumn
			{
				get
				{
					return this.categorizedTableParams.DepthColumn;
				}
			}

			internal Column CategIdColumn
			{
				get
				{
					return this.categorizedTableParams.CategIdColumn;
				}
			}

			internal Column RowTypeColumn
			{
				get
				{
					return this.categorizedTableParams.RowTypeColumn;
				}
			}

			public override SimpleQueryOperator CreateOperator(IConnectionProvider connectionProvider)
			{
				return Factory.CreateCategorizedTableOperator(connectionProvider, this);
			}

			public override void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction)
			{
				operatorDefinitionAction(this);
			}

			private void ConfigureHeaderAndLeafColumnsToFetchAndRenameDictionaries(Table table, IList<Column> columnsToFetch, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary)
			{
				List<Column> list3 = new List<Column>(table.PrimaryKeyIndex.Columns);
				int num = (columnsToFetch != null) ? columnsToFetch.Count : 0;
				this.headerColumnsToFetch = new List<Column>(num);
				list3.Capacity = table.PrimaryKeyIndex.ColumnCount + num;
				this.leafColumnsToFetch = list3;
				this.headerRenameDictionary = new Dictionary<Column, Column>(categorizedTableParams.HeaderRenameDictionary.Count + additionalHeaderRenameDictionary.Count + 1 + columnsToFetch.Count + categorizedTableParams.LeafLogicalSortOrder.Columns.Count);
				foreach (KeyValuePair<Column, Column> keyValuePair in categorizedTableParams.HeaderRenameDictionary)
				{
					this.headerRenameDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
				foreach (KeyValuePair<Column, Column> keyValuePair2 in additionalHeaderRenameDictionary)
				{
					this.headerRenameDictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
				this.headerRenameDictionary.Add(this.RowTypeColumn, Factory.CreateFunctionColumn("HeaderRowType", typeof(int), 4, 0, this.HeaderTable, (object[] columnValues) => collapseState.IsHeaderExpanded((int)columnValues[0], (long)columnValues[1]) ? 3 : 4, "ComputeHeaderRowType", new Column[]
				{
					this.DepthColumn,
					this.CategIdColumn
				}));
				this.leafMessageRenameDictionary = new Dictionary<Column, Column>(additionalLeafRenameDictionary.Count + 3);
				foreach (KeyValuePair<Column, Column> keyValuePair3 in additionalLeafRenameDictionary)
				{
					this.leafMessageRenameDictionary.Add(keyValuePair3.Key, keyValuePair3.Value);
				}
				this.leafMessageRenameDictionary.Add(this.CategIdColumn, Factory.CreateConstantColumn(null, this.CategIdColumn));
				this.leafMessageRenameDictionary.Add(this.DepthColumn, Factory.CreateConstantColumn(this.CategoryCount, this.DepthColumn));
				this.leafMessageRenameDictionary.Add(this.RowTypeColumn, Factory.CreateConstantColumn(1, this.RowTypeColumn));
				this.leafIndexRenameDictionary = new Dictionary<Column, Column>(categorizedTableParams.LeafRenameDictionary.Count + this.leafMessageRenameDictionary.Count);
				foreach (KeyValuePair<Column, Column> keyValuePair4 in categorizedTableParams.LeafRenameDictionary)
				{
					this.leafIndexRenameDictionary.Add(keyValuePair4.Key, keyValuePair4.Value);
				}
				foreach (KeyValuePair<Column, Column> keyValuePair5 in this.leafMessageRenameDictionary)
				{
					this.leafIndexRenameDictionary.Add(keyValuePair5.Key, keyValuePair5.Value);
				}
				foreach (Column column in columnsToFetch)
				{
					this.ValidateColumnToFetch(column);
				}
				if (restriction != null)
				{
					List<Column> list2 = new List<Column>();
					restriction.EnumerateColumns(delegate(Column c, object list)
					{
						((List<Column>)list).Add(c);
					}, list2, true);
					foreach (Column column2 in list2)
					{
						this.ValidateColumnToFetch(column2);
					}
				}
				for (int i = 0; i < categorizedTableParams.LeafLogicalSortOrder.Columns.Count; i++)
				{
					Column column3 = categorizedTableParams.LeafLogicalSortOrder.Columns[i];
					if (!this.leafColumnsToFetch.Contains(column3))
					{
						this.leafColumnsToFetch.Add(column3);
					}
					if (i >= categorizedTableParams.CategoryCount && !this.headerRenameDictionary.ContainsKey(column3))
					{
						this.headerRenameDictionary[column3] = Factory.CreateConstantColumn(null, column3);
					}
				}
				foreach (Column column4 in categorizedTableParams.HeaderLogicalSortOrder.Columns)
				{
					if (!this.headerColumnsToFetch.Contains(column4))
					{
						this.headerColumnsToFetch.Add(column4);
					}
					if (!this.leafColumnsToFetch.Contains(column4))
					{
						ExtendedPropertyColumn extendedPropertyColumn = column4 as ExtendedPropertyColumn;
						if (extendedPropertyColumn == null || categorizedTableParams.HeaderOnlyPropTags == null || !categorizedTableParams.HeaderOnlyPropTags.Contains(extendedPropertyColumn.StorePropTag))
						{
							this.leafColumnsToFetch.Add(column4);
						}
					}
				}
				if (!this.headerColumnsToFetch.Contains(categorizedTableParams.CategIdColumn))
				{
					this.headerColumnsToFetch.Add(categorizedTableParams.CategIdColumn);
				}
				if (!this.leafColumnsToFetch.Contains(categorizedTableParams.CategIdColumn))
				{
					this.leafColumnsToFetch.Add(categorizedTableParams.CategIdColumn);
				}
				if (this.atLeastOneExternalColumn)
				{
					this.leafColumnsToFetchWithoutJoin = new List<Column>();
					for (int j = 0; j < table.PrimaryKeyIndex.Columns.Count; j++)
					{
						Column item = this.leafColumnsToFetch[j];
						this.leafColumnsToFetchWithoutJoin.Add(item);
					}
					for (int k = table.PrimaryKeyIndex.Columns.Count; k < this.leafColumnsToFetch.Count; k++)
					{
						Column column5 = this.leafColumnsToFetch[k];
						Column column6;
						if (this.leafIndexRenameDictionary.TryGetValue(column5, out column6) && column6.MaxLength >= column5.MaxLength)
						{
							this.leafColumnsToFetchWithoutJoin.Add(column5);
						}
					}
					return;
				}
				this.leafColumnsToFetchWithoutJoin = this.leafColumnsToFetch;
			}

			private void ValidateColumnToFetch(Column column)
			{
				if (!this.headerColumnsToFetch.Contains(column))
				{
					this.headerColumnsToFetch.Add(column);
				}
				bool flag = false;
				if (this.categorizedTableParams.HeaderOnlyPropTags != null)
				{
					ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
					if (extendedPropertyColumn != null && this.categorizedTableParams.HeaderOnlyPropTags.Contains(extendedPropertyColumn.StorePropTag))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (!this.leafColumnsToFetch.Contains(column))
					{
						this.leafColumnsToFetch.Add(column);
					}
					Column actualColumn = column.ActualColumn;
					ConversionColumn conversionColumn = actualColumn as ConversionColumn;
					if (conversionColumn != null)
					{
						column = conversionColumn.ArgumentColumn;
					}
					if (!this.headerRenameDictionary.ContainsKey(column) && !(column is ConstantColumn))
					{
						this.headerRenameDictionary[column] = Factory.CreateConstantColumn(null, column);
						Column column2;
						if (!this.atLeastOneExternalColumn && (!this.leafIndexRenameDictionary.TryGetValue(column, out column2) || column2.MaxLength < column.MaxLength))
						{
							this.atLeastOneExternalColumn = true;
						}
					}
				}
			}

			private void ComputeHeaderAndLeafKeyRanges(KeyRange keyRange, CategorizedTableParams categorizedTableParams, bool backwards)
			{
				Index primaryKeyIndex = categorizedTableParams.HeaderTable.PrimaryKeyIndex;
				Index primaryKeyIndex2 = categorizedTableParams.LeafTable.PrimaryKeyIndex;
				if (categorizedTableParams.HeaderKeyPrefix.Count >= primaryKeyIndex.ColumnCount || categorizedTableParams.LeafKeyPrefix.Count >= primaryKeyIndex2.ColumnCount)
				{
					throw new StoreException((LID)40208U, ErrorCodeValue.InvalidBookmark, "Key prefix must be a subset of the full primary key.");
				}
				int num = primaryKeyIndex.ColumnCount - categorizedTableParams.HeaderKeyPrefix.Count;
				int num2 = primaryKeyIndex2.ColumnCount - categorizedTableParams.LeafKeyPrefix.Count;
				List<object> list = new List<object>(categorizedTableParams.HeaderKeyPrefix);
				List<object> list2 = new List<object>(categorizedTableParams.HeaderKeyPrefix);
				List<object> list3 = new List<object>(categorizedTableParams.LeafKeyPrefix);
				List<object> values = new List<object>(categorizedTableParams.LeafKeyPrefix);
				bool inclusive = keyRange.StartKey.Inclusive;
				if (!keyRange.StartKey.IsEmpty)
				{
					if (keyRange.StartKey.Count != num + num2)
					{
						throw new StoreException((LID)49872U, ErrorCodeValue.InvalidBookmark, "The start key must be the concatenation of a header table key and a leaf table key.");
					}
					for (int i = 0; i < num; i++)
					{
						object obj = keyRange.StartKey.Values[i];
						Column column = primaryKeyIndex.Columns[categorizedTableParams.HeaderKeyPrefix.Count + i];
						bool flag;
						obj = ValueHelper.TruncateValueIfNecessary(obj, column.Type, column.MaxLength, out flag);
						list.Add(obj);
					}
					int index = categorizedTableParams.HeaderKeyPrefix.Count + num - 1;
					int? num3 = list[index] as int?;
					if (num3 == null || num3 < 0 || num3 > categorizedTableParams.CategoryCount)
					{
						throw new StoreException((LID)48400U, ErrorCodeValue.InvalidBookmark, "Invalid key value for Depth column.");
					}
					if (num3 == categorizedTableParams.CategoryCount)
					{
						inclusive = true;
						for (int j = num; j < keyRange.StartKey.Count; j++)
						{
							object obj2 = keyRange.StartKey.Values[j];
							Column column2 = primaryKeyIndex2.Columns[categorizedTableParams.LeafKeyPrefix.Count + j - num];
							bool flag2;
							obj2 = ValueHelper.TruncateValueIfNecessary(obj2, column2.Type, column2.MaxLength, out flag2);
							list3.Add(obj2);
						}
						list[index] = num3 - 1;
					}
					else if (num3 == categorizedTableParams.CategoryCount - 1 && !backwards)
					{
						inclusive = true;
					}
				}
				if (!keyRange.StopKey.IsEmpty)
				{
					if (keyRange.StopKey.Count != num + num2)
					{
						throw new StoreException((LID)64784U, ErrorCodeValue.InvalidBookmark, "The stop key must be the concatenation of a header table key and a leaf table key.");
					}
					for (int k = 0; k < num; k++)
					{
						object obj3 = keyRange.StopKey.Values[k];
						Column column3 = primaryKeyIndex.Columns[categorizedTableParams.HeaderKeyPrefix.Count + k];
						bool flag3;
						obj3 = ValueHelper.TruncateValueIfNecessary(obj3, column3.Type, column3.MaxLength, out flag3);
						list2.Add(obj3);
					}
				}
				this.headerKeyRange = new KeyRange(new StartStopKey(inclusive, list), new StartStopKey(keyRange.StopKey.Inclusive, list2));
				this.leafKeyRange = new KeyRange(new StartStopKey(keyRange.StartKey.Inclusive, list3), new StartStopKey(true, values));
			}

			internal override void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
			{
				sb.Append("select");
				bool flag = (formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails;
				bool multiLine = (formatOptions & StringFormatOptions.MultiLine) == StringFormatOptions.MultiLine;
				sb.Append(" from Header ");
				sb.Append(this.HeaderTable.Name);
				sb.Append(" and Leaf ");
				sb.Append(this.LeafTable.Name);
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  categoryCount:[");
				sb.Append(this.CategoryCount);
				sb.Append("]");
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  levelsInitiallyExpanded:[");
				sb.Append(this.CollapseState.LevelsInitiallyExpanded);
				sb.Append("]");
				if (base.ColumnsToFetch != null)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  fetch:[");
					DataAccessOperator.DataAccessOperatorDefinition.AppendColumnsSummaryToStringBuilder(sb, base.ColumnsToFetch, null, formatOptions);
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
					sb.Append(base.MaxRows);
					sb.Append("]");
				}
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  headerLogicalSortOrder(" + this.HeaderLogicalSortOrder.Count + "):[");
				this.HeaderLogicalSortOrder.AppendToStringBuilder(sb, formatOptions);
				sb.Append("]");
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  leafLogicalSortOrder(" + this.LeafLogicalSortOrder.Count + "):[");
				this.LeafLogicalSortOrder.AppendToStringBuilder(sb, formatOptions);
				sb.Append("]");
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  headerKeyPrefix(" + this.HeaderKeyPrefix.Count + "):[");
				StartStopKey.AppendKeyValuesToStringBuilder(sb, formatOptions, this.HeaderKeyPrefix);
				sb.Append("]");
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  leafKeyPrefix(" + this.LeafKeyPrefix.Count + "):[");
				StartStopKey.AppendKeyValuesToStringBuilder(sb, formatOptions, this.LeafKeyPrefix);
				sb.Append("]");
				if (!this.HeaderKeyRange.IsAllRows)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  headerKeyRange:[");
					this.HeaderKeyRange.AppendToStringBuilder(sb, formatOptions);
					sb.Append("]");
				}
				if (!this.LeafKeyRange.IsAllRows)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  leafKeyRange:[");
					this.LeafKeyRange.AppendToStringBuilder(sb, formatOptions);
					sb.Append("]");
				}
				base.Indent(sb, multiLine, nestingLevel, false);
				sb.Append("  atLeastOneExternalColumn:[");
				sb.Append(this.AtLeastOneExternalColumn);
				sb.Append("]");
				if (flag || this.Backwards)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  backwards:[");
					sb.Append(this.Backwards);
					sb.Append("]");
				}
				if (flag)
				{
					base.Indent(sb, multiLine, nestingLevel, false);
					sb.Append("  frequentOp:[");
					sb.Append(base.FrequentOperation);
					sb.Append("]");
				}
			}

			private readonly CategorizedTableParams categorizedTableParams;

			private readonly CategorizedTableCollapseState collapseState;

			private readonly KeyRange keyRange;

			private readonly bool isInclusiveStartKey;

			private readonly bool backwards;

			private IList<Column> headerColumnsToFetch;

			private IList<Column> leafColumnsToFetch;

			private IList<Column> leafColumnsToFetchWithoutJoin;

			private bool atLeastOneExternalColumn;

			private Dictionary<Column, Column> headerRenameDictionary;

			private Dictionary<Column, Column> leafIndexRenameDictionary;

			private Dictionary<Column, Column> leafMessageRenameDictionary;

			private KeyRange headerKeyRange;

			private KeyRange leafKeyRange;
		}

		private class CategorizedTableReader : Reader
		{
			public CategorizedTableReader(Connection connection, CategorizedTableOperator categorizedTableOperator, bool disposeOperator) : base(connection, categorizedTableOperator, disposeOperator)
			{
				this.isReaderOpen = true;
			}

			private CategorizedTableOperator CategorizedTableOperator
			{
				get
				{
					return (CategorizedTableOperator)base.SimpleQueryOperator;
				}
			}

			private Reader CurrentReader(Column columnToRetrieve)
			{
				return this.CategorizedTableOperator.CurrentReader(columnToRetrieve);
			}

			public override bool IsClosed
			{
				get
				{
					return !this.isReaderOpen;
				}
			}

			public override bool Read(out int rowsSkipped)
			{
				rowsSkipped = 0;
				bool result;
				if (this.movedToFirst)
				{
					result = this.CategorizedTableOperator.MoveNext();
				}
				else
				{
					result = this.CategorizedTableOperator.MoveFirst(out rowsSkipped);
					this.movedToFirst = true;
				}
				return result;
			}

			public override bool? GetNullableBoolean(Column column)
			{
				return this.CurrentReader(column).GetNullableBoolean(column);
			}

			public override bool GetBoolean(Column column)
			{
				return this.CurrentReader(column).GetBoolean(column);
			}

			public override long? GetNullableInt64(Column column)
			{
				return this.CurrentReader(column).GetNullableInt64(column);
			}

			public override long GetInt64(Column column)
			{
				return this.CurrentReader(column).GetInt64(column);
			}

			public override int? GetNullableInt32(Column column)
			{
				return this.CurrentReader(column).GetNullableInt32(column);
			}

			public override int GetInt32(Column column)
			{
				return this.CurrentReader(column).GetInt32(column);
			}

			public override short? GetNullableInt16(Column column)
			{
				return this.CurrentReader(column).GetNullableInt16(column);
			}

			public override short GetInt16(Column column)
			{
				return this.CurrentReader(column).GetInt16(column);
			}

			public override Guid? GetNullableGuid(Column column)
			{
				return this.CurrentReader(column).GetNullableGuid(column);
			}

			public override Guid GetGuid(Column column)
			{
				return this.CurrentReader(column).GetGuid(column);
			}

			public override DateTime? GetNullableDateTime(Column column)
			{
				return this.CurrentReader(column).GetNullableDateTime(column);
			}

			public override DateTime GetDateTime(Column column)
			{
				return this.CurrentReader(column).GetDateTime(column);
			}

			public override byte[] GetBinary(Column column)
			{
				return this.CurrentReader(column).GetBinary(column);
			}

			public override string GetString(Column column)
			{
				return this.CurrentReader(column).GetString(column);
			}

			public override object GetValue(Column column)
			{
				return this.CurrentReader(column).GetValue(column);
			}

			public override long GetChars(Column column, long dataIndex, char[] outBuffer, int bufferIndex, int length)
			{
				return this.CurrentReader(column).GetChars(column, dataIndex, outBuffer, bufferIndex, length);
			}

			public override long GetBytes(Column column, long dataIndex, byte[] outBuffer, int bufferIndex, int length)
			{
				return this.CurrentReader(column).GetBytes(column, dataIndex, outBuffer, bufferIndex, length);
			}

			public override void Close()
			{
				this.isReaderOpen = false;
				base.Close();
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<CategorizedTableOperator.CategorizedTableReader>(this);
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose)
				{
					this.Close();
					if (base.DisposeQueryOperator && base.SimpleQueryOperator != null)
					{
						base.SimpleQueryOperator.Dispose();
					}
				}
			}

			private bool movedToFirst;

			private bool isReaderOpen;
		}

		private class HeaderOrLeafReader : ITWIR
		{
			public HeaderOrLeafReader(Reader reader)
			{
				this.reader = reader;
			}

			public Reader InternalReader
			{
				get
				{
					return this.reader;
				}
			}

			public void CloseReader()
			{
				this.reader.Dispose();
				this.reader = null;
			}

			int ITWIR.GetColumnSize(Column column)
			{
				return ((IColumn)column).GetSize(this);
			}

			object ITWIR.GetColumnValue(Column column)
			{
				return this.reader.GetValue(column);
			}

			int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
			{
				object value = this.reader.GetValue(column);
				return SizeOfColumn.GetColumnSize(column, value).GetValueOrDefault();
			}

			object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
			{
				return this.reader.GetValue(column);
			}

			int ITWIR.GetPropertyColumnSize(PropertyColumn column)
			{
				object value = this.reader.GetValue(column);
				return SizeOfColumn.GetColumnSize(column, value).GetValueOrDefault();
			}

			object ITWIR.GetPropertyColumnValue(PropertyColumn column)
			{
				return this.reader.GetValue(column);
			}

			private Reader reader;
		}
	}
}
