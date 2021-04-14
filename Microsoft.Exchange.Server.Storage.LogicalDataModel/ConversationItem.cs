using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ConversationItem : TopMessage
	{
		private ConversationItem(Context context, Mailbox mailbox, Folder folder, byte[] conversationId, TopMessage message) : base(context, mailbox, folder, true, null, null, ExchangeId.Null)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.fidMidList = new List<FidMid>(0);
				this.SetFidMidList(context);
				base.SetColumn(context, base.MessageTable.ConversationId, conversationId);
				base.SetColumn(context, base.MessageTable.CodePage, null);
				this.SetProperty(context, PropTag.Message.SearchKey, null);
				this.SetProperty(context, PropTag.Message.DisplayBcc, null);
				this.SetProperty(context, PropTag.Message.DisplayCc, null);
				this.SetProperty(context, PropTag.Message.DisplayTo, null);
				int num = (message != null) ? TopMessage.GetNewMessageDocumentId(context, message.GetMessageClass(context), message.ParentFolder) : TopMessage.GetNewMessageDocumentId(context, null, folder);
				base.SetColumn(context, base.MessageTable.MessageDocumentId, num);
				disposeGuard.Success();
			}
		}

		private ConversationItem(Context context, Mailbox mailbox, int documentId) : base(context, mailbox, null, false, documentId, null, ExchangeId.Null)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.PossiblyLoadFidMidList(context);
				disposeGuard.Success();
			}
		}

		private ConversationItem(Context context, Mailbox mailbox, Reader reader) : base(context, mailbox, null, reader)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.PossiblyLoadFidMidList(context);
				disposeGuard.Success();
			}
		}

		public override bool IsConversation
		{
			get
			{
				return true;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.fidMidList.Count == 0;
			}
		}

		public bool IsMaxSize
		{
			get
			{
				return this.fidMidList.Count >= 300;
			}
		}

		public TopMessage ModifiedMessage { get; set; }

		public ModifiedSearchFolders ModifiedSearchFolders { get; set; }

		public IList<FidMid> FidMidList
		{
			get
			{
				return this.fidMidList;
			}
		}

		public static ConversationItem CreateConversationItem(Context context, Mailbox mailbox, byte[] conversationId, TopMessage message)
		{
			Folder folder = Folder.OpenFolder(context, mailbox, ConversationItem.GetConversationFolderId(context, mailbox));
			return new ConversationItem(context, mailbox, folder, conversationId, message);
		}

		public static ConversationItem OpenConversationItem(Context context, Mailbox mailbox, Reader reader)
		{
			return new ConversationItem(context, mailbox, reader);
		}

		public static IEnumerable<ConversationItem> OpenConversations(Context context, Mailbox mailbox, IEnumerable<byte[]> conversationIds)
		{
			MessageTable messageTable = DatabaseSchema.MessageTable(mailbox.Database);
			object mailboxPartitionNumber = mailbox.MailboxPartitionNumber;
			int numConversations = conversationIds.Count<byte[]>();
			IList<KeyRange> keys = new List<KeyRange>(numConversations);
			foreach (byte[] array in conversationIds)
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					mailboxPartitionNumber,
					array
				});
				keys.Add(new KeyRange(startStopKey, startStopKey));
			}
			SimpleQueryOperator query = ConversationItem.GetConversationsOperator(context, mailbox, messageTable.ConversationIdUnique, keys);
			using (Reader reader = query.ExecuteReader(true))
			{
				while (reader.Read())
				{
					yield return new ConversationItem(context, mailbox, reader);
				}
			}
			yield break;
		}

		public static ConversationItem OpenConversationItem(Context context, Mailbox mailbox, byte[] conversationId)
		{
			MessageTable messageTable = DatabaseSchema.MessageTable(mailbox.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				conversationId
			});
			IList<KeyRange> keys = new List<KeyRange>(1)
			{
				new KeyRange(startStopKey, startStopKey)
			};
			SimpleQueryOperator conversationsOperator = ConversationItem.GetConversationsOperator(context, mailbox, messageTable.ConversationIdUnique, keys);
			using (Reader reader = conversationsOperator.ExecuteReader(true))
			{
				if (reader.Read())
				{
					return new ConversationItem(context, mailbox, reader);
				}
			}
			return null;
		}

		public static ConversationItem OpenConversationItem(Context context, Mailbox mailbox, ExchangeId mid)
		{
			MessageTable messageTable = DatabaseSchema.MessageTable(mailbox.Database);
			ExchangeId conversationFolderId = ConversationItem.GetConversationFolderId(context, mailbox);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				conversationFolderId.To26ByteArray(),
				false,
				mid.To26ByteArray()
			});
			IList<KeyRange> keys = new List<KeyRange>(1)
			{
				new KeyRange(startStopKey, startStopKey)
			};
			SimpleQueryOperator conversationsOperator = ConversationItem.GetConversationsOperator(context, mailbox, messageTable.MessageUnique, keys);
			using (Reader reader = conversationsOperator.ExecuteReader(true))
			{
				if (reader.Read())
				{
					return new ConversationItem(context, mailbox, reader);
				}
			}
			return null;
		}

		public static SimpleQueryOperator GetConversationsOperator(Context context, Mailbox mailbox, Index index, IList<KeyRange> keys)
		{
			MessageTable messageTable = DatabaseSchema.MessageTable(mailbox.Database);
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
			return Factory.CreateTableOperator(context.Culture, context, messageTable.Table, index, list, null, null, 0, 0, keys, false, true);
		}

		public static ConversationItem OpenConversationItem(Context context, Mailbox mailbox, int documentId)
		{
			ConversationItem conversationItem = new ConversationItem(context, mailbox, documentId);
			if (conversationItem.IsDead)
			{
				conversationItem.Dispose();
				conversationItem = null;
			}
			return conversationItem;
		}

		public static ExchangeId GetConversationFolderId(Context context, Mailbox mailbox)
		{
			if (mailbox.ConversationFolderId.IsNullOrZero)
			{
				mailbox.ConversationFolderId = SpecialFoldersCache.GetSpecialFolders(context, mailbox)[20];
			}
			return mailbox.ConversationFolderId;
		}

		public static int? GetConversationDocumentId(Context context, Mailbox mailbox, byte[] conversationId)
		{
			int? result = null;
			MessageTable messageTable = DatabaseSchema.MessageTable(mailbox.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				conversationId
			});
			TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, messageTable.Table, messageTable.ConversationIdUnique, new Column[]
			{
				messageTable.MessageDocumentId
			}, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true);
			using (Reader reader = tableOperator.ExecuteReader(true))
			{
				if (reader.Read())
				{
					result = new int?(reader.GetInt32(messageTable.MessageDocumentId));
				}
			}
			return result;
		}

		public byte[] GetFidMidBlob()
		{
			return FidMidListSerializer.ToBytes(this.fidMidList);
		}

		public ConversationMembers GetConversationMembers(Context context, TopMessage modifiedMessage, HashSet<StorePropTag> aggregatePropertiesToCompute)
		{
			return new ConversationMembers(base.Mailbox, this.fidMidList, this.GetOriginalFidMids(context), modifiedMessage, aggregatePropertiesToCompute);
		}

		public void AddMessage(Context context, FidMid fidMid)
		{
			if (ExTraceGlobals.ConversationsDetailedTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ConversationsDetailedTracer.TraceDebug<FidMid, int>(0L, "Adding message ({0}) to ConversationItem, which has {1} messages", fidMid, this.fidMidList.Count);
			}
			List<FidMid> list = new List<FidMid>(this.fidMidList);
			int num = list.BinarySearch(fidMid);
			if (num < 0)
			{
				list.Insert(~num, fidMid);
			}
			else
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Conversation already contains this message");
			}
			this.fidMidList = list;
			this.SetFidMidList(context);
		}

		public void RemoveMessage(Context context, FidMid fidMid)
		{
			if (ExTraceGlobals.ConversationsDetailedTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ConversationsDetailedTracer.TraceDebug<FidMid, int>(0L, "Removing message ({0}) from ConversationItem, which has {1} messages", fidMid, this.fidMidList.Count);
			}
			List<FidMid> list = new List<FidMid>(this.fidMidList);
			int num = list.BinarySearch(fidMid);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
			else
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Conversation does not contain this message");
			}
			this.fidMidList = list;
			this.SetFidMidList(context);
		}

		internal static IEnumerable<int> GetConversationDocids(Context context, Mailbox mailbox, IEnumerable<byte[]> conversationIds)
		{
			MessageTable messageTable = DatabaseSchema.MessageTable(mailbox.Database);
			int num = conversationIds.Count<byte[]>();
			IList<KeyRange> list = new List<KeyRange>(num);
			foreach (byte[] array in conversationIds)
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					mailbox.MailboxPartitionNumber,
					array
				});
				list.Add(new KeyRange(startStopKey, startStopKey));
			}
			List<int> list2 = new List<int>(num);
			TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, messageTable.Table, messageTable.ConversationIdUnique, new PhysicalColumn[]
			{
				messageTable.MessageDocumentId
			}, null, null, 0, num, list, false, true);
			using (Reader reader = tableOperator.ExecuteReader(true))
			{
				while (reader.Read())
				{
					list2.Add(reader.GetInt32(messageTable.MessageDocumentId));
				}
			}
			list2.Sort();
			return list2;
		}

		internal static Column GetConversationIdColumn(Mailbox mailbox)
		{
			return PropertySchema.MapToColumn(mailbox.Database, ObjectType.Message, PropTag.Message.ConversationId);
		}

		protected override void PossiblyRiseNotificationEvent(Context context, NotificationEvent notificationEvent)
		{
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.Conversation;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ConversationItem>(this);
		}

		private void PossiblyLoadFidMidList(Context context)
		{
			if (!base.IsDead)
			{
				this.fidMidList = this.LoadFidMidList(context);
			}
		}

		private IEnumerable<FidMid> GetOriginalFidMids(Context context)
		{
			return this.LoadOriginalFidMidList(context);
		}

		internal bool ConversationContainsFidMid(FidMid fidMid)
		{
			return this.fidMidList.BinarySearch(fidMid) >= 0;
		}

		private List<FidMid> LoadFidMidList(Context context)
		{
			byte[] buffer = (byte[])base.GetColumnValue(context, base.MessageTable.ConversationMembers);
			int num = 0;
			return FidMidListSerializer.FromBytes(buffer, ref num, base.Mailbox.ReplidGuidMap);
		}

		private IEnumerable<FidMid> LoadOriginalFidMidList(Context context)
		{
			byte[] buffer = (byte[])base.GetOriginalColumnValue(context, DatabaseSchema.MessageTable(base.Mailbox.Database).ConversationMembers);
			int num = 0;
			return FidMidListSerializer.FromBytes(buffer, ref num, base.Mailbox.ReplidGuidMap);
		}

		private void SetFidMidList(Context context)
		{
			byte[] value = FidMidListSerializer.ToBytes(this.fidMidList);
			base.SetColumn(context, base.MessageTable.ConversationMembers, value);
		}

		private const int MaxConversationMembers = 300;

		private List<FidMid> fidMidList;
	}
}
