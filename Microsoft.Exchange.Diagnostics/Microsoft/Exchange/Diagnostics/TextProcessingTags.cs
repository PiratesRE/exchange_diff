using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TextProcessingTags
	{
		public const int SmartTrie = 0;

		public const int Matcher = 1;

		public const int Fingerprint = 2;

		public const int Boomerang = 3;

		public static Guid guid = new Guid("B15C3C00-9FF8-47B7-A975-70F1278017EF");
	}
}
