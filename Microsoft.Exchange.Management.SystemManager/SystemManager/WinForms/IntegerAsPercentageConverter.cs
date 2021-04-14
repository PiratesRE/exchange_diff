using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class IntegerAsPercentageConverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return string.Format("{0}%", (int)arg);
		}
	}
}
