using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EnhancedTimeSpanAsMinutesCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return ((EnhancedTimeSpan)arg).TotalMinutes.ToString(TextConverter.DoubleFormatString, CultureInfo.InvariantCulture);
		}

		protected override object ParseObject(string s, IFormatProvider provider)
		{
			return EnhancedTimeSpan.FromMinutes((double)long.Parse(s));
		}
	}
}
