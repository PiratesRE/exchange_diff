using System;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal enum RpsAuthZMetadata
	{
		IsAuthorized,
		Operation,
		Action,
		Function,
		AuthorizeUser,
		AuthorizeOperation,
		GetQuota,
		WSManOperationComplete,
		WSManUserComplete,
		WSManQuotaComplete,
		ValidateConnectionLimit,
		GetApplicationPrivateData,
		GetInitialSessionState,
		VariantConfigurationSnapshot,
		CmdletFlightEnabled,
		CmdletFlightDisabled,
		InitializeAppSettings,
		InitThrottlingPerfCounters,
		InitDirectoryTopologyMode,
		ServerActiveRunspaces,
		ServerActiveUsers,
		UserBudgetOnStart,
		TenantBudgetOnStart
	}
}
