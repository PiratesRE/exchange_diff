using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class EndDateTimeCoverter : TextConverter
	{
		protected override object ParseObject(string s, IFormatProvider provider)
		{
			if (string.IsNullOrEmpty(s))
			{
				DateTime today = DateTime.Today;
				return new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
			}
			return base.ParseObject(s, provider);
		}
	}
}
