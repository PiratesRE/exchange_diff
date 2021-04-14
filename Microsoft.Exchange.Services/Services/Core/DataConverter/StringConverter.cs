using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class StringConverter : BaseConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return propertyString;
		}

		public override string ConvertToString(object propertyValue)
		{
			return (string)propertyValue;
		}
	}
}
