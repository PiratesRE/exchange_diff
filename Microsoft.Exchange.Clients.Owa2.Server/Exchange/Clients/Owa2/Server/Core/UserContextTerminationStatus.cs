using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum UserContextTerminationStatus
	{
		NotTerminate,
		TerminatePending,
		TerminateStarted,
		TerminateCompleted
	}
}
