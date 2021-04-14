using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class BooleanAsMountStatusCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			if (!(bool)arg)
			{
				return Strings.DismountedStatus.ToString();
			}
			return Strings.MountedStatus.ToString();
		}
	}
}
