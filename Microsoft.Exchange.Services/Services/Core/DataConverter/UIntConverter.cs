using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class UIntConverter
	{
		public static uint Parse(string propertyString)
		{
			return uint.Parse(propertyString, CultureInfo.InvariantCulture);
		}

		public static string ToString(uint propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}
