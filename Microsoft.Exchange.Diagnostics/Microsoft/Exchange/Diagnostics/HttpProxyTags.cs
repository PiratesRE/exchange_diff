using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct HttpProxyTags
	{
		public const int Brief = 0;

		public const int Verbose = 1;

		public const int FaultInjection = 2;

		public const int Exception = 3;

		public static Guid guid = new Guid("1b54246f-70bf-4885-90b7-800205abb16c");
	}
}
