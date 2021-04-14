using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal enum PipelineSubmitStatus
	{
		Ok,
		PipelineFull,
		RecipientThrottled,
		TenantThrottled
	}
}
