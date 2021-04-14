using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum CultureTypes
	{
		NeutralCultures = 1,
		SpecificCultures = 2,
		InstalledWin32Cultures = 4,
		AllCultures = 7,
		UserCustomCulture = 8,
		ReplacementCultures = 16,
		[Obsolete("This value has been deprecated.  Please use other values in CultureTypes.")]
		WindowsOnlyCultures = 32,
		[Obsolete("This value has been deprecated.  Please use other values in CultureTypes.")]
		FrameworkCultures = 64
	}
}
