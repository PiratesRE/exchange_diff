using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Autodiscover
{
	public enum AutoDiscoverMetadata
	{
		RedirectType,
		FrontEndServer,
		[DisplayName("AutodiscoverRedirect")]
		AutodiscoverRedirect
	}
}
