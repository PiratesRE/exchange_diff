using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SecurityConstants
	{
		public const int MaxElementCountInMultiValuedProperty = 32768;

		public const int MaxInMemoryPropertyStreamLength = 1048576;

		public const int MaxPropertyStreamLength = 1073741824;
	}
}
