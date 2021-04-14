using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class DisconnectEventSourceImpl : DisconnectEventSource
	{
		private DisconnectEventSourceImpl()
		{
		}

		public static DisconnectEventSource Create(SmtpSession smtpSession)
		{
			return new DisconnectEventSourceImpl();
		}
	}
}
