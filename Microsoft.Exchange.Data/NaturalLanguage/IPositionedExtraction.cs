using System;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public interface IPositionedExtraction
	{
		int StartIndex { get; set; }

		EmailPosition Position { get; set; }
	}
}
