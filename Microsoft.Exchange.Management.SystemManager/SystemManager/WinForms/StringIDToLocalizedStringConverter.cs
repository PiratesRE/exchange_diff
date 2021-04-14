using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class StringIDToLocalizedStringConverter : TextConverter
	{
		protected override object ParseObject(string s, IFormatProvider provider)
		{
			if (string.IsNullOrEmpty(s))
			{
				return LocalizedString.Empty;
			}
			return Strings.GetLocalizedString((Strings.IDs)Enum.Parse(typeof(Strings.IDs), s));
		}
	}
}
