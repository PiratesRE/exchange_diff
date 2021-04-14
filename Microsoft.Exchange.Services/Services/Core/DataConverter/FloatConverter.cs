using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class FloatConverter : BaseConverter
	{
		public static float Parse(string propertyString)
		{
			float result;
			try
			{
				result = float.Parse(propertyString, CultureInfo.InvariantCulture);
			}
			catch (OverflowException ex)
			{
				ex.Data["NeverGenerateWatson"] = null;
				throw;
			}
			return result;
		}

		public static string ToString(float propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}

		public override object ConvertToObject(string propertyString)
		{
			return FloatConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return FloatConverter.ToString((float)propertyValue);
		}
	}
}
