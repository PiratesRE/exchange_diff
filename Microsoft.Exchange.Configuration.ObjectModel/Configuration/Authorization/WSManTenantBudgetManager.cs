using System;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class WSManTenantBudgetManager : WSManBudgetManagerBase<WSManTenantBudget>
	{
		private WSManTenantBudgetManager()
		{
		}

		internal static WSManTenantBudgetManager Instance
		{
			get
			{
				return WSManTenantBudgetManager.instance;
			}
		}

		internal static string CreateKeyForTenantBudget(AuthZPluginUserToken userToken)
		{
			return userToken.OrgIdInString;
		}

		protected override string CreateKey(AuthZPluginUserToken userToken)
		{
			return WSManTenantBudgetManager.CreateKeyForTenantBudget(userToken);
		}

		protected override IPowerShellBudget CreateBudget(AuthZPluginUserToken userToken)
		{
			return userToken.CreateBudget(BudgetType.WSManTenant);
		}

		protected override void UpdateBudgetsPerfCounter(int size)
		{
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.PerTenantBudgetsDicSize.RawValue = (long)size;
			}
		}

		protected override void UpdateKeyToRemoveBudgetsPerfCounter(int size)
		{
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.PerTenantKeyToRemoveBudgetsCacheSize.RawValue = (long)size;
			}
		}

		protected override void UpdateConnectionLeakPerfCounter(int leakedConnection)
		{
		}

		protected override bool ShouldThrottling(AuthZPluginUserToken userToken)
		{
			return userToken.OrgId != null && !userToken.OrgId.Equals(OrganizationId.ForestWideOrgId);
		}

		private static readonly WSManTenantBudgetManager instance = new WSManTenantBudgetManager();
	}
}
