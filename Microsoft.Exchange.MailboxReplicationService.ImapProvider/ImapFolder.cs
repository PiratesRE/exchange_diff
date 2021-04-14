using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Exchange.Connections.Imap;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class ImapFolder : DisposeTrackableBase, IFolder, IDisposable
	{
		public ImapFolder()
		{
		}

		public ImapClientFolder Folder { get; private set; }

		public ImapMailbox Mailbox { get; private set; }

		public byte[] FolderId { get; private set; }

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			return this.Mailbox.CreateFolderRec(this.Folder);
		}

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			throw new NotImplementedException();
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			List<uint> list = new List<uint>(keysToLookup.Count);
			list.AddRange(keysToLookup.Select(new Func<byte[], uint>(ImapEntryId.ParseUid)));
			list.Sort((uint x, uint y) => y.CompareTo(x));
			List<ImapMessageRec> imapMessageRecs = this.Folder.LookupMessages(this.Mailbox.ImapConnection, list);
			return this.GetMessageRecs(imapMessageRecs);
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
			return new RuleData[0];
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

		internal static SyncEmailContext GetSyncEmailContext(ImapClientFolder folder, ImapMessageRec messageRec)
		{
			ArgumentValidator.ThrowIfNull("folder", folder);
			ArgumentValidator.ThrowIfNull("messageRec", messageRec);
			SyncEmailContext result = default(SyncEmailContext);
			ImapMailFlags imapMailFlags = messageRec.ImapMailFlags;
			ImapMailFlags imapMailFlags2 = folder.SupportedFlags;
			if (folder.DefaultFolderType.Equals(ImapDefaultFolderType.Drafts))
			{
				imapMailFlags |= ImapMailFlags.Draft;
				imapMailFlags2 |= ImapMailFlags.Draft;
			}
			if (imapMailFlags2.HasFlag(ImapMailFlags.Draft))
			{
				result.IsDraft = new bool?(imapMailFlags.HasFlag(ImapMailFlags.Draft));
			}
			if (imapMailFlags2.HasFlag(ImapMailFlags.Seen))
			{
				result.IsRead = new bool?(imapMailFlags.HasFlag(ImapMailFlags.Seen));
			}
			if (imapMailFlags2.HasFlag(ImapMailFlags.Answered))
			{
				result.ResponseType = new SyncMessageResponseType?(imapMailFlags.HasFlag(ImapMailFlags.Answered) ? SyncMessageResponseType.Replied : SyncMessageResponseType.None);
			}
			ImapExtendedMessageRec imapExtendedMessageRec = messageRec as ImapExtendedMessageRec;
			if (imapExtendedMessageRec != null)
			{
				result.SyncMessageId = imapExtendedMessageRec.MessageId;
			}
			return result;
		}

		internal void Config(byte[] folderId, ImapClientFolder folder, ImapMailbox mailbox)
		{
			this.FolderId = folderId;
			this.Folder = folder;
			this.Mailbox = mailbox;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.Folder = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ImapFolder>(this);
		}

		protected List<MessageRec> EnumerateMessages(FetchMessagesFlags flags, int? highFetchValue = null, int? lowFetchValue = null)
		{
			if (!this.Folder.IsSelectable)
			{
				return new List<MessageRec>(0);
			}
			List<ImapMessageRec> imapMessageRecs = this.Folder.EnumerateMessages(this.Mailbox.ImapConnection, flags, highFetchValue, lowFetchValue);
			return this.GetMessageRecs(imapMessageRecs);
		}

		private List<MessageRec> GetMessageRecs(List<ImapMessageRec> imapMessageRecs)
		{
			if (imapMessageRecs.Count == 0)
			{
				return new List<MessageRec>(0);
			}
			List<MessageRec> list = new List<MessageRec>(imapMessageRecs.Count);
			foreach (ImapMessageRec imapMessageRec in imapMessageRecs)
			{
				SyncEmailContext syncEmailContext = ImapFolder.GetSyncEmailContext(this.Folder, imapMessageRec);
				int messageSize = 0;
				ImapExtendedMessageRec imapExtendedMessageRec = imapMessageRec as ImapExtendedMessageRec;
				if (imapExtendedMessageRec != null)
				{
					messageSize = (int)imapExtendedMessageRec.MessageSize;
				}
				PropValueData[] messageProps = SyncEmailUtils.GetMessageProps(syncEmailContext, this.Mailbox.ImapConnection.ConnectionContext.UserName, this.FolderId, new PropValueData[0]);
				MessageRec item = new MessageRec(ImapEntryId.CreateMessageEntryId(imapMessageRec.Uid, this.Folder.UidValidity, this.Folder.Name, this.Mailbox.ImapConnection.ConnectionContext.UserName), this.FolderId, CommonUtils.DefaultLastModificationTime, messageSize, MsgRecFlags.None, messageProps);
				list.Add(item);
			}
			return list;
		}
	}
}
