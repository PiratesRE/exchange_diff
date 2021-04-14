using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FfoReportingTags
	{
		public const int Cmdlets = 0;

		public const int HttpModule = 1;

		public const int FaultInjection = 2;

		public static Guid guid = new Guid("68B388E3-66FC-486C-BD59-C1738D89D4D7");
	}
}
