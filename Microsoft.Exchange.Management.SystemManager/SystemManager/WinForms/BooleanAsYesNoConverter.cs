using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class BooleanAsYesNoConverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			if (!(bool)arg)
			{
				return Strings.NoString.ToString();
			}
			return Strings.YesString.ToString();
		}
	}
}
