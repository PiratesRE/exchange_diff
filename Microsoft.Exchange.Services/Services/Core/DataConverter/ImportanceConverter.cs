using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ImportanceConverter : EnumConverter
	{
		public static Importance Parse(string propertyString)
		{
			if (propertyString != null)
			{
				Importance result;
				if (!(propertyString == "Low"))
				{
					if (!(propertyString == "Normal"))
					{
						if (!(propertyString == "High"))
						{
							goto IL_3C;
						}
						result = Importance.High;
					}
					else
					{
						result = Importance.Normal;
					}
				}
				else
				{
					result = Importance.Low;
				}
				return result;
			}
			IL_3C:
			throw new FormatException("Invalid Importance string: " + propertyString);
		}

		public static string ToString(Importance propertyValue)
		{
			string result = null;
			switch (propertyValue)
			{
			case Importance.Low:
				result = "Low";
				break;
			case Importance.Normal:
				result = "Normal";
				break;
			case Importance.High:
				result = "High";
				break;
			}
			return result;
		}

		public override object ConvertToObject(string propertyString)
		{
			return ImportanceConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return ImportanceConverter.ToString((Importance)propertyValue);
		}
	}
}
