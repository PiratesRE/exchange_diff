using System;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public abstract class PolicyConfigProviderManager<T> : IPolicyConfigProviderManager where T : PolicyConfigProviderManager<T>, new()
	{
		public static T Instance
		{
			get
			{
				return PolicyConfigProviderManager<T>.instance;
			}
		}

		public virtual PolicyConfigProvider CreateForSyncEngine(Guid organizationId, string auxiliaryStore, bool enablePolicyApplication = true, ExecutionLog logProvider = null)
		{
			throw new NotImplementedException();
		}

		private static readonly T instance = Activator.CreateInstance<T>();
	}
}
