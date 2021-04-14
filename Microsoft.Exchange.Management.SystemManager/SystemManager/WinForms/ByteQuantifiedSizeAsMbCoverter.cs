using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ByteQuantifiedSizeAsMbCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return ((ByteQuantifiedSize)arg).ToMB().ToString(CultureInfo.InvariantCulture);
		}

		protected override object ParseObject(string s, IFormatProvider provider)
		{
			return ByteQuantifiedSize.Parse(s, ByteQuantifiedSize.Quantifier.MB);
		}
	}
}
