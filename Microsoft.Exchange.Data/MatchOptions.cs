using System;

namespace Microsoft.Exchange.Data
{
	public enum MatchOptions
	{
		FullString,
		SubString,
		Prefix,
		Suffix,
		PrefixOnWords,
		ExactPhrase,
		WildcardString
	}
}
