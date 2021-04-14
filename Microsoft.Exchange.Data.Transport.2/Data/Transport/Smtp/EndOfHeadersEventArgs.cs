using System;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class EndOfHeadersEventArgs : ReceiveEventArgs
	{
		public HeaderList Headers
		{
			get
			{
				return this.headers;
			}
			internal set
			{
				this.headers = value;
			}
		}

		public MailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
			internal set
			{
				this.mailItem = value;
			}
		}

		internal EndOfHeadersEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		private HeaderList headers;

		private MailItem mailItem;
	}
}
