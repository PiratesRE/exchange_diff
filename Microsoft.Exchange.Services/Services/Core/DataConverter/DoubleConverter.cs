using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class DoubleConverter : BaseConverter
	{
		public static double Parse(string propertyString)
		{
			double result;
			try
			{
				result = double.Parse(propertyString, CultureInfo.InvariantCulture);
			}
			catch (OverflowException ex)
			{
				ex.Data["NeverGenerateWatson"] = null;
				throw;
			}
			return result;
		}

		public static string ToString(double propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}

		public override object ConvertToObject(string propertyString)
		{
			return DoubleConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return DoubleConverter.ToString((double)propertyValue);
		}
	}
}
