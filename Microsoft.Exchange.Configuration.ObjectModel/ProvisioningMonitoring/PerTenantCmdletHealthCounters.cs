using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.ProvisioningMonitoring
{
	internal sealed class PerTenantCmdletHealthCounters : CmdletHealthCounters
	{
		internal PerTenantCmdletHealthCounters(string name, string orgName, string hostName, CounterType attempts, CounterType successes, CounterType iterationAttempts, CounterType iterationSuccesses)
		{
			this.cmdletName = name;
			this.instanceName = ProvisioningMonitoringConfig.GetInstanceName(hostName, orgName);
			this.counterForAttempts = attempts;
			this.counterForSuccesses = successes;
			this.counterForIterationAttempts = iterationAttempts;
			this.counterForIterationSuccesses = iterationSuccesses;
		}

		internal override void IncrementInvocationCount()
		{
			TenantMonitor.LogActivity(this.counterForAttempts, this.instanceName);
		}

		internal override void UpdateSuccessCount(ErrorRecord errorRecord)
		{
			if (errorRecord == null || ProvisioningMonitoringConfig.IsExceptionWhiteListedForCmdlet(errorRecord, this.cmdletName))
			{
				TenantMonitor.LogActivity(this.counterForSuccesses, this.instanceName);
			}
		}

		internal override void IncrementIterationInvocationCount()
		{
			TenantMonitor.LogActivity(this.counterForIterationAttempts, this.instanceName);
		}

		internal override void UpdateIterationSuccessCount(ErrorRecord errorRecord)
		{
			if (errorRecord == null || ProvisioningMonitoringConfig.IsExceptionWhiteListedForCmdlet(errorRecord, this.cmdletName))
			{
				TenantMonitor.LogActivity(this.counterForIterationSuccesses, this.instanceName);
			}
		}

		private string cmdletName;

		private string instanceName;

		private CounterType counterForAttempts;

		private CounterType counterForSuccesses;

		private CounterType counterForIterationAttempts;

		private CounterType counterForIterationSuccesses;
	}
}
