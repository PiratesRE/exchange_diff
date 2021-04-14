using System;
using Microsoft.Exchange.Cluster.Common.ConfigurableParameters;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class AutoReseedWorkflowStateValues : ConfigurableParameterAccessorBase
	{
		public AutoReseedWorkflowStateValues(Guid dbGuid, AutoReseedWorkflowType workflowType) : base(AutoReseedWorkflowStateDefinitions.Instance, Dependencies.Assert)
		{
			this.m_stateKey = string.Format("{0}\\{1}\\{2}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed", dbGuid.ToString(), workflowType.ToString());
			base.LoadInitialValues();
		}

		protected override StateAccessor GetStateAccessor()
		{
			return new RegistryStateAccess(this.m_stateKey);
		}

		private const string StateRootKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed";

		private const string StateKeyFormat = "{0}\\{1}\\{2}";

		private readonly string m_stateKey;
	}
}
