using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	[Serializable]
	public enum FormatterTypeStyle
	{
		TypesWhenNeeded,
		TypesAlways,
		XsdString
	}
}
