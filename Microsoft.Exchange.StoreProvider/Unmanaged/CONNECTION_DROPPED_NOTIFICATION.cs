using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct CONNECTION_DROPPED_NOTIFICATION
	{
		internal IntPtr lpszServerDN;

		internal IntPtr lpszUserDN;

		internal int dwTickDeath;
	}
}
