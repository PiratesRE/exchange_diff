using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Groups;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupEscalateItemDiagnosticsFrameFactory : IDiagnosticsFrameFactory<IExtensibleLogger, IGroupEscalateItemPerformanceTracker>
	{
		private GroupEscalateItemDiagnosticsFrameFactory()
		{
		}

		public IDiagnosticsFrame CreateDiagnosticsFrame(string operationContext, string operationName, IExtensibleLogger logger, IMailboxPerformanceTracker performanceTracker)
		{
			return new DiagnosticsFrame(operationContext, operationName, GroupEscalateItemDiagnosticsFrameFactory.Tracer, logger, performanceTracker);
		}

		public IExtensibleLogger CreateLogger(Guid mailboxGuid, OrganizationId organizationId)
		{
			return new GroupMessageEscalationLogger(mailboxGuid, organizationId);
		}

		public IGroupEscalateItemPerformanceTracker CreatePerformanceTracker(IMailboxSession mailboxSession)
		{
			return new GroupEscalateItemPerformanceTracker(mailboxSession);
		}

		private static readonly Trace Tracer = ExTraceGlobals.COWGroupMessageEscalationTracer;

		public static readonly GroupEscalateItemDiagnosticsFrameFactory Default = new GroupEscalateItemDiagnosticsFrameFactory();
	}
}
