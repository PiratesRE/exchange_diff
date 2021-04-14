using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	internal enum MessageHygieneFlags
	{
		None = 0,
		AntispamBypass = 1
	}
}
