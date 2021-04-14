using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkingSet;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkingSetPublisherDiagnosticsFrameFactory : IDiagnosticsFrameFactory<IExtensibleLogger, IWorkingSetPublisherPerformanceTracker>
	{
		private WorkingSetPublisherDiagnosticsFrameFactory()
		{
		}

		public IDiagnosticsFrame CreateDiagnosticsFrame(string operationContext, string operationName, IExtensibleLogger logger, IMailboxPerformanceTracker performanceTracker)
		{
			return new DiagnosticsFrame(operationContext, operationName, WorkingSetPublisherDiagnosticsFrameFactory.Tracer, logger, performanceTracker);
		}

		public IExtensibleLogger CreateLogger(Guid mailboxGuid, OrganizationId organizationId)
		{
			return new WorkingSetPublisherLogger(mailboxGuid, organizationId);
		}

		public IWorkingSetPublisherPerformanceTracker CreatePerformanceTracker(IMailboxSession mailboxSession)
		{
			return new WorkingSetPublisherPerformanceTracker(mailboxSession);
		}

		private static readonly Trace Tracer = ExTraceGlobals.WorkingSetPublisherTracer;

		public static readonly WorkingSetPublisherDiagnosticsFrameFactory Default = new WorkingSetPublisherDiagnosticsFrameFactory();
	}
}
