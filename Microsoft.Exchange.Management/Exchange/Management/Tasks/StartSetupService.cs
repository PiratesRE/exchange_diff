using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("start", "setupservice")]
	public sealed class StartSetupService : ManageSetupService
	{
		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!base.Fields.IsModified("FailIfServiceNotInstalled"))
			{
				base.FailIfServiceNotInstalled = true;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.StartService(base.ServiceName, base.IgnoreTimeout, base.FailIfServiceNotInstalled, base.MaximumWaitTime, base.ServiceParameters);
			TaskLogger.LogExit();
		}
	}
}
