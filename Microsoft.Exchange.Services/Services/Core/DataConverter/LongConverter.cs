using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class LongConverter : BaseConverter
	{
		public static long Parse(string propertyString)
		{
			long result;
			try
			{
				result = long.Parse(propertyString, CultureInfo.InvariantCulture);
			}
			catch (OverflowException ex)
			{
				ex.Data["NeverGenerateWatson"] = null;
				throw;
			}
			return result;
		}

		public static string ToString(long propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}

		public override object ConvertToObject(string propertyString)
		{
			return LongConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return LongConverter.ToString((long)propertyValue);
		}
	}
}
