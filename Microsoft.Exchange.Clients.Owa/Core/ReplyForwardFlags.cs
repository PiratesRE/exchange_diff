using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	internal enum ReplyForwardFlags
	{
		None = 0,
		DropBody = 1,
		DropHeader = 2
	}
}
