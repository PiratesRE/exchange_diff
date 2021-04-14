using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class AttachmentDataProviderChangedEventArgs : EventArgs
	{
		internal AttachmentDataProviderChangedEventArgs(MailboxSession mailboxSession)
		{
			this.MailboxSession = mailboxSession;
		}

		internal MailboxSession MailboxSession { get; private set; }
	}
}
