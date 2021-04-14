using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum SessionDataCacheState
	{
		Uninitialized,
		Building,
		Ready,
		Obsolete
	}
}
