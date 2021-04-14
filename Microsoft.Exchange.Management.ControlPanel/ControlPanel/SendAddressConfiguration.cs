using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SendAddressConfiguration : BaseRow
	{
		public SendAddressConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
			this.MailboxMessageConfiguration = mailboxMessageConfiguration;
		}

		[DataMember]
		public string SendAddressDefault
		{
			get
			{
				return this.MailboxMessageConfiguration.SendAddressDefault;
			}
			set
			{
				this.MailboxMessageConfiguration.SendAddressDefault = value;
			}
		}

		private MailboxMessageConfiguration MailboxMessageConfiguration { get; set; }
	}
}
