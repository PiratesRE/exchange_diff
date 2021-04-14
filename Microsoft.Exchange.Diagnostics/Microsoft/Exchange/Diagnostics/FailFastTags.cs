using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FailFastTags
	{
		public const int FailFastCache = 0;

		public const int FailFastModule = 1;

		public const int FailureThrottling = 2;

		public static Guid guid = new Guid("04E8E535-4C59-49CC-B92D-4598368E5B36");
	}
}
