using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaLoginParameters
	{
		public OwaLoginParameters()
		{
			this.ShouldDownloadStaticFile = false;
			this.ShouldDownloadStaticFileOnLogonPage = false;
			this.ShouldMeasureClientLatency = true;
			this.CafeOutboundRequestTimeout = TimeSpan.FromSeconds(100.0);
		}

		public bool ShouldDownloadStaticFile { get; set; }

		public bool ShouldDownloadStaticFileOnLogonPage { get; set; }

		public bool ShouldMeasureClientLatency { get; set; }

		public TimeSpan CafeOutboundRequestTimeout { get; set; }
	}
}
