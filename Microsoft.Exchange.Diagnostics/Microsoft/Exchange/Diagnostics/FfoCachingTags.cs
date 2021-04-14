using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct FfoCachingTags
	{
		public const int PrimingThread = 0;

		public const int CachingProvider = 1;

		public const int CompositeProvider = 2;

		public const int PrimingStateLocalCache = 3;

		public static Guid guid = new Guid("880B0BC2-765E-4B89-82A0-9FFBBA7B8BE1");
	}
}
