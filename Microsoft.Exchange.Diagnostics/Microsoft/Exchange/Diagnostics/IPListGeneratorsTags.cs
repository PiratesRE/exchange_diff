using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct IPListGeneratorsTags
	{
		public const int Common = 0;

		public const int IPListGenerator = 1;

		public const int RWBLListGenerator = 2;

		public const int TBLListGenerator = 3;

		public static Guid guid = new Guid("4A1C4EB6-CEAC-42f3-A708-3FF1536B0DD7");
	}
}
