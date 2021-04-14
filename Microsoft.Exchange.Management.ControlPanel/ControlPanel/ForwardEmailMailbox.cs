using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(ForwardEmailMailbox))]
	[DataContract]
	public class ForwardEmailMailbox : BaseRow
	{
		public ForwardEmailMailbox(Mailbox mailbox) : base(mailbox)
		{
			this.Mailbox = mailbox;
		}

		private Mailbox Mailbox { get; set; }

		[DataMember]
		public string ForwardingSmtpAddress
		{
			get
			{
				if (!(this.Mailbox.ForwardingSmtpAddress == null))
				{
					return this.Mailbox.ForwardingSmtpAddress.AddressString;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool DeliverToMailboxAndForward
		{
			get
			{
				return this.Mailbox.ForwardingSmtpAddress == null || this.Mailbox.DeliverToMailboxAndForward;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
