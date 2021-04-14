using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxRestoreRequest : RequestBase
	{
		public MailboxRestoreRequest()
		{
		}

		internal MailboxRestoreRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public new ADObjectId SourceDatabase
		{
			get
			{
				return base.SourceDatabase;
			}
		}

		public new ADObjectId TargetMailbox
		{
			get
			{
				return base.TargetMailbox;
			}
		}

		public override string ToString()
		{
			if (base.Name != null && this.TargetMailbox != null)
			{
				return string.Format("{0}\\{1}", this.TargetMailbox.ToString(), base.Name);
			}
			return base.ToString();
		}
	}
}
