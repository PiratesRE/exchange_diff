using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ItemAuditInfo
	{
		public StoreObjectId Id { get; internal set; }

		public StoreObjectId ParentFolderId { get; private set; }

		public string ParentFolderPathName { get; internal set; }

		public string Subject { get; private set; }

		public Participant From { get; private set; }

		public bool IsAssociated { get; private set; }

		public List<string> DirtyProperties { get; private set; }

		public ItemAuditInfo(StoreObjectId itemId, StoreObjectId parentFolderId, string parentFolderPathName, string subject) : this(itemId, parentFolderId, parentFolderPathName, subject, null)
		{
		}

		public ItemAuditInfo(StoreObjectId itemId, StoreObjectId parentFolderId, string parentFolderPathName, string subject, Participant from) : this(itemId, parentFolderId, parentFolderPathName, subject, from, false, null)
		{
		}

		public ItemAuditInfo(StoreObjectId itemId, StoreObjectId parentFolderId, string parentFolderPathName, string subject, Participant from, bool isAssociated, List<string> dirtyProperties)
		{
			this.Id = itemId;
			this.ParentFolderId = parentFolderId;
			this.ParentFolderPathName = parentFolderPathName;
			this.Subject = subject;
			this.From = from;
			this.IsAssociated = isAssociated;
			this.DirtyProperties = dirtyProperties;
		}
	}
}
