using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MailboxConnectionArgs : EventArgs
	{
		internal MailboxConnectionArgs(bool connected)
		{
			this.SuccessfulConnection = connected;
		}

		internal bool SuccessfulConnection { get; private set; }
	}
}
