using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum UserContextCreationError
	{
		None,
		UnableToResolveLogonIdentity,
		UnableToAcquireOwaRWLock
	}
}
