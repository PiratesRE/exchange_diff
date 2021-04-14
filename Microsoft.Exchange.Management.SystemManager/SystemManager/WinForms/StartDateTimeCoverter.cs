using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class StartDateTimeCoverter : TextConverter
	{
		protected override object ParseObject(string s, IFormatProvider provider)
		{
			if (string.IsNullOrEmpty(s))
			{
				return DateTime.Today;
			}
			return base.ParseObject(s, provider);
		}
	}
}
