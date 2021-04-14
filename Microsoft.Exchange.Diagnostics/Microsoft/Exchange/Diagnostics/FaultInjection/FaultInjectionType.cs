using System;

namespace Microsoft.Exchange.Diagnostics.FaultInjection
{
	public enum FaultInjectionType
	{
		None,
		Sync,
		Exception,
		Investigate,
		ChangeValue
	}
}
