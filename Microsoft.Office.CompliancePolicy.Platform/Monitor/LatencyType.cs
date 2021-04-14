using System;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	internal enum LatencyType
	{
		Initialization,
		FfoWsCall,
		CrudMgr,
		TenantInfo,
		PersistentQueue
	}
}
