using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum FuzzyLevel : uint
	{
		FullString = 0U,
		SubString = 1U,
		Prefix = 2U,
		PrefixOnWords = 16U,
		ExactPhrase = 32U,
		IgnoreCase = 65536U,
		IgnoreNonSpace = 131072U,
		Loose = 262144U
	}
}
