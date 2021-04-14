using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum DefaultOptionsActionMetadata
	{
		[DisplayName("OPT", "RT")]
		RbacTime,
		[DisplayName("OPT", "CT")]
		CmdletTime
	}
}
