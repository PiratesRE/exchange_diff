using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal class ConnectionSkipException : LocalizedException
	{
		public ConnectionSkipException(SmtpResponse response) : base(new LocalizedString(response.ToString()))
		{
			this.SmtpResponse = response;
		}

		internal SmtpResponse SmtpResponse { get; private set; }
	}
}
