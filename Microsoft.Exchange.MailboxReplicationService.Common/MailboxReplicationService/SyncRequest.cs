using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class SyncRequest : RequestBase
	{
		public SyncRequest()
		{
		}

		internal SyncRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public ADObjectId Mailbox
		{
			get
			{
				return base.TargetMailbox;
			}
		}

		public string RemoteServerName
		{
			get
			{
				return base.RemoteHostName;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}\\{1}", base.TargetMailbox, base.Name);
		}
	}
}
