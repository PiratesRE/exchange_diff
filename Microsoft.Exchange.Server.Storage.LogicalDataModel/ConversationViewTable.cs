using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ConversationViewTable : ViewTable
	{
		public ConversationViewTable(Context context, Mailbox mailbox, ExchangeId folderId, bool useIndexForEmptyFolder, bool expandedMembersOnly) : base(mailbox, Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database).Table)
		{
			this.messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database);
			this.folderId = folderId;
			this.useIndexForEmptyFolder = useIndexForEmptyFolder;
			this.expandedMembersOnly = expandedMembersOnly;
			this.folder = Folder.OpenFolder(context, base.Mailbox, folderId);
			SearchCriteria searchCriteria = Factory.CreateSearchCriteriaCompare(this.messageTable.MailboxPartitionNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailbox.MailboxPartitionNumber, this.messageTable.MailboxPartitionNumber));
			if (this.SearchFolder != null && this.SearchFolder.SearchResults != null && this.SearchFolder.SearchResults.IsValid)
			{
				searchCriteria = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
				{
					searchCriteria,
					Factory.CreateSearchCriteriaCompare(this.messageTable.FolderId, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(null, this.messageTable.FolderId))
				});
			}
			base.SetImplicitCriteria(searchCriteria);
			Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Conversation, PropTag.Message.ConversationMessageDeliveryTime);
			SortOrderBuilder sortOrderBuilder = new SortOrderBuilder
			{
				{
					column,
					false
				}
			};
			this.SortTable(sortOrderBuilder.ToSortOrder());
		}

		public ConversationViewTable(Context context, Mailbox mailbox, ExchangeId folderId, IList<Column> columns, SortOrder sortOrder, SearchCriteria criteria) : this(context, mailbox, folderId, true, false)
		{
			base.SetColumns(context, columns);
			this.SortTable(sortOrder);
			this.Restrict(context, criteria);
		}

		private static bool IsNotTruncated(Column column)
		{
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			return !(extendedPropertyColumn != null) || (extendedPropertyColumn.StorePropTag.PropType != PropertyType.Binary && extendedPropertyColumn.StorePropTag.PropType != PropertyType.Unicode) || ((extendedPropertyColumn.MaxLength != 0) ? extendedPropertyColumn.MaxLength : extendedPropertyColumn.Size) <= PhysicalIndex.MaxSortColumnLength(extendedPropertyColumn.Type);
		}

		private static bool IsInstanceNumber(Column column)
		{
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			return extendedPropertyColumn != null && extendedPropertyColumn.StorePropTag == PropTag.Message.InstanceNum;
		}

		private static bool IsInstanceKey(Column column)
		{
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			return extendedPropertyColumn != null && (extendedPropertyColumn.StorePropTag == PropTag.Message.InstanceKey || extendedPropertyColumn.StorePropTag == PropTag.Message.InstanceKeySvrEid);
		}

		protected override Index LogicalKeyIndex
		{
			get
			{
				if (this.SearchFolder != null && this.SearchFolder.SearchResults != null && this.SearchFolder.SearchResults.IsValid)
				{
					if (ExTraceGlobals.ViewTableFindRowTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ViewTableFindRowTracer.TraceDebug(0L, "Conversation view table works in optimized instant search mode.");
					}
					SearchResultsTableFunction searchResultsTableFunction = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.SearchResultsTableFunction(base.Mailbox.Database);
					return searchResultsTableFunction.TableFunction.PrimaryKeyIndex;
				}
				return this.messageTable.MessagePK;
			}
		}

		protected override bool MustUseLazyIndex
		{
			get
			{
				return true;
			}
		}

		internal ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		internal Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		internal SearchFolder SearchFolder
		{
			get
			{
				return this.Folder as SearchFolder;
			}
		}

		internal bool IsSearchFolder
		{
			get
			{
				return this.SearchFolder != null;
			}
		}

		public override ViewTable QueryRowsViewTable
		{
			get
			{
				if (this.conversationMessageViewTable != null)
				{
					return this.conversationMessageViewTable;
				}
				return this;
			}
		}

		public bool IsOptimizedInstantSearch
		{
			get
			{
				return this.SearchFolder != null && this.SearchFolder.SearchResults != null && this.SearchFolder.SearchResults.IsValid;
			}
		}

		public override void SortTable(SortOrder sortOrder)
		{
			if (this.IsOptimizedInstantSearch)
			{
				SearchResultsTableFunction searchResultsTableFunction = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.SearchResultsTableFunction(base.Mailbox.Database);
				sortOrder = searchResultsTableFunction.TableFunction.PrimaryKeyIndex.SortOrder;
			}
			base.SortTable(sortOrder);
		}

		public override int GetRowCount(Context context)
		{
			if (this.IsViewEmpty(context))
			{
				return 0;
			}
			if (!base.MvExplosion && base.RestrictCriteria == null && !this.IsSearchFolder)
			{
				int result = 0;
				Folder folder = Folder.OpenFolder(context, base.Mailbox, this.folderId);
				if (folder != null)
				{
					result = (int)folder.GetConversationCount(context);
				}
				return result;
			}
			if (this.IsOptimizedInstantSearch)
			{
				return this.SearchFolder.SearchResults.ConversationRows.Count;
			}
			return base.GetRowCount(context);
		}

		public override void Reset()
		{
			base.Reset();
			this.conversationMessageViewTable = null;
		}

		public override void SetColumns(Context context, IList<Column> columns, ViewSetColumnsFlag flags)
		{
			if (this.expandedMembersOnly)
			{
				if (!this.SkipPropertiesPromotionValidation(context, flags))
				{
					Folder folder = Folder.OpenFolder(context, base.Mailbox, this.folderId);
					if (folder == null)
					{
						throw new StoreException((LID)34304U, ErrorCodeValue.InvalidObject);
					}
					PropertyPromotionHelper.ValidatePropertiesPromotion(context, base.Mailbox, folder.GetName(context), columns);
				}
				this.conversationMembersViewColumns = columns;
				base.SetColumns(context, new List<Column>
				{
					this.messageTable.ConversationMembers
				}, flags);
				return;
			}
			if (!this.SkipPropertiesPromotionValidation(context, flags))
			{
				List<Column> list = new List<Column>(columns.Count);
				foreach (Column column in columns)
				{
					ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
					if (extendedPropertyColumn != null)
					{
						StorePropTag propTag = extendedPropertyColumn.StorePropTag.ChangeType(extendedPropertyColumn.StorePropTag.PropType & (PropertyType)57343);
						if (!ConversationMembers.IsAggregateProperty(propTag))
						{
							list.Add(column);
						}
					}
				}
				if (list != null && list.Count != 0)
				{
					Folder folder2 = Folder.OpenFolder(context, base.Mailbox, this.folderId);
					if (folder2 == null)
					{
						throw new StoreException((LID)50688U, ErrorCodeValue.InvalidObject);
					}
					PropertyPromotionHelper.ValidatePropertiesPromotion(context, base.Mailbox, folder2.GetName(context), list);
				}
			}
			base.SetColumns(context, columns, flags);
		}

		public override void Restrict(Context context, SearchCriteria restrictCriteria)
		{
			base.Restrict(context, restrictCriteria);
			this.conversationMessageViewTable = null;
		}

		public override Reader FindRow(Context context, SearchCriteria criteria, ViewSeekOrigin origin, byte[] bookmark, bool backwards, out bool bookmarkPositionChanged)
		{
			Reader reader = null;
			if (this.expandedMembersOnly)
			{
				this.conversationMessageViewTable = null;
				using (Reader reader2 = base.FindRow(context, criteria, origin, bookmark, backwards, out bookmarkPositionChanged))
				{
					if (reader2 != null)
					{
						byte[] binary = reader2.GetBinary(this.messageTable.ConversationMembers);
						int num = 0;
						List<FidMid> list = FidMidListSerializer.FromBytes(binary, ref num, base.Mailbox.ReplidGuidMap);
						ConversationMessageViewTable conversationMessageViewTable = new ConversationMessageViewTable(context, base.Mailbox, this.folderId, new List<ConversationMembersBlob>
						{
							new ConversationMembersBlob(list[0].FolderId.To26ByteArray(), list[0].MessageId.To26ByteArray(), 0)
						});
						conversationMessageViewTable.SetColumns(context, this.conversationMembersViewColumns);
						reader = conversationMessageViewTable.QueryRows(context, 1, false);
						if (!reader.Read())
						{
							reader.Dispose();
							reader = null;
						}
					}
					goto IL_F6;
				}
			}
			reader = base.FindRow(context, criteria, origin, bookmark, backwards, out bookmarkPositionChanged);
			IL_F6:
			this.findRowCriteria = null;
			this.rewrittenFindRowCriteria = null;
			return reader;
		}

		public override void SeekRow(Context context, ViewSeekOrigin origin, byte[] bookmark, int numberOfRows, bool wantSoughtRowCount, out bool soughtLessThanRowCount, out int rowCountActuallySought, bool validateBookmarkPosition, out bool bookmarkPositionChanged)
		{
			this.conversationMessageViewTable = null;
			base.SeekRow(context, origin, bookmark, numberOfRows, wantSoughtRowCount, out soughtLessThanRowCount, out rowCountActuallySought, validateBookmarkPosition, out bookmarkPositionChanged);
		}

		public override Reader QueryRows(Context context, int rowCount, bool backwards)
		{
			if (this.expandedMembersOnly)
			{
				if (this.conversationMessageViewTable == null)
				{
					if (rowCount > 200)
					{
						throw new StoreException((LID)54672U, ErrorCodeValue.TooComplex);
					}
					IList<ConversationMembersBlob> conversationMembersList = null;
					using (Reader reader = base.QueryRows(context, rowCount, backwards))
					{
						conversationMembersList = this.BuildExpandedConversationMembersBlob(context, reader, true);
					}
					this.conversationMessageViewTable = new ConversationMessageViewTable(context, base.Mailbox, this.folderId, conversationMembersList);
					this.conversationMessageViewTable.SetColumns(context, this.conversationMembersViewColumns);
				}
				return this.conversationMessageViewTable.QueryRows(context, 0, false);
			}
			return base.QueryRows(context, rowCount, backwards);
		}

		public override bool NeedIndexForPositionOrRowCount(Context context)
		{
			return (!this.IsViewEmpty(context) && (base.MvExplosion || base.RestrictCriteria != null || this.IsSearchFolder)) || !base.Bookmark.PositionValid;
		}

		public override IChunked PrepareIndexes(Context context, SearchCriteria findRowCriteria)
		{
			if (this.IsOptimizedInstantSearch || this.IsViewEmpty(context))
			{
				return null;
			}
			this.rewrittenFindRowCriteria = this.RewriteSearchCriteria(context, findRowCriteria);
			this.findRowCriteria = findRowCriteria;
			LogicalIndex masterIndex;
			LogicalIndex pseudoIndex = this.GetConversationIndex(context, this.rewrittenFindRowCriteria, out masterIndex);
			if (masterIndex == null)
			{
				return this.PrepareIndex(context, pseudoIndex);
			}
			return new CompositeChunked(new Func<Context, IChunked>[]
			{
				(Context c) => this.PrepareIndex(c, masterIndex),
				(Context c) => this.PrepareIndex(c, pseudoIndex)
			});
		}

		protected override bool IsViewEmpty(Context context)
		{
			if (this.IsOptimizedInstantSearch)
			{
				return this.SearchFolder.SearchResults.ConversationRows == null || this.SearchFolder.SearchResults.ConversationRows.Count == 0;
			}
			Folder folder = Folder.OpenFolder(context, base.Mailbox, this.folderId);
			return folder == null || (this.IsSearchFolder && ((SearchFolder)folder).GetLogicalIndexNumber(context) == null) || (!this.useIndexForEmptyFolder && 0L == folder.GetMessageCount(context));
		}

		protected override SearchCriteria RewriteSearchCriteria(Context context, SearchCriteria criteria)
		{
			if (object.ReferenceEquals(criteria, this.findRowCriteria))
			{
				return this.rewrittenFindRowCriteria;
			}
			criteria = base.RewriteSearchCriteria(context, criteria);
			if (criteria == null)
			{
				return null;
			}
			criteria = criteria.InspectAndFix(delegate(SearchCriteria criterion, CompareInfo compareInfo)
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					SearchCriteriaCompare searchCriteriaCompare = criterion as SearchCriteriaCompare;
					if (searchCriteriaCompare != null && (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal || searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.NotEqual) && searchCriteriaCompare.Rhs is ConstantColumn)
					{
						ConversationItem conversationItem = null;
						int? num = null;
						if (searchCriteriaCompare.Lhs is ExtendedPropertyColumn)
						{
							StorePropTag storePropTag = ((ExtendedPropertyColumn)searchCriteriaCompare.Lhs).StorePropTag;
							uint propTag = storePropTag.PropTag;
							if (propTag <= 267976962U)
							{
								if (propTag <= 267780354U)
								{
									if (propTag != 267780347U && propTag != 267780354U)
									{
										goto IL_362;
									}
								}
								else if (propTag != 267976955U && propTag != 267976962U)
								{
									goto IL_362;
								}
							}
							else if (propTag <= 268370178U)
							{
								if (propTag != 268370171U && propTag != 268370178U)
								{
									goto IL_362;
								}
							}
							else if (propTag != 806551810U)
							{
								if (propTag != 1732902932U && propTag != 1733099540U)
								{
									goto IL_362;
								}
							}
							else
							{
								byte[] conversationId = (byte[])((ConstantColumn)searchCriteriaCompare.Rhs).Value;
								conversationItem = disposeGuard.Add<ConversationItem>(ConversationItem.OpenConversationItem(context, this.Mailbox, conversationId));
								if (conversationItem == null)
								{
									return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
								}
								goto IL_362;
							}
							ExchangeId mid = ExchangeId.Null;
							ushort propId = storePropTag.PropId;
							if (propId <= 4089)
							{
								if (propId != 4086 && propId != 4089)
								{
									goto IL_279;
								}
							}
							else if (propId != 4095)
							{
								if (propId == 26442 || propId == 26445)
								{
									long legacyId = (long)((ConstantColumn)searchCriteriaCompare.Rhs).Value;
									mid = ExchangeId.CreateFromInt64(context, this.Mailbox.ReplidGuidMap, legacyId);
									goto IL_279;
								}
								goto IL_279;
							}
							byte[] entryId = (byte[])((ConstantColumn)searchCriteriaCompare.Rhs).Value;
							ExchangeId exchangeId;
							ExchangeId exchangeId2;
							int num2;
							if (!EntryIdHelpers.ParseServerEntryId(context, this.Mailbox.ReplidGuidMap, entryId, false, out exchangeId, out exchangeId2, out num2))
							{
								return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
							}
							if (storePropTag.PropId == 4086)
							{
								if (exchangeId != ConversationItem.GetConversationFolderId(context, this.Mailbox))
								{
									return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
								}
								if (!this.MvExplosion)
								{
									if (num2 != 0)
									{
										return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
									}
								}
								else
								{
									if (num2 == 0)
									{
										return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
									}
									num = new int?(num2);
								}
								mid = exchangeId2;
							}
							else
							{
								mid = exchangeId;
							}
							IL_279:
							if (this.IsOptimizedInstantSearch)
							{
								SearchResultsTableFunction searchResultsTableFunction = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.SearchResultsTableFunction(this.Mailbox.Database);
								return Factory.CreateSearchCriteriaCompare(searchResultsTableFunction.SortPosition, searchCriteriaCompare.RelOp, Factory.CreateConstantColumn((int)mid.Counter, searchResultsTableFunction.SortPosition));
							}
							conversationItem = disposeGuard.Add<ConversationItem>(ConversationItem.OpenConversationItem(context, this.Mailbox, mid));
							if (conversationItem == null)
							{
								return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
							}
						}
						IL_362:
						if (conversationItem == null && searchCriteriaCompare.Lhs == this.messageTable.MessageDocumentId)
						{
							int documentId = (int)((ConstantColumn)searchCriteriaCompare.Rhs).Value;
							conversationItem = disposeGuard.Add<ConversationItem>(ConversationItem.OpenConversationItem(context, this.Mailbox, documentId));
							if (conversationItem == null)
							{
								return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaFalse() : Factory.CreateSearchCriteriaTrue();
							}
						}
						if (conversationItem != null)
						{
							HashSet<StorePropTag> hashSet = new HashSet<StorePropTag>();
							hashSet = new HashSet<StorePropTag>();
							foreach (Column column in this.SortOrder.Columns)
							{
								StorePropTag storePropTag2 = StorePropTag.Invalid;
								ExtendedPropertyColumn extendedPropertyColumn;
								if (PropertySchema.IsMultiValueInstanceColumn(column, out extendedPropertyColumn))
								{
									storePropTag2 = extendedPropertyColumn.StorePropTag;
								}
								else if (column is PropertyColumn)
								{
									storePropTag2 = ((PropertyColumn)column).StorePropTag;
								}
								if (ConversationMembers.IsAggregateProperty(storePropTag2))
								{
									hashSet.Add(storePropTag2);
								}
							}
							ConversationMembers conversationMembers = conversationItem.GetConversationMembers(context, null, hashSet);
							ConversationValueBag conversationValueBag;
							if (this.IsSearchFolder)
							{
								List<FidMid> messages = conversationMembers.ConversationMessages.ToList<FidMid>();
								SearchFolder searchFolder = (SearchFolder)Folder.OpenFolder(context, this.Mailbox, this.folderId);
								ICollection<FidMid> membersFilterList = searchFolder.FilterMessages(context, messages, new bool?(false));
								conversationValueBag = new ConversationValueBag(conversationItem, null, membersFilterList, conversationMembers);
							}
							else
							{
								conversationValueBag = new ConversationValueBag(conversationItem, this.folderId, conversationMembers);
							}
							Column col = null;
							if (this.MvExplosion)
							{
								col = PropertySchema.MapToColumn(this.Mailbox.Database, ObjectType.Conversation, PropTag.Message.InstanceNum);
							}
							List<SearchCriteria> list = new List<SearchCriteria>(this.SortOrder.Count);
							for (int i = 0; i < this.SortOrder.Count; i++)
							{
								Column column2 = this.SortOrder[i].Column;
								if (column2 == col)
								{
									if (num != null)
									{
										list.Add(Factory.CreateSearchCriteriaCompare(column2, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(num.Value, column2)));
									}
								}
								else if (this.MvExplosion && column2 is ExtendedPropertyColumn && ((ExtendedPropertyColumn)column2).StorePropTag.IsMultiValueInstance)
								{
									if (num != null)
									{
										StorePropTag storePropTag3 = ((ExtendedPropertyColumn)column2).StorePropTag;
										StorePropTag propertyTag = storePropTag3.ChangeType(storePropTag3.PropType & (PropertyType)57343);
										Column column3 = PropertySchema.MapToColumn(this.Mailbox.Database, ObjectType.Conversation, propertyTag);
										object columnValue = conversationValueBag.GetColumnValue(context, column3);
										Array array = (Array)columnValue;
										if (array != null && num.Value > 0 && num.Value - 1 < array.Length)
										{
											list.Add(Factory.CreateSearchCriteriaCompare(column2, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(array.GetValue(num.Value - 1), column2)));
										}
										else
										{
											list.Add(Factory.CreateSearchCriteriaFalse());
										}
									}
								}
								else
								{
									list.Add(Factory.CreateSearchCriteriaCompare(column2, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(conversationValueBag.GetColumnValue(context, column2), column2)));
								}
							}
							return (searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal) ? Factory.CreateSearchCriteriaAnd(list.ToArray()) : Factory.CreateSearchCriteriaNot(Factory.CreateSearchCriteriaAnd(list.ToArray()));
						}
					}
				}
				return criterion;
			}, (context.Culture == null) ? null : context.Culture.CompareInfo, false);
			return MessageViewTable.RewriteMessageSearchCriteria(context, base.Mailbox, criteria, base.SortOrder, true, base.MvExplosion, ExchangeId.Null);
		}

		protected internal override IList<IIndex> GetInScopePseudoIndexes(Context context, SearchCriteria findRowCriteria, out IList<IIndex> masterIndexes)
		{
			masterIndexes = null;
			if (this.IsOptimizedInstantSearch)
			{
				SearchResultsTableFunction searchResultsTableFunction = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.SearchResultsTableFunction(base.Mailbox.Database);
				object[] tableFunctionParameters = new object[]
				{
					this.SearchFolder.SearchResults.ConversationRows
				};
				Dictionary<Column, Column> dictionary = new Dictionary<Column, Column>(3);
				dictionary.Add(this.messageTable.MailboxPartitionNumber, Factory.CreateConstantColumn(base.Mailbox.MailboxPartitionNumber, this.messageTable.MailboxPartitionNumber));
				dictionary.Add(this.messageTable.MessageDocumentId, searchResultsTableFunction.MessageDocumentId);
				if (UnifiedMailbox.IsReady(context, context.Database))
				{
					dictionary.Add(this.messageTable.MailboxNumber, Factory.CreateConstantColumn(base.Mailbox.MailboxNumber, this.messageTable.MailboxNumber));
				}
				return new IIndex[]
				{
					new SimplePseudoIndex(this.messageTable.Table, searchResultsTableFunction.TableFunction, tableFunctionParameters, searchResultsTableFunction.TableFunction.PrimaryKeyIndex.SortOrder, dictionary, null, false)
				};
			}
			LogicalIndex logicalIndex;
			IIndex conversationIndex = this.GetConversationIndex(context, findRowCriteria, out logicalIndex);
			if (logicalIndex != null)
			{
				masterIndexes = new IIndex[]
				{
					logicalIndex
				};
			}
			return new IIndex[]
			{
				conversationIndex
			};
		}

		private static IEnumerable<IColumnValueBag> ColumnValueBagsEnumeratorImpl(IContextProvider contextProvider, JoinOperator.JoinOperatorDefinition selectConversationObjectsOperatorDefinition, int mailboxNumber, ExchangeId folderId, bool isSearchFolder, LogicalIndex searchFolderBaseViewIndex, IEnumerable<Column> requiredColumns, IInterruptControl interruptControl, int conversationPrereadChunkSize, int maxConversationMessagesToPreread)
		{
			HashSet<StorePropTag> aggregatePropertiesToCompute = null;
			if (requiredColumns != null)
			{
				aggregatePropertiesToCompute = new HashSet<StorePropTag>();
				foreach (Column column in requiredColumns)
				{
					StorePropTag storePropTag = StorePropTag.Invalid;
					ExtendedPropertyColumn extendedPropertyColumn;
					if (PropertySchema.IsMultiValueInstanceColumn(column, out extendedPropertyColumn))
					{
						storePropTag = extendedPropertyColumn.StorePropTag;
					}
					else if (column is PropertyColumn)
					{
						storePropTag = ((PropertyColumn)column).StorePropTag;
					}
					if (ConversationMembers.IsAggregateProperty(storePropTag))
					{
						aggregatePropertiesToCompute.Add(storePropTag);
					}
				}
			}
			Queue<ConversationItem> conversationQueue = new Queue<ConversationItem>(conversationPrereadChunkSize);
			try
			{
				using (JoinOperator queryOperator = (JoinOperator)selectConversationObjectsOperatorDefinition.CreateOperator(contextProvider))
				{
					using (Reader reader = queryOperator.ExecuteReader(false))
					{
						queryOperator.PreReadAhead = false;
						queryOperator.PreReadCacheSize = conversationPrereadChunkSize;
						reader.EnableInterrupts(interruptControl);
						Mailbox mailboxToUse = null;
						SearchFolder searchFolderToUse = null;
						object boxedMailboxPartitionNumber = null;
						object boxedFalse = false;
						MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(contextProvider.CurrentContext.Database);
						List<FidMid> messageFidMids = new List<FidMid>(conversationPrereadChunkSize * 3);
						List<KeyRange> keys = new List<KeyRange>(conversationPrereadChunkSize * 3);
						bool rowFound;
						do
						{
							rowFound = reader.Read();
							if (mailboxToUse == null)
							{
								IMailboxContext mailboxContext = contextProvider.CurrentContext.GetMailboxContext(mailboxNumber);
								mailboxToUse = (Mailbox)mailboxContext;
								if (boxedMailboxPartitionNumber == null)
								{
									boxedMailboxPartitionNumber = mailboxToUse.MailboxPartitionNumber;
								}
							}
							if (rowFound && !reader.Interrupted)
							{
								ConversationItem conversationItem2 = ConversationItem.OpenConversationItem(contextProvider.CurrentContext, mailboxToUse, reader);
								conversationQueue.Enqueue(conversationItem2);
								messageFidMids.AddRange(conversationItem2.FidMidList);
							}
							if (conversationQueue.Count != 0 && (!rowFound || conversationQueue.Count >= conversationPrereadChunkSize || messageFidMids.Count >= maxConversationMessagesToPreread || reader.Interrupted))
							{
								if (messageFidMids.Count > 1)
								{
									keys.Clear();
									foreach (FidMid fidMid in messageFidMids)
									{
										StartStopKey startStopKey = new StartStopKey(true, new object[]
										{
											boxedMailboxPartitionNumber,
											fidMid.FolderId.To26ByteArray(),
											boxedFalse,
											fidMid.MessageId.To26ByteArray()
										});
										keys.Add(new KeyRange(startStopKey, startStopKey));
									}
									using (PreReadOperator preReadOperator = Factory.CreatePreReadOperator(contextProvider.CurrentContext.Culture, contextProvider.CurrentContext, messageTable.Table, messageTable.MessageUnique, keys, null, true))
									{
										preReadOperator.ExecuteScalar();
									}
								}
								messageFidMids.Clear();
								while (conversationQueue.Count != 0)
								{
									using (ConversationItem conversationItem = conversationQueue.Dequeue())
									{
										ConversationMembers conversationMembers = conversationItem.GetConversationMembers(contextProvider.CurrentContext, null, aggregatePropertiesToCompute);
										if (isSearchFolder)
										{
											if (searchFolderToUse == null)
											{
												searchFolderToUse = (SearchFolder)Folder.OpenFolder(contextProvider.CurrentContext, mailboxToUse, folderId);
											}
											List<FidMid> conversationMessages = conversationMembers.ConversationMessages.ToList<FidMid>();
											ICollection<FidMid> filterList = searchFolderToUse.FilterMessages(contextProvider.CurrentContext, searchFolderBaseViewIndex, conversationMessages, new bool?(false));
											yield return new ConversationValueBag(conversationItem, null, filterList, conversationMembers);
										}
										else
										{
											yield return new ConversationValueBag(conversationItem, folderId, conversationMembers);
										}
									}
								}
							}
							if (rowFound && reader.Interrupted)
							{
								yield return null;
								mailboxToUse = null;
								searchFolderToUse = null;
							}
						}
						while (rowFound);
					}
				}
			}
			finally
			{
				while (conversationQueue.Count > 0)
				{
					ConversationItem conversationItem3 = conversationQueue.Dequeue();
					conversationItem3.Dispose();
				}
			}
			yield break;
		}

		private IChunked PrepareIndex(Context context, LogicalIndex logicalIndexToPopulate)
		{
			bool arg;
			bool arg2;
			IChunked result = logicalIndexToPopulate.PrepareIndex(context, new GetColumnValueBagsEnumeratorDelegate(this.GetColumnValueBagsEnumerator), out arg, out arg2);
			if (LogicalIndex.IndexUseCallbackTestHook != null)
			{
				LogicalIndex.IndexUseCallbackTestHook(logicalIndexToPopulate, arg, arg2);
			}
			return result;
		}

		private IEnumerable<IColumnValueBag> GetColumnValueBagsEnumerator(IContextProvider contextProvider, IEnumerable<Column> requiredColumns, IInterruptControl interruptControl, out LogicalIndex baseViewIndex)
		{
			baseViewIndex = null;
			Folder folder = Folder.OpenFolder(contextProvider.CurrentContext, base.Mailbox, this.FolderId);
			if (folder == null || 0L == folder.GetMessageCount(contextProvider.CurrentContext))
			{
				return null;
			}
			MessageTable messageTable = this.messageTable;
			SearchCriteria searchCriteria = Factory.CreateSearchCriteriaCompare(messageTable.ConversationDocumentId, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(null, messageTable.ConversationDocumentId));
			SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition;
			if (this.IsSearchFolder)
			{
				baseViewIndex = LogicalIndexCache.GetLogicalIndex(contextProvider.CurrentContext, base.Mailbox, this.folderId, this.SearchFolder.GetLogicalIndexNumber(contextProvider.CurrentContext).Value);
				outerQueryDefinition = this.SearchFolder.BaseViewOperatorDefinition(contextProvider.CurrentContext, base.Mailbox, new PhysicalColumn[]
				{
					messageTable.MailboxPartitionNumber,
					messageTable.ConversationDocumentId
				}, searchCriteria, new bool?(false));
			}
			else
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					base.Mailbox.MailboxPartitionNumber,
					this.folderId.To26ByteArray(),
					false
				});
				outerQueryDefinition = new TableOperator.TableOperatorDefinition(contextProvider.CurrentContext.Culture, messageTable.Table, messageTable.MessageUnique, new PhysicalColumn[]
				{
					messageTable.MailboxPartitionNumber,
					messageTable.ConversationDocumentId
				}, null, searchCriteria, null, 0, 0, new KeyRange[]
				{
					new KeyRange(startStopKey, startStopKey)
				}, false, true, true);
			}
			SimpleQueryOperator.SimpleQueryOperatorDefinition outerQueryDefinition2 = new DistinctOperator.DistinctOperatorDefinition(0, 0, outerQueryDefinition, true);
			IList<Column> list = new List<Column>(messageTable.Table.CommonColumns);
			IList<Column> columns = messageTable.Table.PrimaryKeyIndex.Columns;
			for (int i = 0; i < columns.Count; i++)
			{
				Column item = columns[i];
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			JoinOperator.JoinOperatorDefinition selectConversationObjectsOperatorDefinition = new JoinOperator.JoinOperatorDefinition(contextProvider.CurrentContext.Culture, messageTable.Table, list, null, null, null, 0, 0, columns, outerQueryDefinition2, true);
			return ConversationViewTable.ColumnValueBagsEnumeratorImpl(contextProvider, selectConversationObjectsOperatorDefinition, base.Mailbox.MailboxNumber, this.folderId, this.IsSearchFolder, baseViewIndex, requiredColumns, interruptControl, ConversationViewTable.conversationPrereadChunkSize, ConversationViewTable.maxConversationMessagesToPreread);
		}

		private SortOrder GetMasterIndexSortOrder()
		{
			return new SortOrder(new Column[]
			{
				this.messageTable.MessageDocumentId
			}, new bool[]
			{
				true
			});
		}

		private LogicalIndex GetConversationIndex(Context context, SearchCriteria findRowCriteria, out LogicalIndex masterIndex)
		{
			HashSet<Column> hashSet = (base.ViewColumns != null) ? new HashSet<Column>(base.ViewColumns) : new HashSet<Column>();
			Action<Column, object> callback = delegate(Column c, object collection)
			{
				((ICollection<Column>)collection).Add(c);
			};
			if (base.ImplicitCriteria != null)
			{
				base.ImplicitCriteria.EnumerateColumns(callback, hashSet);
			}
			if (base.RestrictCriteria != null)
			{
				base.RestrictCriteria.EnumerateColumns(callback, hashSet);
			}
			if (findRowCriteria != null)
			{
				findRowCriteria.EnumerateColumns(callback, hashSet);
			}
			foreach (Column item in base.SortOrder.Columns)
			{
				hashSet.Add(item);
			}
			hashSet.RemoveWhere((Column c) => c.ActualColumn is ConstantColumn);
			hashSet.RemoveWhere(new Predicate<Column>(ConversationViewTable.IsInstanceNumber));
			Column item2 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMvFrom);
			Column item3 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMvTo);
			if (hashSet.Contains(item2))
			{
				hashSet.Add(item3);
			}
			else if (hashSet.Contains(item3))
			{
				hashSet.Add(item2);
			}
			bool flag = ConfigurationSchema.EnableConversationMasterIndex.Value && context.Database.PhysicalDatabase.DatabaseType != DatabaseType.Sql;
			bool flag2 = flag && ConfigurationSchema.ForceConversationMasterIndexUpgrade.Value && context.ClientType != ClientType.Transport;
			List<IIndex> list = null;
			if (!flag2)
			{
				list = LogicalIndexCache.GetIndexesInScope(context, base.Mailbox, this.folderId, LogicalIndexType.Conversations, null, false, base.SortOrder, this.ReplaceMviColumns(hashSet.Except(base.SortOrder.Columns.Where(new Func<Column, bool>(ConversationViewTable.IsNotTruncated))), false).Distinct<Column>().ToArray<Column>(), null, base.Table, true, flag);
			}
			if (flag)
			{
				bool existingOnly = list != null;
				SortOrder masterIndexSortOrder = this.GetMasterIndexSortOrder();
				IEnumerable<Column> source = this.ReplaceMviColumns(hashSet.Except(masterIndexSortOrder.Columns), true).Distinct<Column>();
				List<IIndex> indexesInScope = LogicalIndexCache.GetIndexesInScope(context, base.Mailbox, this.folderId, LogicalIndexType.Conversations, null, false, masterIndexSortOrder, source.ToArray<Column>(), null, base.Table, true, existingOnly);
				List<IIndex> indexesInScope2 = LogicalIndexCache.GetIndexesInScope(context, base.Mailbox, this.folderId, LogicalIndexType.Conversations, null, false, base.SortOrder, null, null, base.Table, true, existingOnly);
				if (indexesInScope != null && indexesInScope2 != null)
				{
					masterIndex = (LogicalIndex)indexesInScope[0];
					return (LogicalIndex)indexesInScope2[0];
				}
			}
			masterIndex = null;
			return (LogicalIndex)list[0];
		}

		protected override IReadOnlyDictionary<Column, Column> GetColumnRenames(Context context)
		{
			if (this.renameDictionary == null)
			{
				if (this.IsOptimizedInstantSearch)
				{
					SearchResultsTableFunction searchResultsTableFunction = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.SearchResultsTableFunction(base.Mailbox.Database);
					this.renameDictionary = new Dictionary<Column, Column>();
					Column column = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.Fid);
					ExchangeId conversationFolderId = ConversationItem.GetConversationFolderId(context, base.Mailbox);
					this.renameDictionary[column] = Factory.CreateConstantColumn(conversationFolderId.ToLong(), column);
					Column key = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.Mid);
					Column key2 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.InstanceId);
					Column value = Factory.CreateFunctionColumn("Mid", typeof(long), 8, 0, base.Table, delegate(object[] args)
					{
						if (args == null || args.Length < 1 || args[0] == null)
						{
							return null;
						}
						return ExchangeId.Create(conversationFolderId.Guid, (ulong)((long)((int)args[0])), conversationFolderId.Replid).ToLong();
					}, "FabricateMid", new Column[]
					{
						searchResultsTableFunction.SortPosition
					});
					this.renameDictionary[key] = value;
					this.renameDictionary[key2] = value;
					Column key3 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationTopic);
					Column value2 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.NormalizedSubject);
					this.renameDictionary[key3] = value2;
					Column key4 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMvFrom);
					Column column2 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.SentRepresentingName);
					this.renameDictionary[key4] = Factory.CreateFunctionColumn("ConversationMvFrom", typeof(string[]), PropertyTypeHelper.SizeFromPropType(PropertyType.MVUnicode), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.MVUnicode), base.Table, delegate(object[] args)
					{
						if (args != null && args.Length >= 1 && args[0] != null)
						{
							return new string[]
							{
								(string)args[0]
							};
						}
						return null;
					}, "ComputeConversationMvFrom", new Column[]
					{
						column2
					});
					Column key5 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMvTo);
					Column column3 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.DisplayTo);
					this.renameDictionary[key5] = Factory.CreateFunctionColumn("ConversationMvTo", typeof(string[]), PropertyTypeHelper.SizeFromPropType(PropertyType.MVUnicode), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.MVUnicode), base.Table, delegate(object[] args)
					{
						if (args == null || args.Length < 1 || args[0] == null)
						{
							return null;
						}
						string[] array = ((string)args[0]).Split(ConversationMembers.DisplayToSeparators, StringSplitOptions.RemoveEmptyEntries);
						if (array.Length == 0)
						{
							return null;
						}
						if (array.Length > 100)
						{
							Array.Resize<string>(ref array, 100);
						}
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i].Length > 255)
							{
								array[i] = array[i].Substring(0, 255);
							}
						}
						return array;
					}, "ComputeConversationMvTo", new Column[]
					{
						column3
					});
					Column key6 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMessageDeliveryTime);
					Column key7 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMessageDeliveryTimeMailboxWide);
					Column value3 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.MessageDeliveryTime);
					this.renameDictionary[key6] = value3;
					this.renameDictionary[key7] = value3;
					Column column4 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationCategories);
					this.renameDictionary[column4] = Factory.CreateConstantColumn(null, column4);
					Column key8 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationFlagStatus);
					Column value4 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.FlagStatus);
					this.renameDictionary[key8] = value4;
					Column key9 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationHasAttach);
					Column value5 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.HasAttach);
					this.renameDictionary[key9] = value5;
					Column key10 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationContentCount);
					Column key11 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationContentCountMailboxWide);
					this.renameDictionary[key10] = searchResultsTableFunction.Count;
					this.renameDictionary[key11] = searchResultsTableFunction.Count;
					Column key12 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationContentUnread);
					Column key13 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationContentUnreadMailboxWide);
					Column value6 = Factory.CreateFunctionColumn("ConversationContentUnread", typeof(int), PropertyTypeHelper.SizeFromPropType(PropertyType.Int32), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.Int32), base.Table, (object[] args) => (args == null || args.Length < 1 || !(bool)args[0]) ? 1 : 0, "ComputeConversationContentUnread", new Column[]
					{
						this.messageTable.IsRead
					});
					this.renameDictionary[key12] = value6;
					this.renameDictionary[key13] = value6;
					Column key14 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMessageSize);
					Column value7 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.MessageSize32);
					this.renameDictionary[key14] = value7;
					Column key15 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMessageClasses);
					this.renameDictionary[key15] = Factory.CreateFunctionColumn("ConversationMessageClasses", typeof(string[]), PropertyTypeHelper.SizeFromPropType(PropertyType.MVUnicode), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.MVUnicode), base.Table, delegate(object[] args)
					{
						if (args != null && args.Length >= 1 && args[0] != null)
						{
							return new string[]
							{
								(string)args[0]
							};
						}
						return null;
					}, "ComputeConversationMessageClasses", new Column[]
					{
						this.messageTable.MessageClass
					});
					Column key16 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationImportance);
					Column value8 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.Importance);
					this.renameDictionary[key16] = value8;
					Column key17 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMvItemIds);
					Column key18 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationMvItemIdsMailboxWide);
					Column value9 = Factory.CreateFunctionColumn("ConversationMvItemIds", typeof(byte[][]), PropertyTypeHelper.SizeFromPropType(PropertyType.MVBinary), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.MVBinary), base.Table, delegate(object[] args)
					{
						if (args != null && args.Length >= 2)
						{
							return new byte[][]
							{
								ConversationMembers.BuildFidMid(ExchangeId.CreateFrom26ByteArray(null, null, (byte[])args[0]), ExchangeId.CreateFrom26ByteArray(null, null, (byte[])args[1]))
							};
						}
						return null;
					}, "ComputeConversationMvItemIds", new Column[]
					{
						this.messageTable.FolderId,
						this.messageTable.MessageId
					});
					this.renameDictionary[key17] = value9;
					this.renameDictionary[key18] = value9;
					Column key19 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationLastMemberDocumentId);
					Column value10 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.DocumentId);
					this.renameDictionary[key19] = value10;
					Column column5 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationPredictedActionsSummary);
					this.renameDictionary[column5] = Factory.CreateConstantColumn(0, column5);
					Column column6 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationGroupingActions);
					this.renameDictionary[column6] = Factory.CreateConstantColumn(null, column6);
					Column key20 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationHasIrm);
					Column value11 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.IsIRMMessage);
					this.renameDictionary[key20] = value11;
					Column key21 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationReplyForwardState);
					Column key22 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.ConversationReplyForwardStateMailboxWide);
					Column value12 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.IconIndex);
					this.renameDictionary[key21] = value12;
					this.renameDictionary[key22] = value12;
				}
				else if (!base.MvExplosion && base.ViewColumns != null)
				{
					Column column7 = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.InstanceNum);
					this.renameDictionary = new Dictionary<Column, Column>
					{
						{
							column7,
							Factory.CreateConstantColumn(0, column7)
						}
					};
				}
			}
			return this.renameDictionary;
		}

		protected override void BringIndexesToCurrent(Context context, IList<IIndex> indexList, DataAccessOperator queryPlan)
		{
			if (this.IsOptimizedInstantSearch)
			{
				return;
			}
			if (indexList != null)
			{
				List<DataAccessOperator> planOperators = new List<DataAccessOperator>();
				queryPlan.GetDescendants(planOperators);
				foreach (IIndex index in indexList)
				{
					LogicalIndex logicalIndex = (LogicalIndex)index;
					if (MessageViewTable.PlanUsesIndex(context, planOperators, logicalIndex))
					{
						bool arg;
						bool arg2;
						logicalIndex.UpdateIndex(context, new GetColumnValueBagsEnumeratorDelegate(this.GetColumnValueBagsEnumerator), out arg, out arg2);
						if (LogicalIndex.IndexUseCallbackTestHook != null)
						{
							LogicalIndex.IndexUseCallbackTestHook(logicalIndex, arg, arg2);
						}
					}
				}
			}
		}

		private IEnumerable<Column> ReplaceMviColumns(IEnumerable<Column> columns, bool replaceInstanceKey)
		{
			foreach (Column column in columns)
			{
				ExtendedPropertyColumn baseMultiValueColumn;
				if (PropertySchema.IsMultiValueInstanceColumn(column, out baseMultiValueColumn))
				{
					yield return baseMultiValueColumn;
				}
				else if (replaceInstanceKey && ConversationViewTable.IsInstanceKey(column))
				{
					yield return PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.Fid);
					yield return PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Conversation, PropTag.Message.Mid);
				}
				else
				{
					yield return column;
				}
			}
			yield break;
		}

		private bool SkipPropertiesPromotionValidation(Context context, ViewSetColumnsFlag flags)
		{
			if (!this.folderId.IsValid)
			{
				return true;
			}
			if ((flags & ViewSetColumnsFlag.NoColumnValidation) == ViewSetColumnsFlag.NoColumnValidation)
			{
				return true;
			}
			if (this.expandedMembersOnly)
			{
				if (context.TestCaseId.IsNotNull && ConversationViewTable.expandedConversationViewValidationTestCases.Contains(context.TestCaseId))
				{
					return false;
				}
			}
			else
			{
				if (context.TestCaseId.IsNotNull && ConversationViewTable.conversationViewValidationTestCases.Contains(context.TestCaseId))
				{
					return false;
				}
				if (ViewTable.ClientTypeExcludedFromDefaultPromotedValidation(context.ClientType))
				{
					return true;
				}
			}
			return false;
		}

		private IList<ConversationMembersBlob> BuildExpandedConversationMembersBlob(Context context, Reader reader, bool allRows)
		{
			if (reader == null)
			{
				return new List<ConversationMembersBlob>(0);
			}
			int num = 0;
			List<ConversationMembersBlob> list = new List<ConversationMembersBlob>();
			while (reader.Read())
			{
				byte[] binary = reader.GetBinary(this.messageTable.ConversationMembers);
				if (binary != null)
				{
					int num2 = 0;
					List<FidMid> list2 = FidMidListSerializer.FromBytes(binary, ref num2, base.Mailbox.ReplidGuidMap);
					for (int i = 0; i < list2.Count; i++)
					{
						list.Add(new ConversationMembersBlob(list2[i].FolderId.To26ByteArray(), list2[i].MessageId.To26ByteArray(), num));
						num++;
					}
					if (!allRows)
					{
						break;
					}
				}
			}
			return list;
		}

		private const int MaximumNumberOfRowsForExpandedConversationView = 200;

		private const int DefaultConversationPrereadChunkSize = 10;

		private const int DefaultMaxConversationMessagesToPreread = 150;

		private static readonly TestCaseId[] expandedConversationViewValidationTestCases = new TestCaseId[]
		{
			TestCaseId.ExpandedConversationViewTestCaseId
		};

		private static readonly TestCaseId[] conversationViewValidationTestCases = new TestCaseId[0];

		private static int conversationPrereadChunkSize = 10;

		private static int maxConversationMessagesToPreread = 150;

		private readonly MessageTable messageTable;

		private readonly ExchangeId folderId;

		private readonly Folder folder;

		private readonly bool useIndexForEmptyFolder;

		private readonly bool expandedMembersOnly;

		private IList<Column> conversationMembersViewColumns;

		private ConversationMessageViewTable conversationMessageViewTable;

		private Dictionary<Column, Column> renameDictionary;

		private SearchCriteria findRowCriteria;

		private SearchCriteria rewrittenFindRowCriteria;
	}
}
