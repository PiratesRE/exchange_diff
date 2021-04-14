using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetPeopleICommunicateWithMetadata
	{
		[DisplayName("GPICW", "TgEm")]
		TargetEmailAddress,
		[DisplayName("GPICW", "GPICWFail")]
		GetPeopleICommunicateWithFailed,
		[DisplayName("GPICW", "RCT")]
		ResponseContentType
	}
}
