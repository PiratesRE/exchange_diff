using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ByteQuantifiedSizeAsKbCoverter : TextConverter
	{
		protected override string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			return ((((ByteQuantifiedSize)arg).ToBytes() == 0UL) ? 0UL : Math.Max(1UL, ((ByteQuantifiedSize)arg).ToKB())).ToString(CultureInfo.InvariantCulture);
		}

		protected override object ParseObject(string s, IFormatProvider provider)
		{
			return ByteQuantifiedSize.Parse(s, ByteQuantifiedSize.Quantifier.KB);
		}
	}
}
