using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MailboxResource : ResourceBase
	{
		public MailboxResource(Guid mailboxGuid)
		{
			this.MailboxGuid = mailboxGuid;
			base.ConfigContext = new MailboxSettingsContext(mailboxGuid, base.ConfigContext);
		}

		public Guid MailboxGuid { get; private set; }

		public override string ResourceName
		{
			get
			{
				return this.MailboxGuid.ToString();
			}
		}
	}
}
