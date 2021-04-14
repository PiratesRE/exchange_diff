using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class SensitivityConverter : EnumConverter
	{
		public static Sensitivity Parse(string propertyString)
		{
			if (propertyString != null)
			{
				Sensitivity result;
				if (!(propertyString == "Normal"))
				{
					if (!(propertyString == "Personal"))
					{
						if (!(propertyString == "Private"))
						{
							if (!(propertyString == "Confidential") && !(propertyString == "CompanyConfidential"))
							{
								goto IL_5A;
							}
							result = Sensitivity.CompanyConfidential;
						}
						else
						{
							result = Sensitivity.Private;
						}
					}
					else
					{
						result = Sensitivity.Personal;
					}
				}
				else
				{
					result = Sensitivity.Normal;
				}
				return result;
			}
			IL_5A:
			throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Sensitivity type not supported '{0}'", new object[]
			{
				propertyString
			}));
		}

		public static string ToString(Sensitivity propertyValue)
		{
			string result = null;
			switch (propertyValue)
			{
			case Sensitivity.Normal:
				result = "Normal";
				break;
			case Sensitivity.Personal:
				result = "Personal";
				break;
			case Sensitivity.Private:
				result = "Private";
				break;
			case Sensitivity.CompanyConfidential:
				result = "Confidential";
				break;
			}
			return result;
		}

		public override object ConvertToObject(string propertyString)
		{
			return SensitivityConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return SensitivityConverter.ToString((Sensitivity)propertyValue);
		}
	}
}
