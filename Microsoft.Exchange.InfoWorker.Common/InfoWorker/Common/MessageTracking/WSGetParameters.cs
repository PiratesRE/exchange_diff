using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class WSGetParameters
	{
		internal MessageTrackingReportId MessageTrackingReportId { get; private set; }

		internal WebServiceTrackingAuthority WSAuthority { get; private set; }

		internal WSGetParameters(MessageTrackingReportId reportId, WebServiceTrackingAuthority wsAuthority)
		{
			this.MessageTrackingReportId = reportId;
			this.WSAuthority = wsAuthority;
		}

		public override string ToString()
		{
			return this.MessageTrackingReportId.ToString();
		}
	}
}
