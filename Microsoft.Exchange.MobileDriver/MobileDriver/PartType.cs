using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	[Flags]
	internal enum PartType
	{
		Short = 1,
		Concatenated = 2
	}
}
