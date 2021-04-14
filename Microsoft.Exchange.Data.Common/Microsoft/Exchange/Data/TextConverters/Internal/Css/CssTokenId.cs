using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal enum CssTokenId : byte
	{
		None,
		EndOfFile,
		AtRule = 4,
		Declarations,
		RuleSet
	}
}
