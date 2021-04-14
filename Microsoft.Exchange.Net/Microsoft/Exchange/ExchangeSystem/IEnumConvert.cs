using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal interface IEnumConvert
	{
		bool TryParse(string value, EnumParseOptions options, out object result);
	}
}
