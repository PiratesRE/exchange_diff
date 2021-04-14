using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.ComplianceData
{
	public abstract class ComplianceItemContainer : IDisposable
	{
		public abstract string Id { get; }

		public abstract bool HasItems { get; }

		public abstract string Template { get; }

		public abstract bool SupportsAssociation { get; }

		public abstract bool SupportsBinding { get; }

		public virtual int Level
		{
			get
			{
				return -1;
			}
		}

		public abstract List<ComplianceItemContainer> Ancestors { get; }

		public abstract void UpdatePolicy(Dictionary<PolicyScenario, List<PolicyRuleConfig>> rules);

		public abstract void AddPolicy(PolicyDefinitionConfig definition, PolicyRuleConfig rule);

		public abstract void RemovePolicy(Guid id, PolicyScenario scenario);

		public abstract bool HasPolicy(Guid policyId, PolicyScenario scenario);

		public abstract void ForEachChildContainer(Action<ComplianceItemContainer> containerHandler, Func<ComplianceItemContainer, Exception, bool> exHandler);

		public abstract bool SupportsPolicy(PolicyScenario scenario);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void Dispose(bool disposing);
	}
}
