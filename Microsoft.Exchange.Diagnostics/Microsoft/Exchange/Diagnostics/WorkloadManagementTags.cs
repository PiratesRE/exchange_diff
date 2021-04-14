using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct WorkloadManagementTags
	{
		public const int Common = 0;

		public const int Execution = 1;

		public const int Scheduler = 2;

		public const int Policies = 3;

		public const int ActivityContext = 4;

		public const int UserWorkloadManager = 5;

		public const int FaultInjection = 6;

		public const int AdmissionControl = 7;

		public static Guid guid = new Guid("488b469c-d752-4650-8655-28590e044606");
	}
}
