using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class NullMatch : IMatch
	{
		public bool IsMatch(TextScanContext data)
		{
			return false;
		}
	}
}
