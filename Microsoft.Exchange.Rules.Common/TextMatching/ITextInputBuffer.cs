using System;

namespace Microsoft.Exchange.TextMatching
{
	internal interface ITextInputBuffer
	{
		int NextChar { get; }
	}
}
