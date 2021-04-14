using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct LocalizedDayOfWeek : IFormattable
	{
		public LocalizedDayOfWeek(DayOfWeek dayOfWeek)
		{
			this.dayOfWeek = dayOfWeek;
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.dayOfWeek;
			}
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return LocalizedDayOfWeek.GetDateTimeFormatInfo(formatProvider).DayNames[(int)this.dayOfWeek];
		}

		public override string ToString()
		{
			return this.ToString(string.Empty, null);
		}

		internal static DateTimeFormatInfo GetDateTimeFormatInfo(IFormatProvider formatProvider)
		{
			CultureInfo cultureInfo = formatProvider as CultureInfo;
			DateTimeFormatInfo dateTimeFormatInfo = (cultureInfo != null) ? cultureInfo.DateTimeFormat : null;
			if (dateTimeFormatInfo == null)
			{
				dateTimeFormatInfo = (formatProvider as DateTimeFormatInfo);
			}
			if (dateTimeFormatInfo == null)
			{
				dateTimeFormatInfo = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
			}
			return dateTimeFormatInfo;
		}

		private DayOfWeek dayOfWeek;
	}
}
