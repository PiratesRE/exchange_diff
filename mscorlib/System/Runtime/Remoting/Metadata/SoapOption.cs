using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum SoapOption
	{
		None = 0,
		AlwaysIncludeTypes = 1,
		XsdString = 2,
		EmbedAll = 4,
		Option1 = 8,
		Option2 = 16
	}
}
