using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AnchorService
{
	internal interface ISummarizable
	{
		string SummaryName { get; }

		IEnumerable<string> SummaryTokens { get; }
	}
}
