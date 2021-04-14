using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum ValidateModernGroupAliasMetadata
	{
		[DisplayName("VMGA", "ER")]
		Exception,
		[DisplayName("VMGA", "AQT")]
		AADQueryTime
	}
}
