using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Constants
	{
		internal const int DefaultTimeout = 300000;

		internal const long DefaultMaxBytesToTransfer = 9223372036854775807L;
	}
}
