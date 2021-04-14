using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum AmDbActionStatus
	{
		None = 1,
		Started,
		Completed,
		Cancelled,
		Failed,
		AcllInitiated,
		AcllSuccessful,
		AcllFailed,
		StoreMountInitiated,
		StoreMountSuccessful,
		StoreMountFailed,
		StoreDismountInitiated,
		StoreDismountSuccessful,
		StoreDismountFailed,
		UpdateMasterServerInitiated,
		UpdateMasterServerFinished
	}
}
