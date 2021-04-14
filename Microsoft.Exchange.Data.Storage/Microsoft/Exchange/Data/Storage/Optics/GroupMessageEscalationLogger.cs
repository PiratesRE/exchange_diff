using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMessageEscalationLogger : MailboxLoggerBase
	{
		public GroupMessageEscalationLogger(Guid mailboxGuid, OrganizationId organizationId) : base(mailboxGuid, organizationId, GroupMessageEscalationLogger.Instance.Value)
		{
		}

		private static readonly Lazy<ExtensibleLogger> Instance = new Lazy<ExtensibleLogger>(() => new ExtensibleLogger(GroupMessageEscalationLogConfiguration.Default));
	}
}
