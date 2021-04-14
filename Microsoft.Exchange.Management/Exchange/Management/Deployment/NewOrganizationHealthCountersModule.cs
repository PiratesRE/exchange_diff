using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.Management.Deployment
{
	internal sealed class NewOrganizationHealthCountersModule : CmdletHealthCountersModule
	{
		public NewOrganizationHealthCountersModule(TaskContext context) : base(context)
		{
		}

		protected override CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.NewOrganizationAttempts;
			}
		}

		protected override CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.NewOrganizationSuccesses;
			}
		}

		protected override CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.NewOrganizationIterationAttempts;
			}
		}

		protected override CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.NewOrganizationIterationSuccesses;
			}
		}

		protected override string TenantNameForMonitoringCounters
		{
			get
			{
				return (string)base.CurrentTaskContext.InvocationInfo.Fields["TenantName"];
			}
		}
	}
}
