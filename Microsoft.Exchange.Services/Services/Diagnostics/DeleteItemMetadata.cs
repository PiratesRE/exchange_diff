using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public enum DeleteItemMetadata
	{
		[DisplayName("DI", "ACT")]
		ActionType,
		[DisplayName("DI", "ST")]
		SessionType,
		[DisplayName("DI", "PRIP")]
		Principal
	}
}
