using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupEscalationGetterDiagnosticsFrameFactory : IDiagnosticsFrameFactory<IExtensibleLogger, IMailboxAssociationPerformanceTracker>
	{
		private GroupEscalationGetterDiagnosticsFrameFactory()
		{
		}

		public IDiagnosticsFrame CreateDiagnosticsFrame(string operationContext, string operationName, IExtensibleLogger logger, IMailboxPerformanceTracker performanceTracker)
		{
			return new DiagnosticsFrame(operationContext, operationName, GroupEscalationGetterDiagnosticsFrameFactory.Tracer, logger, performanceTracker);
		}

		public IExtensibleLogger CreateLogger(Guid mailboxGuid, OrganizationId organizationId)
		{
			return new GroupMessageEscalationLogger(mailboxGuid, organizationId);
		}

		public IMailboxAssociationPerformanceTracker CreatePerformanceTracker(IMailboxSession mailboxSession)
		{
			return new MailboxAssociationPerformanceTracker();
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		public static readonly GroupEscalationGetterDiagnosticsFrameFactory Default = new GroupEscalationGetterDiagnosticsFrameFactory();
	}
}
