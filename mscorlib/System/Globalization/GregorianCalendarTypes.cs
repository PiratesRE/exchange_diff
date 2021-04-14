using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public enum GregorianCalendarTypes
	{
		Localized = 1,
		USEnglish,
		MiddleEastFrench = 9,
		Arabic,
		TransliteratedEnglish,
		TransliteratedFrench
	}
}
