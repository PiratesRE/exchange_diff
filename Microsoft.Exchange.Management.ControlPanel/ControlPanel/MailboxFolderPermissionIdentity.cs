using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.StoreTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxFolderPermissionIdentity : Identity
	{
		public MailboxFolderPermissionIdentity(MailboxFolderPermission permission) : base((permission.User.ADRecipient != null) ? permission.User.ADRecipient.Guid.ToString() : permission.User.ToString(), permission.User.ToString())
		{
		}

		public MailboxFolderPermissionIdentity(Identity userId, Identity mailboxFolderId) : base(userId.RawIdentity, userId.DisplayName)
		{
			this.MailboxFolderId = mailboxFolderId;
		}

		[DataMember]
		public Identity MailboxFolderId { get; set; }
	}
}
