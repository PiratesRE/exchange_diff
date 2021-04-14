using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum OwaEventVerb
	{
		Unsupported = 0,
		Post = 1,
		Get = 2
	}
}
