using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class BooleanConverter : BaseConverter
	{
		public static bool Parse(string booleanString)
		{
			string a;
			if ((a = booleanString.ToLowerInvariant()) != null)
			{
				if (a == "false" || a == "0")
				{
					return false;
				}
				if (a == "true" || a == "1")
				{
					return true;
				}
			}
			throw new FormatException("Invalid property value for boolean parsing: " + booleanString);
		}

		public static string ToString(bool propertyValue)
		{
			if (!propertyValue)
			{
				return "false";
			}
			return "true";
		}

		public override object ConvertToObject(string propertyString)
		{
			return BooleanConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return BooleanConverter.ToString((bool)propertyValue);
		}
	}
}
