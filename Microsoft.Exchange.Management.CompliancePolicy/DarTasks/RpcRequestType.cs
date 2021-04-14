using System;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	internal enum RpcRequestType
	{
		NotifyTaskStoreChange,
		EnsureTenantMonitoring,
		GetDarTask,
		SetDarTask,
		GetDarTaskAggregate,
		SetDarTaskAggregate,
		RemoveCompletedDarTasks,
		RemoveDarTaskAggregate,
		GetDarInfo
	}
}
