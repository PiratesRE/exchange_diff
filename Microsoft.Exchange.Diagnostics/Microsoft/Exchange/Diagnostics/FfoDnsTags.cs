using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FfoDnsTags
	{
		public const int FfoDnsServerCommon = 0;

		public const int FfoDnsServer = 1;

		public const int FfoDnsServerDBPlugIn = 2;

		public static Guid guid = new Guid("9CCAE37E-338A-403B-9EBB-2636514DEE9C");
	}
}
