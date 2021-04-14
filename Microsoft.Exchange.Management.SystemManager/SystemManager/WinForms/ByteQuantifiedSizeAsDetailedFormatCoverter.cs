using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ByteQuantifiedSizeAsDetailedFormatCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			ByteQuantifiedSize byteQuantifiedSize = (ByteQuantifiedSize)arg;
			if (byteQuantifiedSize.ToGB() > 0UL)
			{
				return Strings.ByteQuantifiedSizeAsGB(byteQuantifiedSize.ToGB());
			}
			if (byteQuantifiedSize.ToMB() > 0UL)
			{
				return Strings.ByteQuantifiedSizeAsMB(byteQuantifiedSize.ToMB());
			}
			return Strings.ByteQuantifiedSizeAsKB(byteQuantifiedSize.ToKB());
		}
	}
}
