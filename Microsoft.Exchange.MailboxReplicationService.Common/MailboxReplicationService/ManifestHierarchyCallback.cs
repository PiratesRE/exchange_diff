using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ManifestHierarchyCallback : IMapiHierarchyManifestCallback
	{
		public ManifestHierarchyCallback(bool isPagedEnumeration)
		{
			this.isPagedEnumeration = isPagedEnumeration;
		}

		public void InitializeNextPage(MailboxChangesManifest mailboxChangesManifest, int maxChanges)
		{
			this.changes = mailboxChangesManifest;
			this.changes.ChangedFolders = new List<byte[]>((!this.isPagedEnumeration) ? 4 : maxChanges);
			this.changes.DeletedFolders = new List<byte[]>();
			this.maxChanges = maxChanges;
			this.countEnumeratedChanges = 0;
			bool flag = this.isPagedEnumeration;
		}

		ManifestCallbackStatus IMapiHierarchyManifestCallback.Change(PropValue[] props)
		{
			byte[] item = null;
			if (props != null)
			{
				foreach (PropValue propValue in props)
				{
					PropTag propTag = propValue.PropTag;
					if (propTag == PropTag.EntryId)
					{
						item = propValue.GetBytes();
					}
				}
			}
			this.changes.ChangedFolders.Add(item);
			return this.ReturnManifestCallbackStatus();
		}

		ManifestCallbackStatus IMapiHierarchyManifestCallback.Delete(byte[] data)
		{
			this.changes.DeletedFolders.Add(data);
			return this.ReturnManifestCallbackStatus();
		}

		private ManifestCallbackStatus ReturnManifestCallbackStatus()
		{
			this.countEnumeratedChanges++;
			if (this.isPagedEnumeration && this.countEnumeratedChanges == this.maxChanges)
			{
				this.changes.HasMoreHierarchyChanges = true;
				return ManifestCallbackStatus.Yield;
			}
			return ManifestCallbackStatus.Continue;
		}

		private readonly bool isPagedEnumeration;

		private int maxChanges;

		private int countEnumeratedChanges;

		private MailboxChangesManifest changes;
	}
}
