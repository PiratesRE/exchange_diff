using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	internal sealed class GetManagementEndpointHealthCountersModule : CmdletHealthCountersModule
	{
		public GetManagementEndpointHealthCountersModule(TaskContext context) : base(context)
		{
		}

		protected override CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.GetManagementEndpointAttempts;
			}
		}

		protected override CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.GetManagementEndpointSuccesses;
			}
		}

		protected override CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.GetManagementEndpointIterationAttempts;
			}
		}

		protected override CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.GetManagementEndpointIterationSuccesses;
			}
		}

		protected override string TenantNameForMonitoringCounters
		{
			get
			{
				return base.CurrentTaskContext.InvocationInfo.Fields["DomainName"].ToString();
			}
		}
	}
}
