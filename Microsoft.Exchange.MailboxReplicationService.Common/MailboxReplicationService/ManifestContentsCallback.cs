using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ManifestContentsCallback : IMapiManifestCallback
	{
		public ManifestContentsCallback(byte[] folderId, bool isPagedEnumeration)
		{
			this.folderId = folderId;
			this.isPagedEnumeration = isPagedEnumeration;
		}

		public void InitializeNextPage(FolderChangesManifest folderChangesManifest, int maxChanges)
		{
			this.changes = folderChangesManifest;
			this.changes.ChangedMessages = new List<MessageRec>((!this.isPagedEnumeration) ? 0 : maxChanges);
			this.changes.ReadMessages = new List<byte[]>();
			this.changes.UnreadMessages = new List<byte[]>();
			this.maxChanges = maxChanges;
			this.countEnumeratedChanges = 0;
			bool flag = this.isPagedEnumeration;
		}

		ManifestCallbackStatus IMapiManifestCallback.Change(byte[] entryId, byte[] sourceKey, byte[] changeKey, byte[] changeList, DateTime lastModificationTime, ManifestChangeType changeType, bool associated, PropValue[] props)
		{
			int messageSize = 0;
			if (props != null)
			{
				foreach (PropValue propValue in props)
				{
					PropTag propTag = propValue.PropTag;
					if (propTag == PropTag.MessageSize)
					{
						messageSize = propValue.GetInt();
					}
				}
			}
			MsgRecFlags msgRecFlags = associated ? MsgRecFlags.Associated : MsgRecFlags.None;
			if (changeType.Equals(ManifestChangeType.Add))
			{
				msgRecFlags |= MsgRecFlags.New;
			}
			MessageRec item = new MessageRec(entryId, this.folderId, DateTime.MinValue, messageSize, msgRecFlags, null);
			this.changes.ChangedMessages.Add(item);
			this.countEnumeratedChanges++;
			if (this.isPagedEnumeration && this.countEnumeratedChanges == this.maxChanges)
			{
				this.changes.HasMoreChanges = true;
				return ManifestCallbackStatus.Yield;
			}
			return ManifestCallbackStatus.Continue;
		}

		ManifestCallbackStatus IMapiManifestCallback.Delete(byte[] entryId, bool softDelete, bool expiry)
		{
			MessageRec item = new MessageRec(entryId, this.folderId, DateTime.MinValue, 0, MsgRecFlags.Deleted, null);
			this.changes.ChangedMessages.Add(item);
			return ManifestCallbackStatus.Continue;
		}

		ManifestCallbackStatus IMapiManifestCallback.ReadUnread(byte[] entryId, bool read)
		{
			if (read)
			{
				this.changes.ReadMessages.Add(entryId);
			}
			else
			{
				this.changes.UnreadMessages.Add(entryId);
			}
			return ManifestCallbackStatus.Continue;
		}

		private readonly byte[] folderId;

		private readonly bool isPagedEnumeration;

		private int maxChanges;

		private int countEnumeratedChanges;

		private FolderChangesManifest changes;
	}
}
