using System;

namespace Microsoft.Exchange.Monitoring
{
	public class LyncAutodiscoverResult
	{
		public bool IsUcwaSupported { get; set; }

		public string UcwaDiscoveryUrl { get; set; }

		public string Response { get; set; }

		public string DiagnosticInfo { get; set; }
	}
}
