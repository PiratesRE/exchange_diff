using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDiagnosticsFrameFactory<TLogger, TTracker> where TLogger : IExtensibleLogger where TTracker : IMailboxPerformanceTracker
	{
		IDiagnosticsFrame CreateDiagnosticsFrame(string operationContext, string operationName, IExtensibleLogger logger, IMailboxPerformanceTracker performanceTracker);

		TLogger CreateLogger(Guid mailboxGuid, OrganizationId organizationId);

		TTracker CreatePerformanceTracker(IMailboxSession mailboxSession);
	}
}
