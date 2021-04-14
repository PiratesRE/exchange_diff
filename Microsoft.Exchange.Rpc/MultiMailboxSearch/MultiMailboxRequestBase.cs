using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal abstract class MultiMailboxRequestBase : MultiMailboxSearchBase
	{
		internal MultiMailboxRequestBase(int version, MultiMailboxSearchMailboxInfo[] mailboxInfos) : base(version)
		{
			this.mailboxInfos = mailboxInfos;
		}

		internal MultiMailboxRequestBase(MultiMailboxSearchMailboxInfo[] mailboxInfos) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.mailboxInfos = mailboxInfos;
		}

		internal MultiMailboxRequestBase(int version) : base(version)
		{
		}

		internal MultiMailboxRequestBase() : base(MultiMailboxSearchBase.CurrentVersion)
		{
		}

		internal MultiMailboxSearchMailboxInfo[] MailboxInfos
		{
			get
			{
				return this.mailboxInfos;
			}
			set
			{
				this.mailboxInfos = value;
			}
		}

		private MultiMailboxSearchMailboxInfo[] mailboxInfos;
	}
}
