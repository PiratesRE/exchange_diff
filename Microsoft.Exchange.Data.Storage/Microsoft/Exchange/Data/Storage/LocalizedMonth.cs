using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct LocalizedMonth : IFormattable
	{
		public LocalizedMonth(int month)
		{
			this.month = month;
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return LocalizedDayOfWeek.GetDateTimeFormatInfo(formatProvider).MonthNames[this.month - 1];
		}

		private int month;
	}
}
