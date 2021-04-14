using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public sealed class ActiveDirectoryConnectivityOutcome : TransactionOutcomeBase
	{
		internal ActiveDirectoryConnectivityOutcome(ActiveDirectoryConnectivityContext context, TestActiveDirectoryConnectivityTask.ScenarioId scenarioId, LocalizedString scenario, string performanceCounter, string domainController) : base(domainController, scenario, "", performanceCounter, string.Empty)
		{
			this.Id = scenarioId;
			this.ActiveDirectory = domainController;
		}

		public string ActiveDirectory { get; private set; }

		public TestActiveDirectoryConnectivityTask.ScenarioId Id { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}, {1}", base.Scenario, base.Result);
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ActiveDirectoryConnectivityOutcome.schema;
			}
		}

		internal TimeSpan? Timeout { get; set; }

		internal void UpdateTarget(string targetServer)
		{
			this.ActiveDirectory = targetServer;
		}

		private static ActiveDirectoryConnectivityOutcomeSchema schema = ObjectSchema.GetInstance<ActiveDirectoryConnectivityOutcomeSchema>();
	}
}
