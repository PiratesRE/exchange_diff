using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class MailboxExportRequest : RequestBase
	{
		public MailboxExportRequest()
		{
		}

		internal MailboxExportRequest(IRequestIndexEntry index) : base(index)
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
				return base.SourceMailbox;
			}
		}

		public override string ToString()
		{
			if (base.Name != null && base.SourceMailbox != null)
			{
				return string.Format("{0}\\{1}", base.SourceMailbox.ToString(), base.Name);
			}
			return base.ToString();
		}
	}
}
