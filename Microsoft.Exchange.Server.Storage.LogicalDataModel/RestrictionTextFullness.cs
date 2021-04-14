using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum RestrictionTextFullness : ushort
	{
		FullString = 0,
		SubString = 1,
		Prefix = 2,
		PrefixOnAnyWord = 16,
		PhraseMatch = 32
	}
}
