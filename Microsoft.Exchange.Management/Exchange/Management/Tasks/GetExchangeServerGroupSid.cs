using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("get", "ExchangeServerGroupSid", SupportsShouldProcess = true)]
	public sealed class GetExchangeServerGroupSid : SetupTaskBase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.WriteObject(this.exs.Sid.ToString());
			TaskLogger.LogExit();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.exs = base.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.ExSWkGuid);
			if (this.exs == null)
			{
				base.ThrowTerminatingError(new ExSGroupNotFoundException(WellKnownGuid.ExSWkGuid), ErrorCategory.ObjectNotFound, null);
			}
			base.LogReadObject(this.exs);
			TaskLogger.LogExit();
		}

		private ADGroup exs;
	}
}
