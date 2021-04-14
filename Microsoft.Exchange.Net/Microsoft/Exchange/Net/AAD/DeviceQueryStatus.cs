using System;

namespace Microsoft.Exchange.Net.AAD
{
	public enum DeviceQueryStatus
	{
		Unknown,
		Success,
		DeviceNotFound,
		DeviceNotManaged,
		DeviceNotCompliant,
		DeviceNotEnabled,
		PolicyEvaluationFailure
	}
}
