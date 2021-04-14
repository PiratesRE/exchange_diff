using System;
using System.Management.Automation;

namespace Microsoft.Exchange.ProvisioningMonitoring
{
	internal class CmdletHealthCounters
	{
		internal virtual void IncrementInvocationCount()
		{
		}

		internal virtual void UpdateSuccessCount(ErrorRecord errorRecord)
		{
		}

		internal virtual void IncrementIterationInvocationCount()
		{
		}

		internal virtual void UpdateIterationSuccessCount(ErrorRecord errorRecord)
		{
		}
	}
}
