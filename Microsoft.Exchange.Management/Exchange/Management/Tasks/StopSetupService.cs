using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("stop", "setupservice")]
	public sealed class StopSetupService : ManageSetupService
	{
		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!base.Fields.IsModified("FailIfServiceNotInstalled"))
			{
				base.FailIfServiceNotInstalled = false;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.StopService(base.ServiceName, base.IgnoreTimeout, base.FailIfServiceNotInstalled, base.MaximumWaitTime);
			TaskLogger.LogExit();
		}
	}
}
