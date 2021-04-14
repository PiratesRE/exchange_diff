using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class GuidConverter : BaseConverter
	{
		public static Guid Parse(string propertyString)
		{
			return new Guid(propertyString);
		}

		public static string ToString(Guid propertyValue)
		{
			return propertyValue.ToString("D");
		}

		public override object ConvertToObject(string propertyString)
		{
			return GuidConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return GuidConverter.ToString((Guid)propertyValue);
		}
	}
}
