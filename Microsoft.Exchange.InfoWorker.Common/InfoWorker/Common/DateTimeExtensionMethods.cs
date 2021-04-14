using System;
using System.Globalization;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class DateTimeExtensionMethods
	{
		public static string ToIso8061(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
		}

		private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
	}
}
