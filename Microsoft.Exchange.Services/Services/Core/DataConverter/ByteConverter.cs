using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ByteConverter
	{
		public static byte Parse(string propertyString)
		{
			return byte.Parse(propertyString, CultureInfo.InvariantCulture);
		}

		public static string ToString(byte propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}
