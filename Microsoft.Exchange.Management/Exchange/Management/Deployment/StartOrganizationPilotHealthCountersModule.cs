using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class StartOrganizationPilotHealthCountersModule : CmdletHealthCountersModule
	{
		public StartOrganizationPilotHealthCountersModule(TaskContext context) : base(context)
		{
		}

		protected override CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.StartOrganizationPilotAttempts;
			}
		}

		protected override CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.StartOrganizationPilotSuccesses;
			}
		}

		protected override CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.StartOrganizationPilotIterationAttempts;
			}
		}

		protected override CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.StartOrganizationPilotIterationSuccesses;
			}
		}
	}
}
