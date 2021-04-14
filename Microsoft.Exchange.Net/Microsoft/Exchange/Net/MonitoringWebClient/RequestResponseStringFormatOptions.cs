using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	[Flags]
	internal enum RequestResponseStringFormatOptions
	{
		None = 0,
		NoBody = 1,
		TruncateCookies = 2
	}
}
