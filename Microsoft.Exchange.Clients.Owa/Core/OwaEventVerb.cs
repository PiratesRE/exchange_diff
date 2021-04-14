using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum OwaEventVerb
	{
		Unsupported = 0,
		Post = 1,
		Get = 2
	}
}
