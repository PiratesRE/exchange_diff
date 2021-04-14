using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct EdgeSyncTags
	{
		public const int Process = 0;

		public const int Connection = 1;

		public const int Scheduler = 2;

		public const int SyncNow = 3;

		public const int Topology = 4;

		public const int SynchronizationJob = 5;

		public const int Subscription = 6;

		public static Guid guid = new Guid("AB9C28FE-50E0-4907-BB41-8F82D8E0C068");
	}
}
