using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessagingConfigurationBase : BaseRow
	{
		public MessagingConfigurationBase(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
			this.MailboxMessageConfiguration = mailboxMessageConfiguration;
		}

		public MailboxMessageConfiguration MailboxMessageConfiguration { get; private set; }
	}
}
