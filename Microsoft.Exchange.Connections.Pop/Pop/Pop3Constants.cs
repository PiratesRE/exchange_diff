using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class Pop3Constants
	{
		internal const int DefaultPort = 110;

		internal static readonly TimeSpan PopConnectionTimeout = TimeSpan.FromHours(3.0);
	}
}
