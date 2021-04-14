using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_MapiDispTags
	{
		public const int RpcBuffer = 0;

		public const int RpcOperation = 1;

		public const int RpcDetail = 2;

		public const int RopTiming = 3;

		public const int RpcContextPool = 4;

		public const int SyncMailboxWithDS = 5;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("0df8b91e-45ef-41d3-bb91-b60a4446bb35");
	}
}
