using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct RulesPublisherTags
	{
		public const int RulesPublisher = 0;

		public const int IPListPublisher = 1;

		public const int RulesProfiler = 2;

		public const int SpamDataBlobPublisher = 3;

		public static Guid guid = new Guid("0082B730-63A3-475E-B53C-ACCA6AFDC400");
	}
}
