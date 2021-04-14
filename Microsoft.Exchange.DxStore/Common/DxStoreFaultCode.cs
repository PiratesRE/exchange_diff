using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public enum DxStoreFaultCode
	{
		General,
		Unknown,
		Stale,
		InstanceNotReady,
		ServerTimeout,
		ConstraintNotSatisfied
	}
}
