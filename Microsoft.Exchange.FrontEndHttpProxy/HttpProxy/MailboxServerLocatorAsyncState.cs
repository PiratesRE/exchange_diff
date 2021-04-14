using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class MailboxServerLocatorAsyncState
	{
		public ProxyRequestHandler ProxyRequestHandler { get; set; }

		public AnchorMailbox AnchorMailbox { get; set; }

		public MailboxServerLocator Locator { get; set; }
	}
}
