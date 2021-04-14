using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Storage;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class OnLoadedMessageEventArgs : StorageEventArgs
	{
		public OnLoadedMessageEventArgs(MailItem mailItem)
		{
			this.mailItem = mailItem;
		}

		public override MailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		private MailItem mailItem;
	}
}
