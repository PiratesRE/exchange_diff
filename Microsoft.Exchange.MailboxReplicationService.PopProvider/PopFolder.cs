using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class PopFolder : DisposeTrackableBase, IFolder, IDisposable
	{
		public PopFolder()
		{
		}

		public PopMailbox Mailbox { get; private set; }

		public byte[] FolderId { get; private set; }

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			return this.Mailbox.GetFolderRec(this.FolderId);
		}

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			return this.EnumerateMessagesOnPopConnection(0);
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			List<string> list = new List<string>(keysToLookup.Count);
			foreach (byte[] messageEntryId in keysToLookup)
			{
				list.Add(PopEntryId.ParseUid(messageEntryId));
			}
			return this.EnumerateMessagesOnPopConnection(list);
		}

		RawSecurityDescriptor IFolder.GetSecurityDescriptor(SecurityProp secProp)
		{
			return null;
		}

		void IFolder.DeleteMessages(byte[][] entryIds)
		{
			throw new NotImplementedException();
		}

		byte[] IFolder.GetFolderId()
		{
			throw new NotImplementedException();
		}

		void IFolder.SetContentsRestriction(RestrictionData restriction)
		{
			throw new NotImplementedException();
		}

		PropValueData[] IFolder.GetProps(PropTag[] pta)
		{
			throw new NotImplementedException();
		}

		void IFolder.GetSearchCriteria(out RestrictionData restriction, out byte[][] entryIds, out SearchState state)
		{
			throw new NotImplementedException();
		}

		RuleData[] IFolder.GetRules(PropTag[] extraProps)
		{
			return Array<RuleData>.Empty;
		}

		PropValueData[][] IFolder.GetACL(SecurityProp secProp)
		{
			throw new NotImplementedException();
		}

		PropValueData[][] IFolder.GetExtendedAcl(AclFlags aclFlags)
		{
			throw new NotImplementedException();
		}

		PropProblemData[] IFolder.SetProps(PropValueData[] pvda)
		{
			throw new NotImplementedException();
		}

		internal void Config(byte[] folderId, PopMailbox mailbox)
		{
			this.FolderId = folderId;
			this.Mailbox = mailbox;
		}

		protected List<MessageRec> EnumerateMessagesOnPopConnection(int maxItems)
		{
			if (this.FolderId != PopMailbox.InboxEntryId)
			{
				return new List<MessageRec>(0);
			}
			Pop3ResultData uniqueIds = this.Mailbox.PopConnection.GetUniqueIds();
			if (uniqueIds == null)
			{
				return new List<MessageRec>();
			}
			return this.GetMessageRecs(uniqueIds, null, maxItems);
		}

		protected List<MessageRec> EnumerateMessagesOnPopConnection(List<string> uids)
		{
			if (this.FolderId != PopMailbox.InboxEntryId)
			{
				return new List<MessageRec>(0);
			}
			if (uids == null || uids.Count == 0)
			{
				return new List<MessageRec>(0);
			}
			Pop3ResultData uniqueIds = this.Mailbox.PopConnection.GetUniqueIds();
			if (uniqueIds == null)
			{
				return new List<MessageRec>(0);
			}
			return this.GetMessageRecs(uniqueIds, uids, 0);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PopFolder>(this);
		}

		protected MessageRec CreateMessageRec(string uniqueId, long messageSize)
		{
			PropValueData[] messageProps = SyncEmailUtils.GetMessageProps(default(SyncEmailContext), this.Mailbox.PopConnection.ConnectionContext.UserName, this.FolderId, new PropValueData[0]);
			PropValueData[] array = new PropValueData[messageProps.Length + 1];
			messageProps.CopyTo(array, 0);
			array[array.Length - 1] = new PropValueData(PropTag.LastModificationTime, CommonUtils.DefaultLastModificationTime);
			return new MessageRec(PopEntryId.CreateMessageEntryId(uniqueId), this.FolderId, CommonUtils.DefaultLastModificationTime, (int)messageSize, MsgRecFlags.None, array);
		}

		private List<MessageRec> GetMessageRecs(Pop3ResultData result, List<string> uids, int maxItems = 0)
		{
			bool flag = uids == null || uids.Count == 0;
			List<MessageRec> list = new List<MessageRec>();
			int num = result.EmailDropCount;
			while (num > 1 && (maxItems == 0 || list.Count < maxItems))
			{
				string uniqueId = result.GetUniqueId(num);
				if (uniqueId != null)
				{
					this.Mailbox.UniqueIdMap[uniqueId] = num;
					if (flag || uids.Contains(uniqueId))
					{
						long emailSize = result.GetEmailSize(num);
						list.Add(this.CreateMessageRec(uniqueId, emailSize));
					}
				}
				num--;
			}
			return list;
		}
	}
}
