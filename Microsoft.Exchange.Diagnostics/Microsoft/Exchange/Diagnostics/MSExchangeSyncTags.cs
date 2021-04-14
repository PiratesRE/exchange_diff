using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeSyncTags
	{
		public const int Requests = 0;

		public const int Wbxml = 1;

		public const int Xso = 2;

		public const int Algorithm = 3;

		public const int Protocol = 4;

		public const int Conversion = 5;

		public const int ThreadPool = 6;

		public const int RawBodyBytes = 7;

		public const int MethodEnterExit = 8;

		public const int TiUpgrade = 9;

		public const int Validation = 10;

		public const int PfdInitTrace = 11;

		public const int CorruptItem = 12;

		public const int Threading = 13;

		public const int FaultInjection = 14;

		public const int Body = 15;

		public const int Diagnostics = 16;

		public static Guid guid = new Guid("5e88fb2c-0a36-41f2-a710-c911bfe18e44");
	}
}
