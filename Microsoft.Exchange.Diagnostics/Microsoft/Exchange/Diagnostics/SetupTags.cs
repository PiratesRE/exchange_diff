using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SetupTags
	{
		public const int Trace = 0;

		public const int FaultInjection = 1;

		public static Guid guid = new Guid("0868076d-75ca-47bf-8d73-487edd017b4d");
	}
}
