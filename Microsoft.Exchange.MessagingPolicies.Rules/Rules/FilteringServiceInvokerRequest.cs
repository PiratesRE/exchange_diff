using System;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class FilteringServiceInvokerRequest
	{
		protected FilteringServiceInvokerRequest(string organizationId, TimeSpan scanTimeout, int textScanLimit, FipsDataStreamFilteringRequest fipsDataStreamFilteringRequest)
		{
			this.OrganizationId = organizationId;
			this.ScanTimeout = scanTimeout;
			this.TextScanLimit = textScanLimit;
			this.FipsDataStreamFilteringRequest = fipsDataStreamFilteringRequest;
		}

		public string OrganizationId { get; private set; }

		public TimeSpan ScanTimeout { get; private set; }

		public int TextScanLimit { get; private set; }

		public FipsDataStreamFilteringRequest FipsDataStreamFilteringRequest { get; private set; }
	}
}
