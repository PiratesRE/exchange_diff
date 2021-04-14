using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociationDiagnosticsFrameFactory : IDiagnosticsFrameFactory<IExtensibleLogger, IMailboxAssociationPerformanceTracker>
	{
		private MailboxAssociationDiagnosticsFrameFactory()
		{
		}

		public IDiagnosticsFrame CreateDiagnosticsFrame(string operationContext, string operationName, IExtensibleLogger logger, IMailboxPerformanceTracker performanceTracker)
		{
			return new DiagnosticsFrame(operationContext, operationName, MailboxAssociationDiagnosticsFrameFactory.Tracer, logger, performanceTracker);
		}

		public IExtensibleLogger CreateLogger(Guid mailboxGuid, OrganizationId organizationId)
		{
			return new MailboxAssociationLogger(mailboxGuid, organizationId);
		}

		public IMailboxAssociationPerformanceTracker CreatePerformanceTracker(IMailboxSession mailboxSession)
		{
			return new MailboxAssociationPerformanceTracker();
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		public static readonly MailboxAssociationDiagnosticsFrameFactory Default = new MailboxAssociationDiagnosticsFrameFactory();
	}
}
