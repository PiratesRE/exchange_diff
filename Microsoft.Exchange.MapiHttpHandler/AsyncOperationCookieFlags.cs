using System;

namespace Microsoft.Exchange.MapiHttp
{
	[Flags]
	internal enum AsyncOperationCookieFlags
	{
		None = 0,
		RequireSession = 1,
		AllowSession = 2,
		CreateSession = 4,
		RequireSequence = 8,
		AllowInvalidSession = 16,
		DestroySession = 32
	}
}
