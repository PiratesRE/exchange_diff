using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ShortConverter : BaseConverter
	{
		public static short Parse(string propertyString)
		{
			short result;
			try
			{
				result = short.Parse(propertyString, CultureInfo.InvariantCulture);
			}
			catch (OverflowException ex)
			{
				ex.Data["NeverGenerateWatson"] = null;
				throw;
			}
			return result;
		}

		public static string ToString(short propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}

		public override object ConvertToObject(string propertyString)
		{
			return ShortConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return ShortConverter.ToString((short)propertyValue);
		}
	}
}
