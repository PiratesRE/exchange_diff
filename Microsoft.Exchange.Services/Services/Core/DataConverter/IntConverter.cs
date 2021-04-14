using System;
using System.Globalization;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class IntConverter : BaseConverter
	{
		public static int Parse(string propertyString)
		{
			int result;
			try
			{
				result = int.Parse(propertyString, CultureInfo.InvariantCulture);
			}
			catch (OverflowException ex)
			{
				ex.Data["NeverGenerateWatson"] = null;
				throw;
			}
			return result;
		}

		public static string ToString(int propertyValue)
		{
			return propertyValue.ToString(CultureInfo.InvariantCulture);
		}

		public override object ConvertToObject(string propertyString)
		{
			return IntConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return IntConverter.ToString((int)propertyValue);
		}
	}
}
