using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class CompleteOrganizationUpgradeHealthCountersModule : CmdletHealthCountersModule
	{
		public CompleteOrganizationUpgradeHealthCountersModule(TaskContext context) : base(context)
		{
		}

		protected override CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.CompleteOrganizationUpgradeAttempts;
			}
		}

		protected override CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.CompleteOrganizationUpgradeSuccesses;
			}
		}

		protected override CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.CompleteOrganizationUpgradeIterationAttempts;
			}
		}

		protected override CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.CompleteOrganizationUpgradeIterationSuccesses;
			}
		}
	}
}
