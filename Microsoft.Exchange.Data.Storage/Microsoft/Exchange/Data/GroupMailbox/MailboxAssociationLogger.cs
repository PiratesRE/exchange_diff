using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MailboxAssociationLogger : MailboxLoggerBase
	{
		public MailboxAssociationLogger(Guid mailboxGuid, OrganizationId organizationId) : base(mailboxGuid, organizationId, MailboxAssociationLogger.Instance.Value)
		{
		}

		private static readonly Lazy<ExtensibleLogger> Instance = new Lazy<ExtensibleLogger>(() => new ExtensibleLogger(MailboxAssociationLogConfiguration.Default));
	}
}
