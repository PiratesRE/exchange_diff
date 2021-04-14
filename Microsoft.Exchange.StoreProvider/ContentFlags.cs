using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ContentFlags
	{
		FullString = 0,
		SubString = 1,
		Prefix = 2,
		PrefixOnWords = 17,
		ExactPhrase = 32,
		IgnoreCase = 65536,
		IgnoreNonSpace = 131072,
		Loose = 262144
	}
}
