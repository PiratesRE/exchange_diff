using System;

namespace Microsoft.Exchange.Data.Mime
{
	[Flags]
	public enum AddressParserFlags
	{
		None = 0,
		IgnoreComments = 1,
		AllowSquareBrackets = 2
	}
}
