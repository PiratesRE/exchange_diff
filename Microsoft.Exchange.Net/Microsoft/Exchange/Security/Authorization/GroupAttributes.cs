using System;

namespace Microsoft.Exchange.Security.Authorization
{
	[CLSCompliant(false)]
	[Flags]
	public enum GroupAttributes : uint
	{
		Mandatory = 1U,
		EnabledByDefault = 2U,
		Enabled = 4U,
		Owner = 8U,
		UseForDenyOnly = 16U,
		Integrity = 32U,
		IntegrityEnabled = 64U,
		IntegrityEnabledDesktop = 128U,
		LogonId = 3221225472U,
		Resource = 536870912U
	}
}
