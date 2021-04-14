using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal class ProxyInboundMessageEventArgs : ReceiveCommandEventArgs
	{
		public ProxyInboundMessageEventArgs(SmtpSession smtpSession, MailItem mailItem, bool clientIsPreE15InternalServer, bool localFrontendIsColocatedWithHub, string localServerFqdn) : base(smtpSession)
		{
			this.MailItem = mailItem;
			this.ClientIsPreE15InternalServer = clientIsPreE15InternalServer;
			this.LocalFrontendIsColocatedWithHub = localFrontendIsColocatedWithHub;
			this.LocalServerFqdn = localServerFqdn;
		}

		public MailItem MailItem { get; private set; }

		public bool ClientIsPreE15InternalServer { get; private set; }

		public bool LocalFrontendIsColocatedWithHub { get; private set; }

		public string LocalServerFqdn { get; private set; }
	}
}
