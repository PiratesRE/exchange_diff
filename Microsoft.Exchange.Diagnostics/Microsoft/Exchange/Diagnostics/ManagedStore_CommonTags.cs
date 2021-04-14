using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_CommonTags
	{
		public const int General = 0;

		public const int Statistics = 1;

		public const int ResetStatistics = 2;

		public const int LockWaitTime = 3;

		public const int Tasks = 4;

		public const int ExceptionHandler = 5;

		public const int Configuration = 6;

		public const int FaultInjection = 20;

		public const int Buddy1 = 50;

		public const int Buddy2 = 51;

		public const int Buddy3 = 52;

		public const int Buddy4 = 53;

		public const int Buddy5 = 54;

		public static Guid guid = new Guid("b5c49b06-9eda-4e9d-b5b0-d696691fe1a7");
	}
}
