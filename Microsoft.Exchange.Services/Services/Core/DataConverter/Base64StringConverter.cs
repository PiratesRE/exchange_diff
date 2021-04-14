using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class Base64StringConverter : BaseConverter
	{
		public static byte[] Parse(string propertyString)
		{
			return Convert.FromBase64String(propertyString);
		}

		public static string ToString(byte[] propertyValue)
		{
			return Convert.ToBase64String(propertyValue);
		}

		public override object ConvertToObject(string propertyString)
		{
			return Base64StringConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return Base64StringConverter.ToString((byte[])propertyValue);
		}
	}
}
