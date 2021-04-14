using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class HierarchyNotification : BaseNotification
	{
		public FolderId FolderId { get; set; }

		public string FolderClass { get; set; }

		internal StoreObjectType FolderType { get; set; }

		public string DisplayName { get; set; }

		public FolderId ParentFolderId { get; set; }

		public long ItemCount { get; set; }

		public long UnreadCount { get; set; }

		public bool IsHidden { get; set; }

		public HierarchyNotification() : base(NotificationKindType.Hierarchy)
		{
		}

		public byte[] InstanceKey;
	}
}
