using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class RemoveOrgHealthCountersModule : CmdletHealthCountersModule
	{
		public RemoveOrgHealthCountersModule(TaskContext context) : base(context)
		{
		}

		protected override CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.RemoveOrganizationAttempts;
			}
		}

		protected override CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.RemoveOrganizationSuccesses;
			}
		}

		protected override CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.RemoveOrganizationIterationAttempts;
			}
		}

		protected override CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.RemoveOrganizationIterationSuccesses;
			}
		}

		protected override string TenantNameForMonitoringCounters
		{
			get
			{
				OrganizationIdParameter organizationIdParameter = base.CurrentTaskContext.InvocationInfo.Fields["Identity"] as OrganizationIdParameter;
				if (organizationIdParameter != null)
				{
					return organizationIdParameter.ToString();
				}
				return base.TenantNameForMonitoringCounters;
			}
		}
	}
}
