using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EnhancedTimeSpanAsDetailedFormatCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			EnhancedTimeSpan enhancedTimeSpan = (EnhancedTimeSpan)arg;
			if (enhancedTimeSpan.Days > 0)
			{
				return Strings.EnhancedTimeSpanDetailedFormat(enhancedTimeSpan.Days, enhancedTimeSpan.Hours, enhancedTimeSpan.Minutes);
			}
			if (enhancedTimeSpan.Hours > 0)
			{
				return Strings.EnhancedTimeSpanDetailedFormatWithoutDays(enhancedTimeSpan.Hours, enhancedTimeSpan.Minutes);
			}
			return Strings.EnhancedTimeSpanDetailedFormatWithoutHours(enhancedTimeSpan.Minutes);
		}
	}
}
