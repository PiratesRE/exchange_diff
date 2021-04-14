using System;
using System.Threading;

namespace Microsoft.Exchange.Transport
{
	internal static class SessionId
	{
		public static ulong GetNextSessionId()
		{
			return (ulong)Interlocked.Increment(ref SessionId.nextSessionId);
		}

		private static long nextSessionId = DateTime.UtcNow.Ticks;
	}
}
