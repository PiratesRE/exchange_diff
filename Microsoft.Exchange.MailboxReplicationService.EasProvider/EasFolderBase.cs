using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EasFolderBase : DisposeTrackableBase, IFolder, IDisposable
	{
		protected EasFolderBase()
		{
		}

		protected EasFolderBase(string serverId, string parentId, string displayName, EasFolderType folderType)
		{
			this.serverId = serverId;
			this.parentId = parentId;
			this.displayName = displayName;
			this.folderType = folderType;
			this.EntryId = EasMailbox.GetEntryId(serverId);
		}

		internal string ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		internal string ParentId
		{
			get
			{
				return this.parentId;
			}
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal EasFolderType EasFolderType
		{
			get
			{
				return this.folderType;
			}
		}

		internal byte[] EntryId { get; private set; }

		internal EasMailbox Mailbox { get; private set; }

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			return this.InternalGetFolderRec(additionalPtagsToLoad, flags);
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			return this.InternalLookupMessages(ptagToLookup, keysToLookup, additionalPtagsToLoad);
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

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			throw new NotImplementedException();
		}

		internal EasFolderBase Configure(EasMailbox mailbox)
		{
			this.Mailbox = mailbox;
			return this;
		}

		protected static FolderRec CreateGenericFolderRec(EasFolderBase folder)
		{
			return new FolderRec(folder.EntryId, EasMailbox.GetEntryId(folder.ParentId), FolderType.Generic, folder.DisplayName, DateTime.MinValue, null);
		}

		protected virtual FolderRec InternalGetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			return EasFolderBase.CreateGenericFolderRec(this);
		}

		protected abstract List<MessageRec> InternalLookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad);

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Mailbox = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EasFolderBase>(this);
		}

		protected FolderChangesManifest CreateInitializedChangesManifest()
		{
			return new FolderChangesManifest(this.EntryId)
			{
				ChangedMessages = new List<MessageRec>(),
				ReadMessages = new List<byte[]>(),
				UnreadMessages = new List<byte[]>()
			};
		}

		private readonly string serverId;

		private readonly string parentId;

		private readonly string displayName;

		private readonly EasFolderType folderType;
	}
}
