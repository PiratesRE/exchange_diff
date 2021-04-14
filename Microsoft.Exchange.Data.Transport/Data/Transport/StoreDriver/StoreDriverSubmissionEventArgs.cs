using System;

namespace Microsoft.Exchange.Data.Transport.StoreDriver
{
	internal abstract class StoreDriverSubmissionEventArgs : StoreDriverEventArgs
	{
		internal StoreDriverSubmissionEventArgs()
		{
		}

		public abstract MailItem MailItem { get; }
	}
}
