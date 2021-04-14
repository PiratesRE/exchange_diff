using System;
using System.Management.Automation;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data
{
	internal class QueryParserUtils
	{
		internal static object ConvertValueFromString(object valueToConvert, Type resultType)
		{
			string text = valueToConvert as string;
			bool flag;
			if (resultType == typeof(bool) && bool.TryParse(text, out flag))
			{
				return flag;
			}
			object result;
			if (resultType.IsEnum && EnumValidator.TryParse(resultType, text, EnumParseOptions.Default, out result))
			{
				return result;
			}
			if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				bool flag2 = text == null || "null".Equals(text, StringComparison.OrdinalIgnoreCase) || "$null".Equals(text, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					return null;
				}
			}
			return LanguagePrimitives.ConvertTo(text, resultType);
		}
	}
}
