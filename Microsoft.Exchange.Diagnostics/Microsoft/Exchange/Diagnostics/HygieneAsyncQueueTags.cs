using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct HygieneAsyncQueueTags
	{
		public const int AsyncQueueService = 0;

		public const int AsyncQueueExecutor = 1;

		public const int AsyncStepExecutor = 2;

		public static Guid guid = new Guid("040DF3E7-309C-4531-A762-6136DBD1004A");
	}
}
