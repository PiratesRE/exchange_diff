using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ConversationMessageViewTable : ViewTable
	{
		public ConversationMessageViewTable(Context context, Mailbox mailbox, ExchangeId folderId, IList<ConversationMembersBlob> conversationMembersList) : base(mailbox, DatabaseSchema.MessageTable(mailbox.Database).Table)
		{
			this.folderId = folderId;
			this.messageTable = DatabaseSchema.MessageTable(mailbox.Database);
			this.conversationMembersTableFunction = DatabaseSchema.ConversationMembersBlobTableFunction(base.Mailbox.Database);
			this.conversationMembersList = conversationMembersList;
			base.SetImplicitCriteria(Factory.CreateSearchCriteriaTrue());
			this.SortTable(SortOrder.Empty);
		}

		public ConversationMessageViewTable(Context context, Mailbox mailbox, ExchangeId folderId) : this(context, mailbox, folderId, null)
		{
		}

		public ConversationMessageViewTable(Context context, Mailbox mailbox, ExchangeId folderId, IList<Column> columns, SortOrder sortOrder, SearchCriteria criteria) : this(context, mailbox, folderId)
		{
			base.SetColumns(context, columns);
			this.SortTable(sortOrder);
			this.Restrict(context, criteria);
		}

		public byte[] ConversationId
		{
			get
			{
				return this.conversationId;
			}
		}

		internal ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		protected override Index LogicalKeyIndex
		{
			get
			{
				return this.conversationMembersTableFunction.TableFunction.PrimaryKeyIndex;
			}
		}

		protected override bool MustUseLazyIndex
		{
			get
			{
				return true;
			}
		}

		public override IList<Column> LongValueColumnsToPreread
		{
			get
			{
				return this.longValueColumnsToPreread;
			}
		}

		private bool ExpandedConversationMembersView
		{
			get
			{
				return this.conversationMembersList != null;
			}
		}

		public override void Restrict(Context context, SearchCriteria restrictCriteria)
		{
			this.conversationId = ConversationMessageViewTable.GetConversationIdFromRestriction(restrictCriteria);
			this.conversationDocumentId = ConversationItem.GetConversationDocumentId(context, base.Mailbox, this.conversationId);
			base.InvalidateBookmarkAndRowCount();
		}

		public override int GetRowCount(Context context)
		{
			if (this.ExpandedConversationMembersView)
			{
				if (this.conversationMembersList != null)
				{
					return this.conversationMembersList.Count;
				}
				return 0;
			}
			else
			{
				if (this.conversationId == null)
				{
					return 0;
				}
				return base.GetRowCount(context);
			}
		}

		public override void SetColumns(Context context, IList<Column> columns, ViewSetColumnsFlag flags)
		{
			if (!this.SkipPropertiesPromotionValidation(context, flags))
			{
				Folder folder = Folder.OpenFolder(context, base.Mailbox, this.folderId);
				if (folder == null)
				{
					throw new StoreException((LID)47616U, ErrorCodeValue.InvalidObject);
				}
				PropertyPromotionHelper.ValidatePropertiesPromotion(context, base.Mailbox, folder.GetName(context), columns);
			}
			if (columns != null)
			{
				foreach (Column column in columns)
				{
					ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
					if (!(extendedPropertyColumn == null) && (extendedPropertyColumn.StorePropTag.PropTag == 1071185951U || extendedPropertyColumn.StorePropTag.PropTag == 1071120415U))
					{
						this.longValueColumnsToPreread = new List<Column>
						{
							this.messageTable.OffPagePropertyBlob
						};
						break;
					}
				}
			}
			base.SetColumns(context, columns, flags);
		}

		protected override IReadOnlyDictionary<Column, Column> GetColumnRenames(Context context)
		{
			Dictionary<Column, Column> dictionary = new Dictionary<Column, Column>(3);
			dictionary[this.messageTable.VirtualIsRead] = this.messageTable.IsRead;
			dictionary[this.messageTable.VirtualParentDisplay] = TopMessage.CreateVirtualParentDisplayFunctionColumn(this.messageTable, new Func<object[], object>(this.GetParentDisplayColumnFunction));
			dictionary[this.messageTable.VirtualUnreadMessageCount] = PropertySchema.MapToColumn(base.Mailbox.Database, ObjectType.Message, PropTag.Message.UnreadCountInt64);
			return dictionary;
		}

		protected internal override IList<IIndex> GetInScopePseudoIndexes(Context context, SearchCriteria findRowCriteria, out IList<IIndex> masterIndexes)
		{
			masterIndexes = null;
			IList<ConversationMembersBlob> conversationMembers = this.GetConversationMembers(context);
			if (conversationMembers != null && conversationMembers.Count > 2)
			{
				List<KeyRange> list = new List<KeyRange>(conversationMembers.Count);
				for (int i = 0; i < conversationMembers.Count; i++)
				{
					ConversationMembersBlob conversationMembersBlob = conversationMembers[i];
					StartStopKey startStopKey = new StartStopKey(true, new object[]
					{
						base.Mailbox.MailboxPartitionNumber,
						conversationMembersBlob.FolderId,
						false,
						conversationMembersBlob.MessageId
					});
					list.Add(new KeyRange(startStopKey, startStopKey));
				}
				using (PreReadOperator preReadOperator = Factory.CreatePreReadOperator(context.Culture, context, this.messageTable.Table, this.messageTable.MessageUnique, list, null, true))
				{
					preReadOperator.ExecuteScalar();
				}
			}
			object obj = ConversationMembersBlob.Serialize(conversationMembers);
			return new List<IIndex>(1)
			{
				new SimplePseudoIndex(this.messageTable.Table, this.conversationMembersTableFunction.TableFunction, new object[]
				{
					obj
				}, this.conversationMembersTableFunction.TableFunction.PrimaryKeyIndex.SortOrder, new Dictionary<Column, Column>(4)
				{
					{
						this.messageTable.MailboxPartitionNumber,
						Factory.CreateConstantColumn(base.Mailbox.MailboxPartitionNumber, this.messageTable.MailboxPartitionNumber)
					},
					{
						this.messageTable.FolderId,
						this.conversationMembersTableFunction.FolderId
					},
					{
						this.messageTable.MessageId,
						this.conversationMembersTableFunction.MessageId
					},
					{
						this.messageTable.IsHidden,
						Factory.CreateConstantColumn(false, this.messageTable.IsHidden)
					}
				}, null, true)
			};
		}

		public int? GetConversationDocumentId(Context context)
		{
			if (this.conversationDocumentId == null)
			{
				this.conversationDocumentId = ConversationItem.GetConversationDocumentId(context, base.Mailbox, this.conversationId);
			}
			return this.conversationDocumentId;
		}

		private static byte[] GetConversationIdFromRestriction(SearchCriteria restriction)
		{
			SearchCriteriaCompare searchCriteriaCompare = restriction as SearchCriteriaCompare;
			if (searchCriteriaCompare != null && searchCriteriaCompare.RelOp == SearchCriteriaCompare.SearchRelOp.Equal)
			{
				ConstantColumn constantColumn = (searchCriteriaCompare.Lhs as ConstantColumn) ?? (searchCriteriaCompare.Rhs as ConstantColumn);
				ExtendedPropertyColumn extendedPropertyColumn = (searchCriteriaCompare.Lhs as ExtendedPropertyColumn) ?? (searchCriteriaCompare.Rhs as ExtendedPropertyColumn);
				if (null != constantColumn && null != extendedPropertyColumn && PropTag.Message.ConversationId == extendedPropertyColumn.StorePropTag)
				{
					return constantColumn.Value as byte[];
				}
			}
			throw new StoreException((LID)63096U, ErrorCodeValue.TooComplex);
		}

		private IList<ConversationMembersBlob> GetConversationMembers(Context context)
		{
			if (this.ExpandedConversationMembersView)
			{
				return this.conversationMembersList;
			}
			if (this.conversationId == null)
			{
				throw new StoreException((LID)36935U, ErrorCodeValue.NotInitialized);
			}
			List<ConversationMembersBlob> list = null;
			using (ConversationItem conversationItem = ConversationItem.OpenConversationItem(context, base.Mailbox, this.conversationId))
			{
				if (conversationItem != null)
				{
					byte[] fidMidBlob = conversationItem.GetFidMidBlob();
					int num = 0;
					List<FidMid> list2 = FidMidListSerializer.FromBytes(fidMidBlob, ref num, base.Mailbox.ReplidGuidMap);
					if (list2 != null)
					{
						list = new List<ConversationMembersBlob>(list2.Count);
						for (int i = 0; i < list2.Count; i++)
						{
							list.Add(new ConversationMembersBlob(list2[i].FolderId.To26ByteArray(), list2[i].MessageId.To26ByteArray(), i));
						}
					}
				}
			}
			if (list == null)
			{
				list = new List<ConversationMembersBlob>(0);
			}
			return list;
		}

		private object GetParentDisplayColumnFunction(object[] columnValues)
		{
			Context currentOperationContext = base.Mailbox.CurrentOperationContext;
			Folder folder = null;
			ExchangeId id = ExchangeId.CreateFrom26ByteArray(currentOperationContext, base.Mailbox.ReplidGuidMap, (byte[])columnValues[0]);
			if (id.IsValid)
			{
				folder = Folder.OpenFolder(currentOperationContext, base.Mailbox, id);
			}
			if (folder != null)
			{
				return folder.GetName(currentOperationContext);
			}
			return null;
		}

		private bool SkipPropertiesPromotionValidation(Context context, ViewSetColumnsFlag flags)
		{
			return !this.folderId.IsValid || (flags & ViewSetColumnsFlag.NoColumnValidation) == ViewSetColumnsFlag.NoColumnValidation || ((!context.TestCaseId.IsNotNull || !ConversationMessageViewTable.conversationMembersViewValidationTestCases.Contains(context.TestCaseId)) && ViewTable.ClientTypeExcludedFromDefaultPromotedValidation(context.ClientType));
		}

		private static readonly TestCaseId[] conversationMembersViewValidationTestCases = new TestCaseId[0];

		private readonly MessageTable messageTable;

		private readonly ConversationMembersBlobTableFunction conversationMembersTableFunction;

		private readonly IList<ConversationMembersBlob> conversationMembersList;

		private readonly ExchangeId folderId;

		private byte[] conversationId;

		private int? conversationDocumentId;

		private List<Column> longValueColumnsToPreread;
	}
}
