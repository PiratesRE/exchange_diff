using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class IconIndexConverter : EnumConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return EnumUtilities.Parse<IconIndexType>(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			IconIndexType value = (IconIndexType)propertyValue;
			if (!EnumUtilities.IsDefined<IconIndexType>(value))
			{
				value = IconIndexType.Default;
			}
			return EnumUtilities.ToString<IconIndexType>(value);
		}
	}
}
