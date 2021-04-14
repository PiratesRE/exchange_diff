using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ReconciliationTags
	{
		public const int StartProcessingMessage = 0;

		public const int EndProcessingMessage = 255;

		public static Guid guid = new Guid("E06E0123-1B5C-4f61-959D-8258BF6C689A");
	}
}
