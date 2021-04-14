using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal enum EscapeSequence
	{
		None,
		NotRecognized,
		Incomplete,
		JisX0208_1978,
		JisX0208_1983,
		JisX0201K_1976,
		JisX0201_1976,
		JisX0212_1990,
		JisX0208_Nec,
		Iso646Irv,
		ShiftIn,
		ShiftOut,
		JisX0208_1990,
		Cns11643_1992_1,
		Kcs5601_1987,
		Unknown_1,
		EucKsc,
		Gb2312_1980,
		NECKanjiIn
	}
}
