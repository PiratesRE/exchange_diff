using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EnhancedTimeSpanAsDaysCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return ((EnhancedTimeSpan)arg).TotalDays.ToString(TextConverter.DoubleFormatString, CultureInfo.InvariantCulture);
		}

		protected override object ParseObject(string s, IFormatProvider provider)
		{
			return EnhancedTimeSpan.FromDays((double)long.Parse(s));
		}
	}
}
