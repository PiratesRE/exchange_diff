using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxImportRequest : RequestBase
	{
		public MailboxImportRequest()
		{
		}

		internal MailboxImportRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public new string FilePath
		{
			get
			{
				return base.FilePath;
			}
		}

		public ADObjectId Mailbox
		{
			get
			{
				return base.TargetMailbox;
			}
		}

		public override string ToString()
		{
			if (base.Name != null && base.TargetMailbox != null)
			{
				return string.Format("{0}\\{1}", base.TargetMailbox.ToString(), base.Name);
			}
			return base.ToString();
		}
	}
}
