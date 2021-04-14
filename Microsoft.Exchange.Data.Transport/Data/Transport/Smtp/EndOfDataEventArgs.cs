using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class EndOfDataEventArgs : ReceiveEventArgs
	{
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

		internal EndOfDataEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		private MailItem mailItem;
	}
}
