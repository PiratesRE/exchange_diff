using System;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal enum BlockedType
	{
		None,
		NewSession = 10,
		NewRequest = 50
	}
}
