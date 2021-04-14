using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal class Rcpt2CommandEventArgs : ReceiveCommandEventArgs
	{
		public Rcpt2CommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public RoutingAddress RecipientAddress
		{
			get
			{
				return this.recipientAddress;
			}
			set
			{
				if (!value.IsValid)
				{
					throw new ArgumentException(string.Format("The specified address is an invalid SMTP address - {0}", value));
				}
				this.recipientAddress = value;
			}
		}

		public MailItem MailItem { get; set; }

		public Dictionary<string, string> ConsumerMailOptionalArguments { get; set; }

		private RoutingAddress recipientAddress;
	}
}
