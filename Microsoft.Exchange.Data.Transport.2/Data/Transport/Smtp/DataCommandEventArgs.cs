using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class DataCommandEventArgs : ReceiveCommandEventArgs
	{
		internal DataCommandEventArgs()
		{
		}

		internal DataCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
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

		private MailItem mailItem;
	}
}
