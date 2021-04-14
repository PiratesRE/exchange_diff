using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	[Flags]
	public enum EnumParseOptions
	{
		Default = 0,
		AllowNumericConstants = 1,
		IgnoreUnknownValues = 2,
		IgnoreCase = 4
	}
}
