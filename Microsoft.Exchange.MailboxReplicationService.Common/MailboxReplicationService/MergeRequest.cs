using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MergeRequest : RequestBase
	{
		public MergeRequest()
		{
		}

		internal MergeRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public new ADObjectId SourceMailbox
		{
			get
			{
				return base.SourceMailbox;
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
			if (base.Name == null || ((this.TargetMailbox == null || (base.Flags & RequestFlags.Pull) != RequestFlags.Pull) && (this.SourceMailbox == null || (base.Flags & RequestFlags.Push) != RequestFlags.Push)))
			{
				return base.ToString();
			}
			if (this.TargetMailbox != null && (base.Flags & RequestFlags.Pull) == RequestFlags.Pull)
			{
				return string.Format("{0}\\{1}", this.TargetMailbox.ToString(), base.Name);
			}
			return string.Format("{0}\\{1}", this.SourceMailbox.ToString(), base.Name);
		}
	}
}
