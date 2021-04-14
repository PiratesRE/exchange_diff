using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TasksTags
	{
		public const int Trace = 0;

		public const int Log = 1;

		public const int Error = 2;

		public const int Event = 3;

		public const int EnterExit = 4;

		public const int FaultInjection = 5;

		public static Guid guid = new Guid("1e254b9e-d663-4138-8183-e5e4b077f8d3");
	}
}
