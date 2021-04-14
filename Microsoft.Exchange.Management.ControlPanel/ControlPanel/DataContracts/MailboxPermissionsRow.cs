using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	[DataContract]
	public class MailboxPermissionsRow : BaseRow
	{
		public MailboxPermissionsRow(MailboxAcePresentationObject mailboxpo)
		{
			this.mailboxpermissionspo = mailboxpo;
		}

		[DataMember]
		public bool HasReadAccess
		{
			get
			{
				return this.HasRight(MailboxRights.ReadPermission);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool HasFullAccess
		{
			get
			{
				return this.HasRight(MailboxRights.FullAccess);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private bool HasRight(MailboxRights right)
		{
			foreach (MailboxRights mailboxRights in this.mailboxpermissionspo.AccessRights)
			{
				if ((mailboxRights & right) == right)
				{
					return true;
				}
			}
			return false;
		}

		private MailboxAcePresentationObject mailboxpermissionspo;
	}
}
