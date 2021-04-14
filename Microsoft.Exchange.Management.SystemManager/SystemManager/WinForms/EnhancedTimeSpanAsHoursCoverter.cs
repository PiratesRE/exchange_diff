using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EnhancedTimeSpanAsHoursCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return ((EnhancedTimeSpan)arg).TotalHours.ToString(TextConverter.DoubleFormatString, CultureInfo.InvariantCulture);
		}

		protected override object ParseObject(string s, IFormatProvider provider)
		{
			return EnhancedTimeSpan.FromHours((double)long.Parse(s));
		}
	}
}
