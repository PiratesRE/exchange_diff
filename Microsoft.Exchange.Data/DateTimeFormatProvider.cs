using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DateTimeFormatProvider : IFormatProvider
	{
		private DateTimeFormatProvider(DateTimeFormatProvider.DateTimeFormat format)
		{
			this.format = format;
		}

		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(DateTimeFormatProvider.DateTimeFormat))
			{
				switch (this.format)
				{
				case DateTimeFormatProvider.DateTimeFormat.UTC:
					return "yyMMddHHmmss'Z'";
				}
				return "yyyyMMddHHmmss'.0Z'";
			}
			return null;
		}

		internal static IFormatProvider UTC = new DateTimeFormatProvider(DateTimeFormatProvider.DateTimeFormat.UTC);

		internal static IFormatProvider Generalized = new DateTimeFormatProvider(DateTimeFormatProvider.DateTimeFormat.Generalized);

		private DateTimeFormatProvider.DateTimeFormat format;

		internal enum DateTimeFormat
		{
			Generalized,
			UTC
		}
	}
}
