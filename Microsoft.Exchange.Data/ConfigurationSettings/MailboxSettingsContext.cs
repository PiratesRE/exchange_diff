using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxSettingsContext : SettingsContextBase
	{
		public MailboxSettingsContext(Guid mailboxGuid, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.mailboxGuid = mailboxGuid;
		}

		public override Guid? MailboxGuid
		{
			get
			{
				return new Guid?(this.mailboxGuid);
			}
		}

		private readonly Guid mailboxGuid;
	}
}
