using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum UserContextState : uint
	{
		Active,
		MarkedForLogoff,
		Abandoned
	}
}
