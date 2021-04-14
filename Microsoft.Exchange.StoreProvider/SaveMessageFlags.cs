using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum SaveMessageFlags
	{
		None = 0,
		Unicode = -2147483648,
		BestBody = 1,
		PlainText = 2,
		Html = 4,
		Rtf = 8
	}
}
