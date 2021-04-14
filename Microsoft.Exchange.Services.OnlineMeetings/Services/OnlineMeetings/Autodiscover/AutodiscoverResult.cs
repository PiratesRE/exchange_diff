using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal class AutodiscoverResult : LogEntry
	{
		internal AutodiscoverResult()
		{
			this.AuthenticatedRedirects = new List<string>();
			this.UnauthenticatedRedirects = new List<string>();
		}

		internal string SipUri { get; set; }

		internal bool IsOnlineMeetingEnabled { get; set; }

		internal bool IsAuthdServerFromCache { get; set; }

		internal string AuthenticatedLyncAutodiscoverServer { get; set; }

		internal bool IsUcwaUrlFromCache { get; set; }

		internal bool IsUcwaSupported
		{
			get
			{
				return !string.IsNullOrEmpty(this.UcwaDiscoveryUrl);
			}
		}

		internal string UcwaDiscoveryUrl { get; set; }

		internal List<string> UnauthenticatedRedirects { get; set; }

		internal List<string> AuthenticatedRedirects { get; set; }

		internal AutodiscoverError Error { get; set; }

		internal string ResponseBody { get; set; }

		internal bool HasError
		{
			get
			{
				return this.Error != null;
			}
		}
	}
}
