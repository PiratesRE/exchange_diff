using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class WSManBudgetManager : WSManBudgetManagerBase<WSManBudget>
	{
		private WSManBudgetManager()
		{
		}

		internal static WSManBudgetManager Instance
		{
			get
			{
				return WSManBudgetManager.instance;
			}
		}

		protected override string CreateRelatedBudgetKey(AuthZPluginUserToken userToken)
		{
			return WSManTenantBudgetManager.CreateKeyForTenantBudget(userToken);
		}

		protected override void CorrectRelatedBudgetWhenLeak(string key, int delta)
		{
			WSManTenantBudgetManager.Instance.CorrectRunspacesLeakPassively(key, delta);
		}

		private static readonly WSManBudgetManager instance = new WSManBudgetManager();
	}
}
