using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ULongConverter
	{
		public static ulong Parse(string propertyString)
		{
			return ulong.Parse(propertyString, CultureInfo.InvariantCulture);
		}

		public static string ToString(ulong propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}
