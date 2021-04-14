using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Entities.Diagnostics
{
	internal enum EntitiesMetadata
	{
		[DisplayName("ET.CMD")]
		CommandName,
		[DisplayName("ET.CEL")]
		CoreExecutionLatency,
		[DisplayName("ET.CUSTOMDATA")]
		CustomData
	}
}
