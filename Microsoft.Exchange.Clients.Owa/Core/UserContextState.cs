using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum UserContextState : uint
	{
		Active,
		MarkedForLogoff,
		Abandoned
	}
}
