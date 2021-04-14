using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class ViewTable
	{
		public ViewTable(Mailbox mailbox, Table table)
		{
			this.mailbox = mailbox;
			this.table = table;
			this.bookmark = Bookmark.BOT;
		}

		public ViewTable(Mailbox mailbox, TableFunction tableFunction, object[] tableFunctionParameters)
		{
			this.mailbox = mailbox;
			this.table = tableFunction;
			this.bookmark = Bookmark.BOT;
			this.tableFunctionParameters = tableFunctionParameters;
		}

		public Mailbox Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public Bookmark Bookmark
		{
			get
			{
				return this.bookmark;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		public virtual ViewTable QueryRowsViewTable
		{
			get
			{
				return this;
			}
		}

		public int CategoryCount
		{
			get
			{
				return this.categoryCount;
			}
		}

		public CategorizedTableCollapseState CollapseState
		{
			get
			{
				return this.collapseState;
			}
		}

		public SortOrder CategoryHeadersSortOrder
		{
			get
			{
				return this.categoryHeadersSortOrder;
			}
		}

		public CategoryHeaderSortOverride[] CategoryHeaderSortOverrides
		{
			get
			{
				return this.categoryHeaderSortOverrides;
			}
		}

		public bool IsCategorizedView
		{
			get
			{
				return this.categoryCount > 0;
			}
		}

		public virtual IList<Column> LongValueColumnsToPreread
		{
			get
			{
				return null;
			}
		}

		internal IList<Column> ViewColumns
		{
			get
			{
				return this.viewColumns;
			}
		}

		internal SearchCriteria ImplicitCriteria
		{
			get
			{
				return this.implicitCriteria;
			}
		}

		internal SearchCriteria RestrictCriteria
		{
			get
			{
				return this.restrictCriteria;
			}
			set
			{
				this.restrictCriteria = value;
			}
		}

		protected bool RowCountValid
		{
			get
			{
				return this.rowCountValid;
			}
		}

		protected int RowCount
		{
			get
			{
				return this.rowCount;
			}
		}

		protected virtual Index LogicalKeyIndex
		{
			get
			{
				return this.table.PrimaryKeyIndex;
			}
		}

		protected virtual Dictionary<Column, FilterFactorHint> FilterFactorHints
		{
			get
			{
				return null;
			}
		}

		protected bool MvExplosion
		{
			get
			{
				return this.multiValueInstanceColumn != null;
			}
		}

		protected virtual bool MustUseLazyIndex
		{
			get
			{
				return false;
			}
		}

		public static bool ClientTypeExcludedFromDefaultPromotedValidation(ClientType clientType)
		{
			return clientType == ClientType.MoMT || clientType == ClientType.WebServices || clientType == ClientType.Pop || clientType == ClientType.Imap || clientType == ClientType.AirSync || clientType == ClientType.User || clientType == ClientType.LoadGen;
		}

		public virtual void Reset()
		{
			this.viewColumns = null;
			this.restrictCriteria = null;
			this.sortOrder = this.LogicalKeyIndex.SortOrder;
			this.categoryCount = 0;
			this.collapseState = null;
			this.categoryHeadersSortOrder = SortOrder.Empty;
			this.categoryHeaderSortOverrides = null;
			this.InvalidateBookmarkAndRowCount();
		}

		public void SetColumns(Context context, IList<Column> columns)
		{
			this.SetColumns(context, columns, ViewSetColumnsFlag.None);
		}

		public virtual void SetColumns(Context context, IList<Column> columns, ViewSetColumnsFlag flags)
		{
			this.viewColumns = columns;
		}

		public virtual void Restrict(Context context, SearchCriteria restrictCriteria)
		{
			if (restrictCriteria == Factory.CreateSearchCriteriaTrue())
			{
				this.restrictCriteria = null;
			}
			else if (this.IsCategorizedView && restrictCriteria != null)
			{
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel.ExTraceGlobals.CategorizationsTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel.ExTraceGlobals.CategorizationsTracer.TraceDebug<int>(0L, "Mailbox {0}: Don't currently support restricting a categorized view, so the restriction request will be ignored.", this.Mailbox.MailboxNumber);
				}
				this.restrictCriteria = null;
			}
			else
			{
				this.restrictCriteria = this.RewriteSearchCriteria(context, restrictCriteria);
			}
			this.InvalidateBookmarkAndRowCount();
		}

		public virtual void SortTable(SortOrder sortOrder)
		{
			SortOrderBuilder sortOrderBuilder = new SortOrderBuilder(sortOrder);
			bool ascending = sortOrder.Count == 0 || sortOrder[0].Ascending;
			if (this.LogicalKeyIndex != null)
			{
				IList<Column> columns = this.LogicalKeyIndex.Columns;
				for (int i = 0; i < columns.Count; i++)
				{
					Column column = columns[i];
					if (!sortOrderBuilder.Contains(column))
					{
						sortOrderBuilder.Add(column, ascending);
					}
				}
			}
			Column col = null;
			foreach (SortColumn sortColumn in ((IEnumerable<SortColumn>)sortOrderBuilder))
			{
				if (PropertySchema.IsMultiValueInstanceColumn(sortColumn.Column))
				{
					col = (sortColumn.Column as ExtendedPropertyColumn);
					Column column2 = PropertySchema.MapToColumn(this.Mailbox.Database, ObjectType.Message, PropTag.Message.InstanceNum);
					if (!sortOrderBuilder.Contains(column2))
					{
						sortOrderBuilder.Add(column2, ascending);
						break;
					}
					break;
				}
			}
			this.Uncategorize();
			this.sortOrder = sortOrderBuilder.ToSortOrder();
			if (col != this.multiValueInstanceColumn)
			{
				this.rowCountValid = false;
				this.multiValueInstanceColumn = col;
			}
			this.bookmark = Bookmark.BOT;
		}

		public virtual void Categorize(Context context, int categoryCount, int expandedCount, CategoryHeaderSortOverride[] categoryHeaderSortOverrides)
		{
			throw new StoreException((LID)50872U, ErrorCodeValue.NotSupported, "This view type does not support categorized views.");
		}

		public virtual int CollapseRow(Context context, ExchangeId categoryId)
		{
			throw new StoreException((LID)41759U, ErrorCodeValue.Leaf, "This view type only contains leaf rows, so there are no header rows to collapse.");
		}

		public virtual int ExpandRow(Context context, ExchangeId categoryId)
		{
			throw new StoreException((LID)58143U, ErrorCodeValue.Leaf, "This view type only contains leaf rows, so there are no header rows to expand.");
		}

		public virtual byte[] GetCollapseState(Context context, ExchangeId rowInstanceId, int rowInstanceNumber)
		{
			throw new StoreException((LID)40832U, ErrorCodeValue.Leaf, "This view type only contains leaf rows.");
		}

		public virtual byte[] SetCollapseState(Context context, byte[] collapseState)
		{
			throw new StoreException((LID)57216U, ErrorCodeValue.Leaf, "This view type only contains leaf rows.");
		}

		public byte[] CreateExternalBookmark()
		{
			return ViewTable.SerializeBookmark(this.GetSortOrderForView(), this.bookmark);
		}

		public void FreeExternalBookmark(byte[] bookmark)
		{
		}

		public int GetPosition(Context context)
		{
			if (!this.bookmark.PositionValid)
			{
				if (this.bookmark.IsEOT)
				{
					return this.GetRowCount(context);
				}
				if (this.IsViewEmpty(context))
				{
					return 0;
				}
				using (OrdinalPositionOperator ordinalPositionOperator = this.GetOrdinalPositionOperator(context))
				{
					int position = (int)ordinalPositionOperator.ExecuteScalar();
					this.bookmark = new Bookmark(this.bookmark.KeyValues, this.bookmark.PositionedOn, position);
				}
			}
			return this.bookmark.Position;
		}

		public int? GetCachedPosition()
		{
			if (!this.bookmark.PositionValid)
			{
				return null;
			}
			return new int?(this.bookmark.Position);
		}

		public void SeekRow(Context context, ViewSeekOrigin origin, int numberOfRows)
		{
			bool flag;
			int num;
			bool flag2;
			this.SeekRow(context, origin, null, numberOfRows, false, out flag, out num, false, out flag2);
		}

		public virtual void SeekRow(Context context, ViewSeekOrigin origin, byte[] bookmark, int numberOfRows, bool wantSoughtRowCount, out bool soughtLessThanRowCount, out int rowCountActuallySought, bool validateBookmarkPosition, out bool bookmarkPositionChanged)
		{
			soughtLessThanRowCount = false;
			rowCountActuallySought = 0;
			bookmarkPositionChanged = false;
			int num = numberOfRows;
			Bookmark bookmark2;
			if (origin == ViewSeekOrigin.Beginning)
			{
				bookmark2 = Bookmark.BOT;
				if (num > 0 && this.bookmark.PositionValid)
				{
					int num2 = num - this.bookmark.Position;
					int num3 = (num2 >= 0) ? num2 : (-num2);
					if (num3 < num && this.optimizedSeeks++ % 32 != 0)
					{
						bookmark2 = this.bookmark;
						num = num2;
					}
				}
			}
			else if (origin == ViewSeekOrigin.End)
			{
				bookmark2 = Bookmark.EOT;
			}
			else if (origin == ViewSeekOrigin.Current)
			{
				bookmark2 = this.bookmark;
			}
			else
			{
				if (origin != ViewSeekOrigin.Bookmark)
				{
					throw new NotSupportedException("SeekRow does not support this origin");
				}
				bookmark2 = ViewTable.DeserializeBookmark(this.GetSortOrderForView(), bookmark);
			}
			if (num == -2147483648)
			{
				num++;
			}
			if (bookmark2.IsBOT)
			{
				if (num <= 0)
				{
					this.bookmark = Bookmark.BOT;
					if (num < 0)
					{
						soughtLessThanRowCount = true;
						rowCountActuallySought = 0;
						return;
					}
				}
				else
				{
					this.DoSeekQuery(context, bookmark2, false, num, new int?(num), out soughtLessThanRowCount, out rowCountActuallySought);
					if (soughtLessThanRowCount)
					{
						this.rowCount = rowCountActuallySought;
						this.rowCountValid = true;
						return;
					}
				}
			}
			else if (bookmark2.IsEOT)
			{
				if (num < 0)
				{
					int? newPosition = null;
					if (this.rowCountValid)
					{
						newPosition = new int?(this.rowCount + num);
					}
					this.DoSeekQuery(context, bookmark2, true, -num, newPosition, out soughtLessThanRowCount, out rowCountActuallySought);
					if (soughtLessThanRowCount)
					{
						this.rowCount = rowCountActuallySought;
						this.rowCountValid = true;
					}
					rowCountActuallySought = -rowCountActuallySought;
					return;
				}
				this.bookmark = Bookmark.EOT;
				if (num > 0)
				{
					soughtLessThanRowCount = true;
					rowCountActuallySought = 0;
					return;
				}
			}
			else
			{
				if (validateBookmarkPosition)
				{
					bookmarkPositionChanged = !this.CheckBookmarkPosition(context, bookmark2);
				}
				if (num == 0)
				{
					this.bookmark = bookmark2;
				}
				else if (bookmark2.PositionedOn && num == 1)
				{
					if (bookmark2.PositionValid)
					{
						this.bookmark = new Bookmark(bookmark2.KeyValues, false, bookmark2.Position + 1);
					}
					else
					{
						this.bookmark = new Bookmark(bookmark2.KeyValues, false);
					}
					rowCountActuallySought = 1;
				}
				else if (!bookmark2.PositionedOn && num == -1)
				{
					if (bookmark2.PositionValid)
					{
						this.bookmark = new Bookmark(bookmark2.KeyValues, true, bookmark2.Position - 1);
					}
					else
					{
						this.bookmark = new Bookmark(bookmark2.KeyValues, true);
					}
					rowCountActuallySought = -1;
				}
				else
				{
					bool flag = num < 0;
					int rowsToSeek = flag ? (-num) : num;
					int? newPosition2 = null;
					if (bookmark2.PositionValid)
					{
						newPosition2 = new int?(bookmark2.Position + num);
					}
					this.DoSeekQuery(context, bookmark2, flag, rowsToSeek, newPosition2, out soughtLessThanRowCount, out rowCountActuallySought);
					if (flag)
					{
						rowCountActuallySought = -rowCountActuallySought;
					}
				}
				if (origin == ViewSeekOrigin.Beginning && wantSoughtRowCount)
				{
					if (soughtLessThanRowCount)
					{
						this.DoSeekQuery(context, Bookmark.BOT, false, numberOfRows, new int?(numberOfRows), out soughtLessThanRowCount, out rowCountActuallySought);
						if (soughtLessThanRowCount)
						{
							this.rowCount = rowCountActuallySought;
							this.rowCountValid = true;
							return;
						}
					}
					else
					{
						rowCountActuallySought = numberOfRows;
					}
				}
			}
		}

		public virtual Reader QueryRows(Context context, int rowCount, bool backwards)
		{
			SimpleQueryOperator simpleQueryOperator = this.GetQueryRowsOperator(context, rowCount, backwards);
			if (simpleQueryOperator == null)
			{
				return null;
			}
			Reader result;
			try
			{
				Reader reader = simpleQueryOperator.ExecuteReader(true);
				simpleQueryOperator = null;
				result = reader;
			}
			finally
			{
				if (simpleQueryOperator != null)
				{
					simpleQueryOperator.Dispose();
				}
			}
			return result;
		}

		public bool FindRow(Context context, SearchCriteria criteria, ViewSeekOrigin origin, bool backwards)
		{
			bool flag;
			bool result;
			using (Reader reader = this.FindRow(context, criteria, origin, null, backwards, out flag))
			{
				result = (reader != null);
			}
			return result;
		}

		public virtual Reader FindRow(Context context, SearchCriteria criteria, ViewSeekOrigin origin, byte[] bookmark, bool backwards, out bool bookmarkPositionChanged)
		{
			bookmarkPositionChanged = false;
			Bookmark bookmark2;
			switch (origin)
			{
			case ViewSeekOrigin.Beginning:
				bookmark2 = Bookmark.BOT;
				break;
			case ViewSeekOrigin.Current:
				bookmark2 = this.bookmark;
				break;
			case ViewSeekOrigin.End:
				bookmark2 = Bookmark.EOT;
				break;
			case ViewSeekOrigin.Bookmark:
				bookmark2 = ViewTable.DeserializeBookmark(this.GetSortOrderForView(), bookmark);
				break;
			default:
				throw new NotSupportedException("bookmark origin is not supported");
			}
			if (!bookmark2.IsBOT && !bookmark2.IsEOT)
			{
				bookmarkPositionChanged = !this.CheckBookmarkPosition(context, bookmark2);
			}
			if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.TraceDebug(0L, "{0} FindRow:  view:[{1}]  categoryCount:{2}  implicitCriteria:[{3}]  restrictCriteria:[{4}]  findRowCriteria:[{5}]  startBookmark:[{6}], backwards:{7}", new object[]
				{
					base.GetType().Name,
					this.GetSortOrderForView(),
					this.CategoryCount,
					this.implicitCriteria,
					this.restrictCriteria,
					criteria,
					bookmark2,
					backwards
				});
			}
			return this.FindRow(context, criteria, bookmark2, backwards);
		}

		public virtual int GetRowCount(Context context)
		{
			if (!this.rowCountValid)
			{
				this.rowCount = 0;
				if (!this.IsViewEmpty(context))
				{
					using (CountOperator countOperator = this.GetCountOperator(context))
					{
						this.rowCount = (int)countOperator.ExecuteScalar();
					}
				}
				this.rowCountValid = true;
			}
			return this.rowCount;
		}

		public virtual bool NeedIndexForPositionOrRowCount(Context context)
		{
			return false;
		}

		public virtual IChunked PrepareIndexes(Context context, SearchCriteria findRowCriteria)
		{
			return null;
		}

		public virtual void ForceReload(Context context, bool invalidateRowCount)
		{
			if (invalidateRowCount)
			{
				this.rowCountValid = false;
			}
			if (this.bookmark.PositionValid && !this.bookmark.IsBOT)
			{
				this.bookmark = new Bookmark(this.bookmark.KeyValues, this.bookmark.PositionedOn);
			}
		}

		public void BookmarkCurrentRow(Reader reader, bool positionedOn)
		{
			this.bookmark = new Bookmark(this.GetSortOrderForView(), reader, positionedOn, null);
		}

		public void BookmarkCurrentRow(Reader reader, bool positionedOn, int? position)
		{
			this.bookmark = new Bookmark(this.GetSortOrderForView(), reader, positionedOn, position);
		}

		public void SaveLastBookmark()
		{
			this.lastBookmark = this.bookmark;
		}

		public void RevertToLastBookmark()
		{
			this.bookmark = this.lastBookmark;
		}

		public bool CheckBookmarkMatchesRow(Context context, Reader reader, Bookmark expectedBookmark, bool checkHeaderBookmarkOnly)
		{
			CompareInfo compareInfo = (context.Culture == null) ? null : context.Culture.CompareInfo;
			Bookmark bookmark = new Bookmark(this.GetSortOrderForView(), reader, expectedBookmark.PositionedOn, null);
			bool result = false;
			if (checkHeaderBookmarkOnly && this.IsCategorizedView)
			{
				if (expectedBookmark.KeyValues.Count == bookmark.KeyValues.Count)
				{
					result = true;
					for (int i = 0; i < this.CategoryHeadersSortOrder.Count; i++)
					{
						if (!ValueHelper.ValuesEqual(expectedBookmark.KeyValues[i], bookmark.KeyValues[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
						{
							result = false;
							break;
						}
					}
				}
			}
			else
			{
				result = ValueHelper.ListsEqual(expectedBookmark.KeyValues, bookmark.KeyValues, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
			}
			return result;
		}

		public bool IsHeaderRowBookmark(Bookmark bookmark)
		{
			bool result = false;
			if (this.IsCategorizedView)
			{
				int num = (int)bookmark.KeyValues[this.categoryHeadersSortOrder.Count - 1];
				if (num < this.CategoryCount)
				{
					result = true;
				}
			}
			return result;
		}

		public bool IsLeafRowBookmark(Bookmark bookmark)
		{
			bool result = false;
			if (this.IsCategorizedView)
			{
				int num = (int)bookmark.KeyValues[this.categoryHeadersSortOrder.Count - 1];
				if (num == this.CategoryCount)
				{
					result = true;
				}
			}
			return result;
		}

		internal static IDisposable SetFindRowTestHook(Action action)
		{
			return ViewTable.findRowTestHook.SetTestHook(action);
		}

		internal static IDisposable SetFindRowOperatorTestHook(Action<SimpleQueryOperator, int, int> action)
		{
			return ViewTable.findRowOperatorTestHook.SetTestHook(action);
		}

		internal static byte[] SerializeBookmark(SortOrder sortOrder, Bookmark bookmark)
		{
			int num = 4;
			if (!bookmark.IsBOT && !bookmark.IsEOT)
			{
				num += 5 + SerializedValue.ComputeSize(bookmark.KeyValues);
			}
			byte[] array = new byte[num];
			if (bookmark.IsBOT)
			{
				ParseSerialize.SerializeInt32(173305666, array, 0);
			}
			else if (bookmark.IsEOT)
			{
				ParseSerialize.SerializeInt32(173305669, array, 0);
			}
			else
			{
				ParseSerialize.SerializeInt32(175727947, array, 0);
				ParseSerialize.SerializeInt32(sortOrder.GetHashCode(), array, 4);
				array[8] = (bookmark.PositionedOn ? 1 : 0);
				int num2 = 9;
				SerializedValue.Serialize(bookmark.KeyValues, array, ref num2);
			}
			return array;
		}

		internal static Bookmark DeserializeBookmark(SortOrder sortOrder, byte[] serializedBookmark)
		{
			if (serializedBookmark != null && serializedBookmark.Length >= 4)
			{
				int num = ParseSerialize.ParseInt32(serializedBookmark, 0);
				if (num == 173305666)
				{
					if (serializedBookmark.Length != 4)
					{
						throw new StoreException((LID)32888U, ErrorCodeValue.InvalidParameter);
					}
					return Bookmark.BOT;
				}
				else if (num == 173305669)
				{
					if (serializedBookmark.Length != 4)
					{
						throw new StoreException((LID)41080U, ErrorCodeValue.InvalidParameter);
					}
					return Bookmark.EOT;
				}
				else if (num == 175727947)
				{
					if (serializedBookmark.Length < 9)
					{
						throw new StoreException((LID)57464U, ErrorCodeValue.InvalidParameter);
					}
					int num2 = ParseSerialize.ParseInt32(serializedBookmark, 4);
					bool positionedOn = 0 != serializedBookmark[8];
					int num3 = 9;
					IList<object> list;
					if (!SerializedValue.TryParseList(serializedBookmark, ref num3, out list))
					{
						throw new StoreException((LID)49272U, ErrorCodeValue.InvalidParameter);
					}
					if (num2 != sortOrder.GetHashCode() || list == null || list.Count != sortOrder.Count)
					{
						throw new StoreException((LID)48760U, ErrorCodeValue.InvalidBookmark);
					}
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != null && list[i].GetType() != sortOrder[i].Column.Type)
						{
							throw new StoreException((LID)65144U, ErrorCodeValue.InvalidBookmark);
						}
					}
					return new Bookmark(list, positionedOn);
				}
			}
			throw new StoreException((LID)40568U, ErrorCodeValue.InvalidParameter);
		}

		protected internal IList<IIndex> GetInScopePseudoIndexes(Context context, SearchCriteria findRowCriteria)
		{
			IList<IIndex> list;
			IList<IIndex> inScopePseudoIndexes = this.GetInScopePseudoIndexes(context, findRowCriteria, out list);
			if (list == null)
			{
				return inScopePseudoIndexes;
			}
			List<IIndex> list2 = new List<IIndex>(list);
			list2.AddRange(inScopePseudoIndexes);
			return list2;
		}

		protected internal virtual IList<IIndex> GetInScopePseudoIndexes(Context context, SearchCriteria findRowCriteria, out IList<IIndex> masterIndexes)
		{
			masterIndexes = null;
			return null;
		}

		internal CountOperator GetCountOperator(Context context)
		{
			IList<IIndex> list;
			IList<IIndex> inScopePseudoIndexes = this.GetInScopePseudoIndexes(context, null, out list);
			QueryPlanner queryPlanner = new QueryPlanner(context, this.Table, this.tableFunctionParameters, this.implicitCriteria, this.restrictCriteria, null, null, null, this.GetColumnRenames(context), this.GetCategorizedQueryParams(context), inScopePseudoIndexes, list, SortOrder.Empty, Bookmark.BOT, 0, 0, false, this.MustUseLazyIndex, false, true, false, list == null, (this.FilterFactorHints == null) ? QueryPlanner.Hints.Empty : new QueryPlanner.Hints
			{
				FilterFactorHints = this.FilterFactorHints
			});
			CountOperator result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CountOperator countOperator = disposeGuard.Add<CountOperator>(queryPlanner.CreateCountPlan());
				if (list != null)
				{
					this.BringIndexesToCurrent(context, list, countOperator);
				}
				this.BringIndexesToCurrent(context, inScopePseudoIndexes, countOperator);
				disposeGuard.Success();
				result = countOperator;
			}
			return result;
		}

		internal OrdinalPositionOperator GetOrdinalPositionOperator(Context context)
		{
			IList<IIndex> list;
			IList<IIndex> inScopePseudoIndexes = this.GetInScopePseudoIndexes(context, null, out list);
			QueryPlanner queryPlanner = new QueryPlanner(context, this.Table, this.tableFunctionParameters, this.implicitCriteria, this.restrictCriteria, null, null, null, this.GetColumnRenames(context), this.GetCategorizedQueryParams(context), inScopePseudoIndexes, list, this.GetSortOrderForView(), this.Bookmark, 0, 0, false, this.MustUseLazyIndex, false, true, false, list == null, (this.FilterFactorHints == null) ? QueryPlanner.Hints.Empty : new QueryPlanner.Hints
			{
				FilterFactorHints = this.FilterFactorHints
			});
			OrdinalPositionOperator result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				OrdinalPositionOperator ordinalPositionOperator = disposeGuard.Add<OrdinalPositionOperator>(queryPlanner.CreateOrdinalPositionPlan());
				if (list != null)
				{
					this.BringIndexesToCurrent(context, list, ordinalPositionOperator);
				}
				this.BringIndexesToCurrent(context, inScopePseudoIndexes, ordinalPositionOperator);
				disposeGuard.Success();
				result = ordinalPositionOperator;
			}
			return result;
		}

		internal SimpleQueryOperator GetQueryRowsOperator(Context context, int maxRows, bool backwards)
		{
			SimpleQueryOperator simpleQueryOperator = null;
			IList<IIndex> list = null;
			IList<IIndex> list2 = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				if (!this.IsViewEmpty(context))
				{
					list = this.GetInScopePseudoIndexes(context, null, out list2);
					QueryPlanner queryPlanner = new QueryPlanner(context, this.Table, this.tableFunctionParameters, this.implicitCriteria, this.restrictCriteria, null, this.ViewColumns, this.LongValueColumnsToPreread, this.GetColumnRenames(context), this.GetCategorizedQueryParams(context), list, list2, this.GetSortOrderForView(), this.bookmark, 0, maxRows, backwards, this.MustUseLazyIndex, false, true, false, list2 == null, (this.FilterFactorHints == null) ? QueryPlanner.Hints.Empty : new QueryPlanner.Hints
					{
						FilterFactorHints = this.FilterFactorHints
					});
					simpleQueryOperator = disposeGuard.Add<SimpleQueryOperator>(queryPlanner.CreatePlan());
				}
				if (simpleQueryOperator != null)
				{
					if (list2 != null)
					{
						this.BringIndexesToCurrent(context, list2, simpleQueryOperator);
					}
					this.BringIndexesToCurrent(context, list, simpleQueryOperator);
				}
				disposeGuard.Success();
			}
			return simpleQueryOperator;
		}

		internal SimpleQueryOperator GetFindRowOperator(Context context, Bookmark startBookmark, SearchCriteria findRowCriteria, bool backwards)
		{
			int num;
			int num2;
			return this.GetFindRowOperator(context, startBookmark, findRowCriteria, backwards, out num, out num2);
		}

		internal SimpleQueryOperator GetFindRowOperator(Context context, Bookmark startBookmark, SearchCriteria findRowCriteria, bool backwards, out int planCost, out int planCardinality)
		{
			planCost = 0;
			planCardinality = 0;
			SimpleQueryOperator simpleQueryOperator = null;
			IList<IIndex> list = null;
			IList<IIndex> list2 = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				if (!this.IsViewEmpty(context))
				{
					findRowCriteria = this.RewriteSearchCriteria(context, findRowCriteria);
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.TraceDebug<SearchCriteria>(0L, "FindRow: Generating FindRow query operator for criteria [{0}].", findRowCriteria);
					}
					list = this.GetInScopePseudoIndexes(context, findRowCriteria, out list2);
					QueryPlanner queryPlanner = new QueryPlanner(context, this.Table, this.tableFunctionParameters, this.implicitCriteria, this.restrictCriteria, findRowCriteria, this.ViewColumns, null, this.GetColumnRenames(context), this.GetCategorizedQueryParams(context), list, list2, this.GetSortOrderForView(), startBookmark, 0, 1, backwards, this.MustUseLazyIndex, false, true, false, list2 == null, (this.FilterFactorHints == null) ? QueryPlanner.Hints.Empty : new QueryPlanner.Hints
					{
						FilterFactorHints = this.FilterFactorHints
					});
					simpleQueryOperator = disposeGuard.Add<SimpleQueryOperator>(queryPlanner.CreatePlan(out planCost, out planCardinality));
				}
				if (simpleQueryOperator != null)
				{
					if (list2 != null)
					{
						this.BringIndexesToCurrent(context, list2, simpleQueryOperator);
					}
					this.BringIndexesToCurrent(context, list, simpleQueryOperator);
				}
				disposeGuard.Success();
			}
			if (ViewTable.findRowOperatorTestHook.Value != null)
			{
				ViewTable.findRowOperatorTestHook.Value(simpleQueryOperator, planCost, planCardinality);
			}
			return simpleQueryOperator;
		}

		internal void ResetCollapseStateForTest(int expandedCount)
		{
			this.collapseState = new CategorizedTableCollapseState(this.categoryCount, expandedCount);
		}

		protected virtual bool IsViewEmpty(Context context)
		{
			return false;
		}

		protected void SetCategorizedView(int categoryCount, int expandedCount, SortOrder categoryHeadersSortOrder, CategoryHeaderSortOverride[] categoryHeaderSortOverrides)
		{
			this.categoryCount = categoryCount;
			this.collapseState = new CategorizedTableCollapseState(categoryCount, expandedCount);
			this.categoryHeadersSortOrder = categoryHeadersSortOrder;
			this.categoryHeaderSortOverrides = categoryHeaderSortOverrides;
		}

		protected SortOrder GetSortOrderForView()
		{
			if (this.IsCategorizedView)
			{
				List<Column> list = new List<Column>(this.CategoryHeadersSortOrder.Columns);
				list.AddRange(this.SortOrder.Columns);
				List<bool> list2 = new List<bool>(this.CategoryHeadersSortOrder.Ascending);
				list2.AddRange(this.SortOrder.Ascending);
				return new SortOrder(list, list2);
			}
			return this.SortOrder;
		}

		protected virtual Reader FindRow(Context context, SearchCriteria findRowCriteria, Bookmark startBookmark, bool backwards)
		{
			FaultInjection.InjectFault(ViewTable.findRowTestHook);
			SimpleQueryOperator simpleQueryOperator = this.GetFindRowOperator(context, startBookmark, findRowCriteria, backwards);
			if (simpleQueryOperator == null)
			{
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.TraceDebug(0L, "FindRow: Failed (no operator).");
				}
				return null;
			}
			Reader reader = null;
			try
			{
				reader = simpleQueryOperator.ExecuteReader(true);
				simpleQueryOperator = null;
				bool flag = reader.Read();
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices.ExTraceGlobals.ViewTableFindRowTracer.TraceDebug<string>(0L, "FindRow: {0}.", flag ? "Succeeded" : "Failed");
				}
				if (flag)
				{
					this.BookmarkCurrentRow(reader, true);
					Reader result = reader;
					reader = null;
					return result;
				}
			}
			finally
			{
				if (reader != null)
				{
					reader.Dispose();
				}
				if (simpleQueryOperator != null)
				{
					simpleQueryOperator.Dispose();
				}
			}
			return null;
		}

		protected void InvalidateBookmarkAndRowCount()
		{
			this.bookmark = Bookmark.BOT;
			this.rowCountValid = false;
		}

		protected void AdjustRowCountAfterExpandOrCollapse(int delta)
		{
			if (this.rowCountValid)
			{
				this.rowCount += delta;
			}
		}

		protected void BookmarkRow(IList<object> keyValues, bool positionedOn)
		{
			this.bookmark = new Bookmark(keyValues, positionedOn);
		}

		protected void SetImplicitCriteria(SearchCriteria implicitCriteria)
		{
			this.implicitCriteria = implicitCriteria;
		}

		protected virtual void BringIndexesToCurrent(Context context, IList<IIndex> indexList, DataAccessOperator queryPlan)
		{
		}

		protected virtual SearchCriteria RewriteSearchCriteria(Context context, SearchCriteria criteriaToRewrite)
		{
			if (criteriaToRewrite == null)
			{
				return null;
			}
			return criteriaToRewrite.InspectAndFix(delegate(SearchCriteria criteria, CompareInfo compareInfo)
			{
				if (this.MvExplosion)
				{
					SearchCriteriaCompare searchCriteriaCompare = criteria as SearchCriteriaCompare;
					if (searchCriteriaCompare != null)
					{
						ExtendedPropertyColumn extendedPropertyColumn = searchCriteriaCompare.Lhs as ExtendedPropertyColumn;
						if (extendedPropertyColumn != null)
						{
							StorePropTag storePropTag = ((ExtendedPropertyColumn)this.multiValueInstanceColumn).StorePropTag;
							if (extendedPropertyColumn.StorePropTag.IsMultiValued && extendedPropertyColumn.StorePropTag == storePropTag.ChangeType(storePropTag.PropType & (PropertyType)57343))
							{
								return Factory.CreateSearchCriteriaCompare(this.multiValueInstanceColumn, searchCriteriaCompare.RelOp, searchCriteriaCompare.Rhs);
							}
						}
					}
				}
				return criteria;
			}, (context.Culture == null) ? null : context.Culture.CompareInfo, false);
		}

		protected virtual IReadOnlyDictionary<Column, Column> GetColumnRenames(Context context)
		{
			return null;
		}

		protected virtual CategorizedQueryParams GetCategorizedQueryParams(Context context)
		{
			return null;
		}

		protected virtual bool CheckBookmarkPosition(Context context, Bookmark bookmark)
		{
			if (!this.IsViewEmpty(context))
			{
				IList<IIndex> list;
				IList<IIndex> inScopePseudoIndexes = this.GetInScopePseudoIndexes(context, null, out list);
				QueryPlanner queryPlanner = new QueryPlanner(context, this.Table, this.tableFunctionParameters, this.implicitCriteria, this.restrictCriteria, null, null, null, this.GetColumnRenames(context), this.GetCategorizedQueryParams(context), inScopePseudoIndexes, list, this.GetSortOrderForView(), bookmark, 0, 1, !bookmark.PositionedOn, this.MustUseLazyIndex, false, true, false, list == null, (this.FilterFactorHints == null) ? QueryPlanner.Hints.Empty : new QueryPlanner.Hints
				{
					FilterFactorHints = this.FilterFactorHints
				});
				using (SimpleQueryOperator simpleQueryOperator = queryPlanner.CreatePlan())
				{
					if (list != null)
					{
						this.BringIndexesToCurrent(context, list, simpleQueryOperator);
					}
					this.BringIndexesToCurrent(context, inScopePseudoIndexes, simpleQueryOperator);
					using (Reader reader = simpleQueryOperator.ExecuteReader(false))
					{
						if (reader.Read())
						{
							return this.CheckBookmarkMatchesRow(context, reader, bookmark, false);
						}
					}
				}
				return false;
			}
			return false;
		}

		protected virtual void DoSeekQuery(Context context, Bookmark bookmark, bool backwards, int rowsToSeek, int? newPosition, out bool soughtLessThanRowCount, out int rowCountActuallySought)
		{
			soughtLessThanRowCount = true;
			rowCountActuallySought = 0;
			if (!this.IsViewEmpty(context))
			{
				IList<IIndex> list;
				IList<IIndex> inScopePseudoIndexes = this.GetInScopePseudoIndexes(context, null, out list);
				IList<Column> columnsToFetch = this.IsCategorizedView ? new Column[]
				{
					PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount)
				} : null;
				QueryPlanner queryPlanner = new QueryPlanner(context, this.Table, this.tableFunctionParameters, this.implicitCriteria, this.restrictCriteria, null, columnsToFetch, null, this.GetColumnRenames(context), this.GetCategorizedQueryParams(context), inScopePseudoIndexes, list, this.GetSortOrderForView(), bookmark, rowsToSeek - 1, 1, backwards, this.MustUseLazyIndex, false, true, false, list == null, (this.FilterFactorHints == null) ? QueryPlanner.Hints.Empty : new QueryPlanner.Hints
				{
					FilterFactorHints = this.FilterFactorHints
				});
				using (SimpleQueryOperator simpleQueryOperator = queryPlanner.CreatePlan())
				{
					if (simpleQueryOperator != null)
					{
						if (list != null)
						{
							this.BringIndexesToCurrent(context, list, simpleQueryOperator);
						}
						this.BringIndexesToCurrent(context, inScopePseudoIndexes, simpleQueryOperator);
						using (Reader reader = simpleQueryOperator.ExecuteReader(false))
						{
							int num;
							if (reader.Read(out num))
							{
								this.BookmarkCurrentRow(reader, backwards, newPosition);
								if (this.rowCountValid && this.rowCount <= rowsToSeek - 1)
								{
									this.rowCountValid = false;
								}
								soughtLessThanRowCount = false;
								rowCountActuallySought = rowsToSeek;
							}
							else
							{
								soughtLessThanRowCount = true;
								rowCountActuallySought = num;
							}
						}
					}
				}
			}
			if (soughtLessThanRowCount)
			{
				this.bookmark = (backwards ? Bookmark.BOT : Bookmark.EOT);
			}
		}

		private void Uncategorize()
		{
			if (this.IsCategorizedView)
			{
				this.categoryCount = 0;
				this.collapseState = null;
				this.categoryHeadersSortOrder = SortOrder.Empty;
				this.categoryHeaderSortOverrides = null;
				this.InvalidateBookmarkAndRowCount();
			}
		}

		public const int MaximumNumberOfCategoryHeaders = 4;

		private const int MagicBookmarkBOT = 173305666;

		private const int MagicBookmarkEOT = 173305669;

		private const int MagicBookmarkKey = 175727947;

		private static readonly ClientType[] excludedFromColumnValidation = new ClientType[]
		{
			ClientType.WebServices,
			ClientType.MoMT,
			ClientType.User,
			ClientType.Migration,
			ClientType.PublicFolderSystem,
			ClientType.Management,
			ClientType.LoadGen,
			ClientType.Pop,
			ClientType.Imap
		};

		private static Hookable<Action> findRowTestHook = Hookable<Action>.Create(true, null);

		private static Hookable<Action<SimpleQueryOperator, int, int>> findRowOperatorTestHook = Hookable<Action<SimpleQueryOperator, int, int>>.Create(true, null);

		private readonly Mailbox mailbox;

		private readonly Table table;

		private IList<Column> viewColumns;

		private SearchCriteria implicitCriteria;

		private SearchCriteria restrictCriteria;

		private SortOrder sortOrder;

		private int categoryCount;

		private CategorizedTableCollapseState collapseState;

		private SortOrder categoryHeadersSortOrder;

		private CategoryHeaderSortOverride[] categoryHeaderSortOverrides;

		private Bookmark bookmark;

		private Bookmark lastBookmark;

		private int rowCount;

		private bool rowCountValid;

		private Column multiValueInstanceColumn;

		private object[] tableFunctionParameters;

		private int optimizedSeeks;
	}
}
