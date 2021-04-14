using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SpamFilterTags
	{
		public const int Agent = 0;

		public const int BlockSenders = 1;

		public const int SafeSenders = 2;

		public const int BypassCheck = 3;

		public static Guid guid = new Guid("175562D6-54D7-4C59-A421-598E03755639");
	}
}
