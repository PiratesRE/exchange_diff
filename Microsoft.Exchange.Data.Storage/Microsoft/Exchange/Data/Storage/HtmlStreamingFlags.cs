using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum HtmlStreamingFlags
	{
		None = 0,
		FilterHtml = 1,
		Fragment = 2
	}
}
