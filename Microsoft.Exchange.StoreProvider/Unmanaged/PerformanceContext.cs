using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PerformanceContext
	{
		public uint rpcCount;

		public ulong rpcLatency;

		public uint currentActiveConnections;

		public uint currentConnectionPoolSize;

		public uint failedConnections;

		private IntPtr prev;

		private IntPtr next;
	}
}
