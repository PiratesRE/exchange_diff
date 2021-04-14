using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class FolderMoveRequest : RequestBase
	{
		public FolderMoveRequest()
		{
		}

		internal FolderMoveRequest(IRequestIndexEntry index) : base(index)
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
			if (base.Name != null && this.TargetMailbox != null)
			{
				return string.Format("{0}\\{1}", this.TargetMailbox.ToString(), base.Name);
			}
			return base.ToString();
		}
	}
}
