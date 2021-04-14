using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal class ThreadLimitExceededException : LocalizedException
	{
		public ThreadLimitExceededException(SmtpResponse response) : base(new LocalizedString(response.ToString()))
		{
			this.SmtpResponse = response;
		}

		public ThreadLimitExceededException(string message) : base(new LocalizedString(message))
		{
			this.SmtpResponse = SmtpResponse.Empty;
		}

		internal SmtpResponse SmtpResponse { get; private set; }
	}
}
