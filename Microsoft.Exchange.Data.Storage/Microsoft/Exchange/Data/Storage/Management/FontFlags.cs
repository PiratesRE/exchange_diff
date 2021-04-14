using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	public enum FontFlags
	{
		Normal = 0,
		Bold = 1,
		Italic = 2,
		Underline = 4,
		All = 7
	}
}
