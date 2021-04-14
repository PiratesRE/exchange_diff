using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class RcptCommandEventArgs : ReceiveCommandEventArgs
	{
		internal RcptCommandEventArgs()
		{
		}

		internal RcptCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public DsnTypeRequested Notify
		{
			get
			{
				return this.notify;
			}
			set
			{
				this.notify = value;
			}
		}

		public string OriginalRecipient
		{
			get
			{
				return this.orcpt;
			}
			set
			{
				this.orcpt = value;
			}
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

		internal Dictionary<string, string> ConsumerMailOptionalArguments { get; set; }

		private DsnTypeRequested notify;

		private string orcpt;

		private MailItem mailItem;

		private RoutingAddress recipientAddress;
	}
}
