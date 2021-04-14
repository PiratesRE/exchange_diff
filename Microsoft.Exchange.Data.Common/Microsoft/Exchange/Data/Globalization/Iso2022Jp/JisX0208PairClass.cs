using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	[Flags]
	internal enum JisX0208PairClass
	{
		Unrecognized = 1,
		IbmExtension = 2,
		Cp932 = 4
	}
}
