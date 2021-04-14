using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class WorkingSetPublisherLogger : MailboxLoggerBase
	{
		public WorkingSetPublisherLogger(Guid mailboxGuid, OrganizationId organizationId) : base(mailboxGuid, organizationId, WorkingSetPublisherLogger.Instance.Value)
		{
		}

		private static readonly Lazy<ExtensibleLogger> Instance = new Lazy<ExtensibleLogger>(() => new ExtensibleLogger(WorkingSetPublisherLogConfiguration.Default));
	}
}
