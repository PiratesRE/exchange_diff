using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class NullableDateTimeAsLogTimeCoverter : TextConverter
	{
		protected override string NullValueText
		{
			get
			{
				return Strings.LogTimeNever.ToString();
			}
		}

		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return ((ExDateTime)arg).ToString("F");
		}
	}
}
