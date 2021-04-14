using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public abstract class QueuedMessageEventArgs : EventArgs
	{
		internal QueuedMessageEventArgs()
		{
		}

		public abstract MailItem MailItem { get; }
	}
}
