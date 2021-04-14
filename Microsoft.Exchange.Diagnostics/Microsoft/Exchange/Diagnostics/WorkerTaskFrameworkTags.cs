using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct WorkerTaskFrameworkTags
	{
		public const int FaultInjection = 0;

		public const int Core = 1;

		public const int DataAccess = 2;

		public const int WorkItem = 3;

		public const int ManagedAvailability = 4;

		public static Guid guid = new Guid("EAF36C57-87B9-4D84-B551-3537A14A62B8");
	}
}
