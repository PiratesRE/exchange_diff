using System;

namespace Microsoft.Exchange.TextProcessing
{
	public interface IMatch
	{
		bool IsMatch(TextScanContext data);
	}
}
