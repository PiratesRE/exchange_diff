using System;

namespace Microsoft.Exchange.Data.Transport.Storage
{
	public abstract class StorageEventArgs : EventArgs
	{
		internal StorageEventArgs()
		{
		}

		public abstract MailItem MailItem { get; }
	}
}
