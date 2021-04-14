using System;

namespace Microsoft.Exchange.Diagnostics
{
	public enum BudgetMetadata
	{
		ThrottlingRequestType,
		ThrottlingDelay,
		BeginBudgetConnections,
		EndBudgetConnections,
		BeginBudgetHangingConnections,
		EndBudgetHangingConnections,
		BeginBudgetAD,
		EndBudgetAD,
		BeginBudgetCAS,
		EndBudgetCAS,
		BeginBudgetRPC,
		EndBudgetRPC,
		BeginBudgetFindCount,
		EndBudgetFindCount,
		BeginBudgetSubscriptions,
		EndBudgetSubscriptions,
		ThrottlingPolicy,
		MDBResource,
		MDBHealth,
		MDBHistoricalLoad,
		TotalDCRequestCount,
		TotalDCRequestLatency,
		TotalMBXRequestCount,
		TotalMBXRequestLatency,
		MaxConn,
		MaxBurst,
		BeginBalance,
		Cutoff,
		RechargeRate,
		IsServiceAct,
		LiveTime,
		EndBalance
	}
}
