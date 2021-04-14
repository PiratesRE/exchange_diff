using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal enum UserContextTerminationStatus
	{
		NotTerminate,
		TerminatePending,
		TerminateStarted,
		TerminateCompleted
	}
}
